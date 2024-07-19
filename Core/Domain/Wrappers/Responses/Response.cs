using MediatR;

namespace Shared.Wrappers
{
    /// <summary>
    /// Represents a standard response wrapper for API responses.
    /// </summary>
    /// <typeparam name="T">Type of the data payload.</typeparam>
    public class Response<T>
    {
        /// <summary>
        /// Default constructor for a successful response.
        /// </summary>
        public Response()
        {
            Succeeded = true;
            Message = string.Empty;
            Errors = default;
            Data = default;
        }

        /// <summary>
        /// Constructor for a successful response with data and optional message.
        /// </summary>
        /// <param name="data">The data payload.</param>
        /// <param name="message">Optional message (default is "Success").</param>
        private Response(T? data, string? message = null)
        {
            Succeeded = true;
            Message = message ?? "Success";
            Data = data;
            Errors = default;
        }

        /// <summary>
        /// Constructor for a failed response with optional message and success status.
        /// </summary>
        /// <param name="message">Optional error message (default is "Success").</param>
        /// <param name="succeeded">Success status (default is false).</param>
        private Response(string? message = null, bool succeeded = false)
        {
            Succeeded = succeeded;
            Message = message ?? "Success";
        }

        /// <summary>
        /// Creates a successful response with data and optional message.
        /// </summary>
        /// <param name="data">The data payload.</param>
        /// <param name="message">Optional message (default is "Ok").</param>
        /// <returns>A new instance of Response{T}.</returns>
        public static Response<T> Success(T? data, string? message = "Ok") => new(data, message);

        /// <summary>
        /// Creates a response with a message and success status.
        /// </summary>
        /// <param name="message">Optional message.</param>
        /// <param name="succeeded">Success status (default is false).</param>
        /// <returns>A new instance of Response{T}.</returns>
        public static Response<Unit> MessageResponse(string? message = null, bool succeeded = false) => new(message, succeeded);

        /// <summary>
        /// Creates a failure response with an error message.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>A new instance of Response{T}.</returns>
        public static Response<Unit> Failure(string message) => new(message, false);

        /// <summary>
        /// Indicates if the operation succeeded.
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// Message associated with the response.
        /// </summary>
        public virtual string Message { get; set; }

        /// <summary>
        /// Array of errors, if any.
        /// </summary>
        public virtual string[]? Errors { get; set; }

        /// <summary>
        /// Data payload of the response.
        /// </summary>
        public virtual T? Data { get; set; }
    }
}
