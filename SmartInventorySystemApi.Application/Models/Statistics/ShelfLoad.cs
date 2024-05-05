using SmartInventorySystemApi.Application.Models.Dto;

namespace SmartInventorySystemApi.Application.Models.Statistics;

public class ShelfLoad
{
    public ShelfDto Shelf { get; set; }

    public int ItemsCount { get; set; }
}
