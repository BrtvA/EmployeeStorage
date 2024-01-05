using EmployeeStorage.API.Domain;
using EmployeeStorage.API.Domain.EmployeeAggregate;
using System.Text.Json.Serialization;

namespace EmployeeStorage.API.Infrastructure.DataContracts;

public class UpdateRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("surname")]
    public string? Surname { get; set; }
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }
    [JsonPropertyName("company_id")]
    public int? CompanyId { get; set; }
    [JsonPropertyName("passport")]
    public Passport? Passport { get; set; }
    [JsonPropertyName("department_id")]
    public int? DepartmentId { get; set; }

    public EmployeeExtended MergeWithCurrentData(int id, EmployeeExtended employee) =>
        new EmployeeExtended()
        {
            Id = id,
            Name = Name ?? employee.Name,
            Surname = Surname ?? employee.Surname,
            Phone = Phone ?? employee.Phone,
            CompanyId = CompanyId ?? employee.CompanyId,
            Passport = Passport is not null
                        ? new Passport
                        {
                            Type = Passport.Type ?? employee.Passport.Type,
                            Number = Passport.Number ?? employee.Passport.Number
                        }
                        : employee.Passport,
            DepartmentId = DepartmentId ?? employee.DepartmentId
        };
}