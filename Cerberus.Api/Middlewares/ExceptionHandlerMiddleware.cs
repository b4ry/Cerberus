using Cerberus.Api.Constants;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;

namespace Cerberus.Api.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex)
            {
                if(ex is DbUpdateException)
                {
                    if (ex.InnerException is SqliteException sqliteException)
                    {
                        if (sqliteException!.SqliteExtendedErrorCode == (int)SqliteErrors.ConstraintPrimaryKey)
                        {
                            context.Response.StatusCode = StatusCodes.Status409Conflict;
                        }
                    }
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
                
                var response = JsonConvert.SerializeObject(new { errorMessage = ex.Message });
                await context.Response.WriteAsync(response);

                _logger.LogError(exception: ex, message: ex.Message, ex.Data);
            }
        }
    }
}
