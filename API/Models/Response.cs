using System.Net;

namespace API;

public class Response<T>()
{
    public bool IsSuccess { get; set; }

    public string? Message { get; set; }

    public T? Data { get; set; }

    public HttpStatusCode Status { get; set; }

    public static Response<T> Success(T data, string? message = null, HttpStatusCode status = HttpStatusCode.OK)
    {
        return new Response<T>
        {
            IsSuccess = true,
            Data = data,
            Message = message,
            Status = status
        };
    }

    public static Response<T> Fail(string message, HttpStatusCode status = HttpStatusCode.BadRequest)
    {
        return new Response<T>
        {
            IsSuccess = false,
            Message = message,
            Status = status
        };
    }
}
