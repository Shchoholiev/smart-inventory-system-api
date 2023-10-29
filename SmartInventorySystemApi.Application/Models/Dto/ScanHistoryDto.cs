using SmartInventorySystemApi.Domain.Enums;

namespace SmartInventorySystemApi.Application.Models.Dto;

public class ScanHistoryDto
{
    public string Id { get; set; }
    
    public ScanType ScanType { get; set; }

    public string Result { get; set; }
}
