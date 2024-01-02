using EmployeeStorage.API.Domain;
using System.Text.Json.Serialization;

namespace EmployeeStorage.API.Infrastructure.DataContracts;

public class UpdateRequest : BaseRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("surname")]
    public string? Surname { get; set; }
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }
    [JsonPropertyName("company_id")]
    public int? CompanyId { get; set; }
    [JsonPropertyName("password")]
    public Passport? Passport { get; set; }
    [JsonPropertyName("department_id")]
    public int? DepartmentId { get; set; }
}