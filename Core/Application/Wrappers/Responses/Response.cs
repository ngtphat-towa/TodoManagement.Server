namespace Shared.Wrappers;

public class Response<T>
{
    public Response()
    {
        Succeeded = true;
        Message = string.Empty;
        Errors = default;
        Data = default;
    }

    public Response(T? data, string? message = null)
    {
        Succeeded = true;
        Message = message ?? string.Empty;
        Data = data;
        Errors = default;
    }

    public Response(string message)
    {
        Succeeded = false;
        Message = message;
        Errors = default;
        Data = default;
    }

    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
    public string[]? Errors { get; set; } = null;
    public T? Data { get; set; }
}
