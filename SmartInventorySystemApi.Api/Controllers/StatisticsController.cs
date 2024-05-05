using Microsoft.AspNetCore.Mvc;
using SmartInventorySystemApi.Application.IServices;
using SmartInventorySystemApi.Application.Models.Statistics;

namespace SmartInventorySystemApi.Api.Controllers;

[Route("")]
public class StatisticsController : ApiController
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    [HttpGet("items/popularity")]
    public async Task<List<ItemPopularity>> GetItemsByPopularityAsync(string groupId, int count = 10, CancellationToken cancellationToken = default)
    {
        return await _statisticsService.GetItemsByPopularityAsync(groupId, count, cancellationToken);
    }

    [HttpGet("shelves/items-count")]
    public async Task<List<ShelfLoad>> GetShelvesByItemsCountAsync(string groupId, CancellationToken cancellationToken = default)
    {
        return await _statisticsService.GetShelvesByItemsCountAsync(groupId, cancellationToken);
    }

    [HttpGet("users/activity")]
    public async Task<List<UserActivity>> GetUsersByActivityWithItemsAsync(string groupId, int count = 10, CancellationToken cancellationToken = default)
    {
        return await _statisticsService.GetUsersByActivityWithItemsAsync(groupId, count, cancellationToken);
    }

    [HttpGet("users/items-taken")]
    public async Task<List<UserDebt>> GetUsersWithMostItemsTakenAsync(string groupId, int count = 10, CancellationToken cancellationToken = default)
    {
        return await _statisticsService.GetUsersWithMostItemsTakenAsync(groupId, count, cancellationToken);
    }
}
