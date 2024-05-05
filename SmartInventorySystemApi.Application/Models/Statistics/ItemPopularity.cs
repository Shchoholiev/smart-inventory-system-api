using SmartInventorySystemApi.Application.Models.Dto;

namespace SmartInventorySystemApi.Application.Models.Statistics;

public class ItemPopularity
{
    public ItemDto Item { get; set; }

    public int ActionsCount { get; set; }
}
