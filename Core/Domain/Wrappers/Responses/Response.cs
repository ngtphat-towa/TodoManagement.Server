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
        Message = message ?? "Success";
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
    public virtual string Message { get; set; }
    public virtual string[]? Errors { get; set; }
    public virtual T? Data { get; set; }
}
