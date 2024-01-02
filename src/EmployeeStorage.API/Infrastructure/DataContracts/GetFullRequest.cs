using System.Text.Json.Serialization;

namespace EmployeeStorage.API.Infrastructure.DataContracts;

public class GetFullRequest : GetRequest
{
    [JsonPropertyName("department_name")]
    public string DepartmentName { get; set; } = null!;
}
