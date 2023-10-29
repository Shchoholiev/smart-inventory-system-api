using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartInventorySystemApi.Application.Exceptions;
using SmartInventorySystemApi.Application.IRepositories;
using SmartInventorySystemApi.Application.IServices;
using SmartInventorySystemApi.Application.Models;
using SmartInventorySystemApi.Application.Models.CreateDto;
using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Application.Models.GlobalInstances;
using SmartInventorySystemApi.Application.Paging;
using SmartInventorySystemApi.Domain.Entities;
using SmartInventorySystemApi.Infrastructure.Services.Identity;
using LinqKit;
using System.Text.RegularExpressions;

namespace SmartInventorySystemApi.Infrastructure.Services;

public class ItemsService : ServiceBase, IItemsService
{
    private readonly IItemsRepository _itemsRepository;

    private readonly IItemHistoryRepository _itemsHistoryRepository;
    
    private readonly ILogger _logger;

    private readonly IMapper _mapper;

    public ItemsService(
        IItemsRepository itemsRepository,
        IItemHistoryRepository itemsHistoryRepository,
        ILogger<DevicesService> logger,
        IMapper mapper)
    {
        _itemsRepository = itemsRepository;
        _itemsHistoryRepository = itemsHistoryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedList<ItemDto>> GetItemsPageAsync(int page, int size, string groupId, string search, bool? isTaken, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Getting items page {page} with size {size} for group {groupId}.");

        var groupObjectId = ParseObjectId(groupId); 
        Expression<Func<Item, bool>> predicate = PredicateBuilder.New<Item>(i => i.GroupId == groupObjectId);

        if (isTaken.HasValue)
        {
            predicate = predicate.And(i => i.IsTaken == isTaken.Value);
        }

        if (!string.IsNullOrEmpty(search))
        {
            predicate = predicate.And(i => 
                Regex.IsMatch(i.Name, search, RegexOptions.IgnoreCase)
                || (!string.IsNullOrEmpty(i.Description) 
                    && Regex.IsMatch(i.Description, search, RegexOptions.IgnoreCase)));
        }

        var itemsTask = _itemsRepository.GetPageAsync(page, size, predicate, cancellationToken);
        var totalCountTask = _itemsRepository.GetCountAsync(predicate, cancellationToken);

        await Task.WhenAll(itemsTask, totalCountTask);

        var items = await itemsTask;
        var totalCount = await totalCountTask;

        var itemDtos = _mapper.Map<List<ItemDto>>(items);
        var pagedList = new PagedList<ItemDto>(itemDtos, page, size, totalCount);

        _logger.LogInformation($"Retrieved {itemDtos.Count} items.");

        return pagedList;
    }
    
    public async Task<ItemDto> GetItemAsync(string itemId, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Getting item with Id {itemId}.");

        var id = ParseObjectId(itemId);
        var item = await _itemsRepository.GetOneAsync(id, cancellationToken);
        if (item == null)
        {
            throw new EntityNotFoundException($"Item with Id {itemId} is not found in database.");
        }

        var itemDto = _mapper.Map<ItemDto>(item);

        _logger.LogInformation($"Retrieved item with Id {itemId}.");

        return itemDto;
    }

    public async Task<ItemDto> UpdateItemAsync(string itemId, ItemCreateDto itemDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Updating item with Id {itemId}.");

        var id = ParseObjectId(itemId);
        var item = await _itemsRepository.GetOneAsync(id, cancellationToken);
        if (item == null)
        {
            throw new EntityNotFoundException($"Item with Id {itemId} is not found in database.");
        }

        _mapper.Map(itemDto, item);
        var updatedItem = await _itemsRepository.UpdateAsync(item, cancellationToken);
        var itemDtoResult = _mapper.Map<ItemDto>(updatedItem);

        _logger.LogInformation($"Updated item with Id {itemId}.");

        return itemDtoResult;
    }

    public async Task<ItemDto> UpdateItemStatusAsync(string itemId, ItemStatusChangeDto status, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Updating item status with Id {itemId}.");

        var id = ParseObjectId(itemId);
        var item = await _itemsRepository.GetOneAsync(id, cancellationToken);
        if (item == null)
        {
            throw new EntityNotFoundException($"Item with Id {itemId} is not found in database.");
        }

        item.IsTaken = status.IsTaken;
        var updatedItemTask = _itemsRepository.UpdateAsync(item, cancellationToken);

        var itemHistory = new ItemHistory
        {
            ItemId = item.Id,
            IsTaken = item.IsTaken,
            Comment = status.Comment,
            CreatedById = GlobalUser.Id.Value,
            CreatedDateUtc = DateTime.UtcNow
        };
        var itemHistoryTask = _itemsHistoryRepository.AddAsync(itemHistory, cancellationToken);

        await Task.WhenAll(updatedItemTask, itemHistoryTask);

        var updatedItem = await updatedItemTask;
        var itemDtoResult = _mapper.Map<ItemDto>(updatedItem);

        _logger.LogInformation($"Updated item status with Id {itemId}.");

        return itemDtoResult;
    }

    public async Task DeleteItemAsync(string itemId, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Deleting item with Id {itemId}.");

        var id = ParseObjectId(itemId);
        var item = await _itemsRepository.GetOneAsync(id, cancellationToken);
        if (item == null)
        {
            throw new EntityNotFoundException($"Item with Id {itemId} is not found in database.");
        }

        await _itemsRepository.DeleteAsync(item, cancellationToken);

        _logger.LogInformation($"Deleted item with Id {itemId}.");
    }
}
