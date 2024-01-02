using System.Text.Json.Serialization;

namespace EmployeeStorage.API.Domain.EmployeeAggregate;

public class Passport
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    [JsonPropertyName("number")]
    public string? Number { get; set; }
}
