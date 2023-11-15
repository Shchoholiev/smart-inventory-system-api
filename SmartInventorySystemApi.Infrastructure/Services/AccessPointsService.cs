using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using SmartInventorySystemApi.Application.Exceptions;
using SmartInventorySystemApi.Application.IRepositories;
using SmartInventorySystemApi.Application.IServices;
using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Application.Models.GlobalInstances;
using SmartInventorySystemApi.Domain.Entities;
using SmartInventorySystemApi.Domain.Enums;
using SmartInventorySystemApi.Infrastructure.Services.Identity;

namespace SmartInventorySystemApi.Infrastructure.Services;

public class AccessPointsService : ServiceBase, IAccessPointsService
{
    private readonly IScanHistoryRepository _scanHistoryRepository;

    private readonly IDevicesRepository _devicesRepository;

    private readonly IItemsRepository _itemsRepository;

    private readonly IShelvesRepository _shelvesRepository;

    private readonly IShelfControllersService _shelfControllersService;

    private readonly IImageRecognitionService _imageRecognitionService;

    private readonly ILogger _logger;

    private readonly IMapper _mapper;

    public AccessPointsService(
        IScanHistoryRepository scanHistoryRepository,
        IDevicesRepository devicesRepository,
        IItemsRepository itemsRepository,
        IShelvesRepository shelvesRepository,
        IShelfControllersService shelfControllersService,
        IImageRecognitionService imageRecognitionService,
        ILogger<AccessPointsService> logger,
        IMapper mapper)
    {
        _scanHistoryRepository = scanHistoryRepository;
        _devicesRepository = devicesRepository;
        _itemsRepository = itemsRepository;
        _shelvesRepository = shelvesRepository;
        _shelfControllersService = shelfControllersService;
        _imageRecognitionService = imageRecognitionService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task FindItemByImageAsync(string deviceGuid, byte[] image, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Finding item by image for device {deviceGuid}.");

        using var cts = new CancellationTokenSource();

        var findItemByCodeTask = FindItemByScannableCodeAsync(image, cancellationToken);
        var findItemByTagsTask = FindItemByImageAsync(image, cts.Token);
        var searchTask = Task.WhenAny(findItemByCodeTask, findItemByTagsTask);

        var parsedDeviceGuid = Guid.Parse(deviceGuid);
        var deviceTask = _devicesRepository.GetOneAsync(
            d => d.Guid == parsedDeviceGuid && !d.IsDeleted, cancellationToken);
        
        await Task.WhenAll(
            deviceTask,
            searchTask
        );

        ScanType scanType;
        Item item;

        var completedSearchTask = await searchTask;
        // If first completed task is Search by Scannable Codes then cancel image tagging
        // Because Image Tagging will always return tags and can accidentally find item by tags on the image with a QR Code
        if (completedSearchTask == findItemByCodeTask)
        {
            (item, scanType) = await completedSearchTask;
            if (item != null)
            {
                cts.Cancel(); 
            }
        }
        else
        {
            // Handle result of findItemByTagsTask
            (item, scanType) = await completedSearchTask; 
        }

        // Rare scenario, because of this it is happening in parallel with recognition.
        var device = await deviceTask;
        if (device == null)
        {
            throw new EntityNotFoundException($"Device wit Guid: {deviceGuid} is not found in database.");
        }

        // TODO: separate functions
        var tasks = new List<Task>
        {
            Task.Run(async () =>
            {
                var scanHistory = new ScanHistory
                {
                    DeviceId = device.Id,
                    ScanType = scanType,
                    Result = item == null ? "Not Found Item" : "Found Item",
                    CreatedById = GlobalUser.Id ?? ObjectId.Empty, // TODO: Add authorization for Access Point Device
                    CreatedDateUtc = DateTime.UtcNow
                };
                await _scanHistoryRepository.AddAsync(scanHistory, cancellationToken);

                _logger.LogInformation($"Scan history for device {deviceGuid} saved successfully.");
            })
        };
        
        if (item != null)
        {
            tasks.Add(Task.Run(async () =>
            {
                _logger.LogInformation($"Item found by {scanType}.");

                var shelf = await _shelvesRepository.GetOneAsync(item.ShelfId, cancellationToken);
                var comment = $"Light Turned on By AccessPointDevice. {scanType} Scan.";
                await _shelfControllersService.ControlLightAsync(
                    deviceGuid, shelf.PositionInRack, true, item.Id.ToString(), ItemHistoryType.Scan, comment, cancellationToken);
            }));
        }

        await Task.WhenAll(tasks);

        _logger.LogInformation("Finding item by image finished.");
    }

    public async Task<List<ScanHistoryDto>> GetScansHistoryAsync(int page, int size, string deviceId, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Getting scan history for device {deviceId}.");

        var deviceObjectId = ObjectId.Parse(deviceId);
        var scanHistories = await _scanHistoryRepository.GetHistoryPageAsync(page, size, deviceObjectId, cancellationToken);
        var dtos = _mapper.Map<List<ScanHistoryDto>>(scanHistories);

        _logger.LogInformation($"Returning {dtos.Count} scan histories.");

        return dtos;
    }

    private async Task<(Item?, ScanType)> FindItemByScannableCodeAsync(byte[] image, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Finding item by scannable code.");

        // Function runs in parallel so it need try catch to prevent from unexpected failures
        try
        {
            var regex = @".*/items/([0-9a-fA-F]+)";
            var decodedCodes = await _imageRecognitionService.ReadScannableCodeAsync(image, cancellationToken);
            var itemIdFromCode = decodedCodes
                .Where(c => c.Type == ScannableCodeType.QRCode && Regex.IsMatch(c.Data, regex))
                .Select(c => Regex.Match(c.Data, regex).Groups[1].Value)
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(itemIdFromCode))
            {
                var itemObjectId = ParseObjectId(itemIdFromCode);
                var item = await _itemsRepository.GetOneAsync(i => i.Id == itemObjectId && !i.IsDeleted, cancellationToken);

                _logger.LogInformation($"Item found by scannable code.");
                
                // TODO: Add Barcode when it is implemented
                return (item, ScanType.QRCode);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while finding item by scannable code.");
        }

        _logger.LogInformation($"Item not found by scannable code.");

        return (null, ScanType.QRCode);
    }

    private async Task<(Item?, ScanType)> FindItemByImageAsync(byte[] image, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Finding item by image.");

        // Function runs in parallel so it need try catch to prevent from unexpected failures
        try
        {
            var tags = await _imageRecognitionService.GetImageTagsAsync(image, cancellationToken);
            var searchTasks = tags
                .Take(3)
                .Select(tag => _itemsRepository.GetOneAsync(
                    // TODO: Add description? Add isTaken? Use embedded search?
                    i => !i.IsDeleted && Regex.IsMatch(i.Name, tag.Name, RegexOptions.IgnoreCase), 
                    cancellationToken));
            var searchResults = await Task.WhenAll(searchTasks);
            if (searchResults.Any(i => i != null))
            {
                var item = searchResults.First(i => i != null);

                _logger.LogInformation($"Item found by image.");

                return (item, ScanType.Object);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while finding item by image Tag.");
        }

        _logger.LogInformation($"Item not found by image.");

        return (null, ScanType.Object);
    }
}
