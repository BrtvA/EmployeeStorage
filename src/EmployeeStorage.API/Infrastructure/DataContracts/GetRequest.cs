using System.Text.Json.Serialization;

namespace EmployeeStorage.API.Infrastructure.DataContracts;

public class GetRequest
{
    [JsonPropertyName("company_name")]
    public string CompanyName { get; set; } = null!;
}