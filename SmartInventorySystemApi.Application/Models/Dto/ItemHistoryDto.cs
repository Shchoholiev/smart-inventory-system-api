using SmartInventorySystemApi.Domain.Enums;

namespace SmartInventorySystemApi.Application.Models.Dto;

public class ItemHistoryDto
{
    public ItemHistoryType Type { get; set; }
    
    public bool IsTaken { get; set; }

    public string? Comment { get; set; }
}
