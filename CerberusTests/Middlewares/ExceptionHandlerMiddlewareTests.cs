using Cerberus.Api.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.Net;

namespace Tests.Middlewares
{
    public class ExceptionHandlerMiddlewareTests
    {
        private readonly Mock<ILogger<ExceptionHandlerMiddleware>> _logger;

        public ExceptionHandlerMiddlewareTests()
        {
            _logger = new Mock<ILogger<ExceptionHandlerMiddleware>>();
        }

        [Fact]
        public async Task Invoke_ShouldSetResponseStatusCodeTo500_WhenGeneralExceptionOccured()
        {
            // Arrange
            var nextDelegate = new RequestDelegate((HttpContext context) =>
            {
                throw new Exception();
            });

            var exceptionHandlerMiddleware = new ExceptionHandlerMiddleware(nextDelegate, _logger.Object);
            var httpContext = new DefaultHttpContext();

            // Act
            await exceptionHandlerMiddleware.Invoke(httpContext);

            // Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, httpContext.Response.StatusCode);
        }

        [Fact]
        public async Task Invoke_ShouldSetResponseMessageToException_WhenExceptionOccured()
        {
            // Arrange
            var nextDelegate = new RequestDelegate((HttpContext context) =>
            {
                throw new Exception("test");
            });

            var exceptionHandlerMiddleware = new ExceptionHandlerMiddleware(nextDelegate, _logger.Object);
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            // Act
            await exceptionHandlerMiddleware.Invoke(httpContext);

            // Assert
            var responseMessage = "";
            var expectedMessage = JsonConvert.SerializeObject(new { errorMessage = "test" });

            using (var memoryStream = httpContext.Response.Body)
            {
                memoryStream.Position = 0;
                var streamReader = new StreamReader(memoryStream);
                responseMessage = streamReader.ReadToEnd();
            }

            Assert.Equal(expectedMessage, responseMessage);
        }

        [Fact]
        public async Task Invoke_ShouldSetResponseStatusCodeTo409_WhenDbUpdateExceptionOccured()
        {
            // Arrange
            var nextDelegate = new RequestDelegate((HttpContext context) =>
            {
                throw new DbUpdateException();
            });

            var exceptionHandlerMiddleware = new ExceptionHandlerMiddleware(nextDelegate, _logger.Object);
            var httpContext = new DefaultHttpContext();

            // Act
            await exceptionHandlerMiddleware.Invoke(httpContext);

            // Assert
            Assert.Equal((int)HttpStatusCode.Conflict, httpContext.Response.StatusCode);
        }
    }
}
