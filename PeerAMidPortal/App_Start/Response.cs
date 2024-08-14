using System;
using System.Net;

namespace YardStickPortal;

[Serializable]
public class Response<T>
{
    public bool IsSuccess;
    public string Message;
    public object Result;
    public HttpStatusCode Status;
    public DateTime UpdatedDate;
    public T? Value;

    private Response(bool success, string message, HttpStatusCode status, object result, DateTime date, T? value)
    {
        IsSuccess = success;
        Message = message;
        Status = status;
        UpdatedDate = date;
        Result = result;
        Value = value;
    }

    public static Response<T> Create(bool success, string message, HttpStatusCode status, object result, DateTime date, T? value)
        => new(success, message, status, result, date, value);

    public static Response<T> Create(bool success, string message, HttpStatusCode status, object result, DateTime date)
        => new(success, message, status, result, date, default);
}
