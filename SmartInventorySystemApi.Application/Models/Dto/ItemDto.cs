namespace SmartInventorySystemApi.Application.Models.Dto;

public class ItemDto
{
    public string Id { get; set; }
    
    public string Name { get; set; }

    public string? Description { get; set; }
    
    public bool IsTaken { get; set; }
}
