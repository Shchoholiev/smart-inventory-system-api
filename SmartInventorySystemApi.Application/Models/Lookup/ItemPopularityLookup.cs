using SmartInventorySystemApi.Domain.Entities;

namespace SmartInventorySystemApi.Application.Models.Lookup;

public class ItemPopularityLookup
{
    public Item Item { get; set; }

    public int ActionsCount { get; set; }
}
