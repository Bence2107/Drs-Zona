namespace Services;

public class ResponseResult<T>
{
    public bool IsSuccess { get; private init; }
    public T? Value { get; private init; }
    public string? ErrorField { get; private init; }
    public string? Message { get; private init; }

    public static ResponseResult<T> Success(T value) => 
        new() { IsSuccess = true, Value = value };

    public static ResponseResult<T> Failure(string message) => 
        new() { IsSuccess = false, Message = message };

    public static ResponseResult<T> Failure(string field, string message) => 
        new() { IsSuccess = false, ErrorField = field, Message = message };
}