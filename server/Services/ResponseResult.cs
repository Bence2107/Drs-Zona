namespace Services;

public class ResponseResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Value { get; set; }
    public string? ErrorField { get; set; }
    public string? Message { get; set; }

    public static ResponseResult<T> Success(T value) => 
        new() { IsSuccess = true, Value = value };

    public static ResponseResult<T> Failure(string message) => 
        new() { IsSuccess = false, Message = message };

    public static ResponseResult<T> Failure(string field, string message) => 
        new() { IsSuccess = false, ErrorField = field, Message = message };
}