using SmartInventorySystemApi.Domain.Entities;

namespace SmartInventorySystemApi.Application.Models.Lookup;

public class ShelfLoadLookup
{
    public Shelf Shelf { get; set; }

    public int ItemsCount { get; set; }
}
