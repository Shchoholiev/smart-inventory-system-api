using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartInventorySystemApi.Application.IRepositories;
using SmartInventorySystemApi.Application.IServices;
using SmartInventorySystemApi.Application.Models.Statistics;
using SmartInventorySystemApi.Infrastructure.Services.Identity;

namespace SmartInventorySystemApi.Infrastructure.Services;
public class StatisticsService : ServiceBase, IStatisticsService
{
    public readonly IItemsRepository _itemsRepository;

    public readonly IShelvesRepository _shelvesRepository;

    public readonly IUsersRepository _usersRepository;

    private readonly ILogger _logger;

    private readonly IMapper _mapper;

    public StatisticsService(
        IItemsRepository itemsRepository,
        IShelvesRepository shelvesRepository,
        IUsersRepository usersRepository,
        ILogger<DevicesService> logger,
        IMapper mapper
        ) : base()
    {
        _itemsRepository = itemsRepository;
        _shelvesRepository = shelvesRepository;
        _usersRepository = usersRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<List<ItemPopularity>> GetItemsByPopularityAsync(string groupId, int count = 10, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Getting items popularity statistics for group with Id {groupId}.");

        var objectGroupId = ParseObjectId(groupId);
        var items = await _itemsRepository.GetItemsByPopularityAsync(objectGroupId, count, cancellationToken);
        var itemsDto = _mapper.Map<List<ItemPopularity>>(items);

        _logger.LogInformation($"Retrieved items popularity statistics.");

        return itemsDto;
    }

    public async Task<List<ShelfLoad>> GetShelvesByItemsCountAsync(string groupId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Getting shelves items count statistics for group with Id {groupId}.");

        var objectGroupId = ParseObjectId(groupId);
        var shelves = await _shelvesRepository.GetShelvesByItemsCountAsync(objectGroupId, cancellationToken);
        var shelvesDto = _mapper.Map<List<ShelfLoad>>(shelves);

        _logger.LogInformation($"Retrieved shelves items count statistics.");

        return shelvesDto;
    }

    public async Task<List<UserActivity>> GetUsersByActivityWithItemsAsync(string groupId, int count = 10, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Getting users activity statistics for group with Id {groupId}.");

        var objectGroupId = ParseObjectId(groupId);
        var users = await _usersRepository.GetUsersByActivityWithItemsAsync(objectGroupId, count, cancellationToken);
        var usersDto = _mapper.Map<List<UserActivity>>(users);

        _logger.LogInformation($"Retrieved users activity statistics.");

        return usersDto;
    }

    public async Task<List<UserDebt>> GetUsersWithMostItemsTakenAsync(string groupId, int count = 10, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Getting users with most items statistics for group with Id {groupId}.");

        var objectGroupId = ParseObjectId(groupId);
        var users = await _itemsRepository.GetUsersWithMostItemsTakenAsync(objectGroupId, count, cancellationToken);
        var usersDto = _mapper.Map<List<UserDebt>>(users);

        _logger.LogInformation($"Retrieved users with most items statistics.");

        return usersDto;
    }
}

