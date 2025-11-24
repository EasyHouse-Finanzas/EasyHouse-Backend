namespace EasyHouse.Shared.Application;

public class Result<T>
{
    public bool IsSuccess { get; }
    public string? Message { get; }
    public T? Value { get; }

    private Result(bool isSuccess, T? value, string? message)
    {
        IsSuccess = isSuccess;
        Value = value;
        Message = message;
    }

    public static Result<T> Success(T value)
        => new(true, value, null);

    public static Result<T> Failure(string message)
        => new(false, default, message);
}