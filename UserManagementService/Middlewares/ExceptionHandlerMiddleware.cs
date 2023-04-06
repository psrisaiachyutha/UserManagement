#region References
using Common.Exceptions;
using Common.Models;
using System.Net;
using System.Text.Json;
#endregion References

namespace UserManagementService.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        #region Declarations
        private readonly RequestDelegate _next;
        #endregion Declarations

        #region Constructor
        /// <summary>
        /// Exception Handler Middleware is initialized with its dependencies
        /// </summary>
        /// <param name="next">It represents the requestdelegate which should be called</param>
        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        #endregion Constructor

        #region Public Methods
        /// <summary>
        /// This function is used to handle the invokation of request delegate
        /// </summary>
        /// <param name="context">It represents the httpContext</param>
        /// <returns>Nothing is returned from this function</returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                response.StatusCode = error switch
                {
                    BadRequestException => (int)HttpStatusCode.BadRequest,
                    RecordNotFoundException => (int)HttpStatusCode.NotFound,
                    RecordAlreadyExistsException => (int)HttpStatusCode.Conflict,
                    InvalidCredentialsException => (int)HttpStatusCode.Unauthorized,
                    _ => (int)HttpStatusCode.InternalServerError,// unhandled error
                };

                var result = JsonSerializer.Serialize(new ApiErrorObject { ErrorMessage = error?.Message });

                await response.WriteAsync(result);
            }
        }
        #endregion Public Methods
    }
}
