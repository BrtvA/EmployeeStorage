using EmployeeStorage.API.Domain;
using EmployeeStorage.API.Domain.EmployeeAggregate;
using System.Text.Json.Serialization;

namespace EmployeeStorage.API.Infrastructure.DataContracts;

public class CreateRequest
{
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
    [JsonPropertyName("department_id")]
    public int DepartmentId { get; set; }

    public EmployeeExtended ConvertToEmployeeExtended() =>
        new EmployeeExtended
        {
            Name = Name,
            Surname = Surname,
            Phone = Phone,
            CompanyId = CompanyId,
            Passport = Passport,
            DepartmentId = DepartmentId
        };
}
