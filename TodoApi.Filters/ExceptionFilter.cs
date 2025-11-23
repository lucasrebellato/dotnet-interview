using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TodoApi.IBusinessLogic.Exceptions;

namespace TodoApi.Filters;

public class ExceptionFilter : IExceptionFilter
{
    private readonly Dictionary<Type, (int StatusCode, string DefaultMessage)> _errors = new()
    {
        {
            typeof(ArgumentException),
            ((int)HttpStatusCode.BadRequest, "There was a problem with the provided arguments")
        },
        {
            typeof(ValidationException),
            ((int)HttpStatusCode.BadRequest, "There was a problem with the provided arguments")
        },
        { 
            typeof(NotFoundException), 
            ((int)HttpStatusCode.NotFound, "The specified resource was not found") 
        }
    };

    public void OnException(ExceptionContext filterContext)
    {
        var exception = filterContext.Exception;
        if (_errors.TryGetValue(exception.GetType(), out var errorInfo))
        {
            var message = !string.IsNullOrWhiteSpace(exception.Message) ? exception.Message : errorInfo.DefaultMessage;
            filterContext.Result =
                new ObjectResult(new { Message = message, StatusCode = errorInfo.StatusCode })
                {
                    StatusCode = errorInfo.StatusCode
                };
            filterContext.ExceptionHandled = true;
        }
        else
        {
            Console.WriteLine($"[ERROR 500] {DateTime.Now:yyyy-MM-dd HH:mm:ss} {exception}");

            filterContext.Result =
                new ObjectResult(new { Message = "There was an error.", StatusCode = 500 })
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            filterContext.ExceptionHandled = true;
        }
    }
}
