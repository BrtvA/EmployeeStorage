using System.Text.Json.Serialization;

namespace EmployeeStorage.API.Domain;

public class Passport
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;
    [JsonPropertyName("number")]
    public string Number { get; set; } = null!;
}
