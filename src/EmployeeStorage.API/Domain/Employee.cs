using System.Text.Json.Serialization;

namespace EmployeeStorage.API.Domain;

public class Employee
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("surname")]
    public string Surname { get; set; } = null!;
    [JsonPropertyName("phone")]
    public string Phone { get; set; } = null!;
    [JsonPropertyName("company_id")]
    public int CompanyId { get; set; }
    [JsonPropertyName("passport")]
    public Passport Passport { get; set; } = null!;
    [JsonPropertyName("department")]
    public Department Department { get; set; } = null!;
}
