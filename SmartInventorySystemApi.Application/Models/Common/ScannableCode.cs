using SmartInventorySystemApi.Domain.Enums;

namespace SmartInventorySystemApi.Application.Models.Common;

public class ScannableCode
{
    public ScannableCodeType Type { get; set; }

    public string Data { get; set; }
}
