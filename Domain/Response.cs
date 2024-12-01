using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;

namespace Domain
{
    [ExcludeFromCodeCoverage]

    public sealed class Response<T> : Response, IResponse
    {
        public T Data { get; private set; } = default!;
    }

    [ExcludeFromCodeCoverage]

    public class Error
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public static Error CreateError(string errorCode, string errorMessage) => new Error { ErrorCode = errorCode, ErrorMessage = errorMessage };
    }

    [ExcludeFromCodeCoverage]

    public class Response : IResponse
    {
        private readonly IList<IDictionary<string, string>> _messages = new List<IDictionary<string, string>>();
        public List<Error> Errors { get;  set; }
        public object? Data { get;  set; }
        public HttpStatusCode StatusCode { get;  set; }
        public string Message { get;  set; }

        public Response() => Errors = new List<Error>();

        public Response(object? data) : this() => Data = data;

        public async Task<Response> CreateSuccessResponseAsync(object? data, string message = "")
        {
            var retult = new Response
            {
                Data = data,
                Message = message,
                StatusCode = HttpStatusCode.OK
            };
            return retult;
        }

        public async Task<Response> CreateErrorResponseAsync(HttpStatusCode? statusCode = HttpStatusCode.BadRequest)
        {
            var result = new Response
            {
                Data = default,
                StatusCode = statusCode ?? HttpStatusCode.BadRequest
            };
            return result;
        }

        public async Task<Response> CreateErrorResponseAsync(object? data, HttpStatusCode? statusCode = HttpStatusCode.BadRequest)
        {
            var result = new Response
            {
                Data = data,
                StatusCode = statusCode ?? HttpStatusCode.BadRequest
            };
            return result;
        }
    }

    [ExcludeFromCodeCoverage]

    public static partial class ResponseExtensions
    {
        public static Response WithError<T>(this Response response, Expression<Func<T, string>> expression, params object[]? args)
        {
            response.Errors.Add(GetError(expression.Body as MemberExpression, args));
            return response;
        }

        public static Response WithErrorAsync<T>(this Task<Response?> response, Expression<Func<T, string>> expression, params object[]? args)
        {
            response.Result.Errors.Add(GetError(expression.Body as MemberExpression, args));
            return response.Result;
        }

        public static Response AddError(this Response response, Error error)
        {
            response.Errors.Add(error);
            return response;
        }

        public static Response AddErrors(this Response response, IEnumerable<Error> errors)
        {
            response.Errors.AddRange(errors);
            return response;
        }

        private static Error GetError(MemberExpression memberExpression, params object[]? args)
        {
            PropertyInfo propInfo = memberExpression?.Member as PropertyInfo;

            var key = propInfo?.Name;
            var value = propInfo?.GetValue(null, null).ToString();

            return Error.CreateError(key, String.Format(value, args));
        }

    }
}
