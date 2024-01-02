using System.Text.Json.Serialization;

namespace EmployeeStorage.API.Infrastructure.DataContracts
{
    public class BaseRequest
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}
