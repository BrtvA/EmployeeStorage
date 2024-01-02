using System.Text.Json.Serialization;

namespace EmployeeStorage.API.Infrastructure.Services;

public class Result<T>
{
    [JsonPropertyName("is_failure")]
    public bool IsFailure { get; private set; }
    [JsonPropertyName("status_code")]
    public int StatusCode { get; private set; }
    [JsonPropertyName("message")]
    public string? Message { get; private set; }
    [JsonPropertyName("data")]
    public T? Value { get; private set; }

    private Result(bool isFailure, int statusCode)
    {
        IsFailure = isFailure;
        StatusCode = statusCode;
    }

    private Result(T value) : this(false, 200)
    {
        Value = value;
    }

    private Result(string message, int statusCode) : this(true, statusCode)
    {
        Message = message;
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }

    public static Result<T> Failure(string message, int statusCode)
    {
        return new Result<T>(message, statusCode);
    }
}
