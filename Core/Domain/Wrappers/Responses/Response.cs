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

    private Response(T? data, string? message = null)
    {
        Succeeded = true;
        Message = message ?? "Success";
        Data = data;
        Errors = default;
    }
    private Response(string? message = null, bool succeeded = false)
    {
        Succeeded = succeeded;
        Message = message ?? "Success";
    }


    public static Response<T> Success(T? data, string? message = "Ok") => new(data, message);
    public static Response<Non> MessageResponse(string? message = null, bool succeeded = false) => new(message, succeeded);
    public static Response<Non> Failure(string message) => new(message, false);

    public bool Succeeded { get; set; }
    public virtual string Message { get; set; }
    public virtual string[]? Errors { get; set; }
    public virtual T? Data { get; set; }
}

public record Non { }