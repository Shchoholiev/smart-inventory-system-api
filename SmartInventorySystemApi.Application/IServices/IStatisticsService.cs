using SmartInventorySystemApi.Application.Models.Statistics;

namespace SmartInventorySystemApi.Application.IServices;

public interface IStatisticsService
{
    Task<List<ItemPopularity>> GetItemsByPopularityAsync(string groupId, int count = 10, CancellationToken cancellationToken = default);

    Task<List<ShelfLoad>> GetShelvesByItemsCountAsync(string groupId, CancellationToken cancellationToken = default);

    Task<List<UserActivity>> GetUsersByActivityWithItemsAsync(string groupId, int count = 10, CancellationToken cancellationToken = default);

    Task<List<UserDebt>> GetUsersWithMostItemsTakenAsync(string groupId, int count = 10, CancellationToken cancellationToken = default);
}

