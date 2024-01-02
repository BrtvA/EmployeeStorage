using System.Text.Json.Serialization;

namespace EmployeeStorage.API.Domain;

public class Department
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("phone")]
    public string Phone { get; set; } = null!;
}
