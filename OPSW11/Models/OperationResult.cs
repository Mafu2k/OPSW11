namespace OPSW11.Models;

public class OperationResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public string? DetailedOutput { get; init; }

    public static OperationResult Success(string msg = "OK", string? output = null)
        => new() { IsSuccess = true, Message = msg, DetailedOutput = output };

    public static OperationResult Failure(string msg, Exception? ex = null, string? output = null)
        => new() { IsSuccess = false, Message = msg, DetailedOutput = output };
}
