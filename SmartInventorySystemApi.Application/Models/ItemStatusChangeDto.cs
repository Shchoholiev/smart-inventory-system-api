namespace SmartInventorySystemApi.Application.Models;

public class ItemStatusChangeDto
{
    public bool IsTaken { get; set; }

    public string? Comment { get; set; }
}
