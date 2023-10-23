namespace SmartInventorySystemApi.Application.Models;

public class ShelfStatusChangeDto
{
    public bool IsLitUp { get; set; }

    public string? Comment { get; set; }

    public string ItemId { get; set; }
}
