using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Moq;
using System.Text.Json;
using Ikhtibar.API.Middleware;

namespace Ikhtibar.Tests.API.Middleware
{
    /// <summary>
    /// Tests for ErrorHandlingMiddleware functionality
    /// </summary>
    public class ErrorHandlingMiddlewareTests
    {
        private readonly Mock<ILogger<ErrorHandlingMiddleware>> _mockLogger;
        private readonly ErrorHandlingMiddleware _middleware;
        private readonly HttpContext _httpContext;
        private readonly Mock<RequestDelegate> _mockNext;

        public ErrorHandlingMiddlewareTests()
        {
            _mockLogger = new Mock<ILogger<ErrorHandlingMiddleware>>();
            _mockNext = new Mock<RequestDelegate>();
            var env = new Mock<IWebHostEnvironment>();
            _middleware = new ErrorHandlingMiddleware(_mockNext.Object, _mockLogger.Object, env.Object);

            _httpContext = new DefaultHttpContext();
            _httpContext.Response.Body = new MemoryStream();
        }

    [Fact]
    public async Task InvokeAsync_WhenNoException_ShouldCallNextMiddleware()
        {
            // Arrange
            _mockNext.Setup(x => x(It.IsAny<HttpContext>()))
                    .Returns(Task.CompletedTask);

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert
            _mockNext.Verify(x => x(_httpContext), Times.Once);
            Assert.Equal(200, _httpContext.Response.StatusCode);
        }

    [Fact]
    public async Task InvokeAsync_WhenExceptionOccurs_ShouldReturn500WithProblemDetails()
        {
            // Arrange
            var exception = new InvalidOperationException("Test exception");
            _mockNext.Setup(x => x(It.IsAny<HttpContext>()))
                    .ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert
            Assert.Equal(500, _httpContext.Response.StatusCode);
            Assert.Equal("application/problem+json", _httpContext.Response.ContentType);

            // Verify response body contains ProblemDetails JSON with expected properties (don't require exact message)
            _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(_httpContext.Response.Body);
            var responseBody = await reader.ReadToEndAsync();

            Assert.False(string.IsNullOrEmpty(responseBody));
            // Ensure it's valid JSON and contains required problem fields
            var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            Assert.True(root.TryGetProperty("title", out _));
            Assert.True(root.TryGetProperty("status", out _));
            Assert.True(root.TryGetProperty("detail", out _));
        }

    [Fact]
    public async Task InvokeAsync_WhenArgumentExceptionOccurs_ShouldReturn400WithProblemDetails()
        {
            // Arrange
            var exception = new ArgumentException("Invalid argument", "paramName");
            _mockNext.Setup(x => x(It.IsAny<HttpContext>()))
                    .ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert
            Assert.Equal(400, _httpContext.Response.StatusCode);
            Assert.Equal("application/problem+json", _httpContext.Response.ContentType);
        }

    [Fact]
    public async Task InvokeAsync_WhenUnauthorizedAccessExceptionOccurs_ShouldReturn401WithProblemDetails()
        {
            // Arrange
            var exception = new UnauthorizedAccessException("Access denied");
            _mockNext.Setup(x => x(It.IsAny<HttpContext>()))
                    .ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert
            Assert.Equal(401, _httpContext.Response.StatusCode);
            Assert.Equal("application/problem+json", _httpContext.Response.ContentType);
        }

    [Fact]
    public async Task InvokeAsync_WhenKeyNotFoundExceptionOccurs_ShouldReturn404WithProblemDetails()
        {
            // Arrange
            var exception = new KeyNotFoundException("Resource not found");
            _mockNext.Setup(x => x(It.IsAny<HttpContext>()))
                    .ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert
            Assert.Equal(404, _httpContext.Response.StatusCode);
            Assert.Equal("application/problem+json", _httpContext.Response.ContentType);
        }

    [Fact]
    public async Task InvokeAsync_WhenExceptionOccurs_ShouldLogError()
        {
            // Arrange
            var exception = new Exception("Test exception");
            _mockNext.Setup(x => x(It.IsAny<HttpContext>()))
                    .ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

    [Fact]
    public async Task InvokeAsync_WhenExceptionOccurs_ShouldSetCorrelationId()
        {
            // Arrange
            var exception = new Exception("Test exception");
            _mockNext.Setup(x => x(It.IsAny<HttpContext>()))
                    .ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert
            Assert.True(_httpContext.Response.Headers.ContainsKey("X-Correlation-ID"));
            Assert.False(string.IsNullOrEmpty(_httpContext.Response.Headers["X-Correlation-ID"]));
        }

    [Fact]
    public async Task InvokeAsync_WhenExceptionOccurs_ShouldReturnValidJson()
        {
            // Arrange
            var exception = new Exception("Test exception");
            _mockNext.Setup(x => x(It.IsAny<HttpContext>()))
                    .ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert
            _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(_httpContext.Response.Body);
            var responseBody = await reader.ReadToEndAsync();

            // Verify it's valid JSON and contains ProblemDetails properties
            var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;

            Assert.True(root.TryGetProperty("title", out _));
            Assert.True(root.TryGetProperty("status", out _));
            Assert.True(root.TryGetProperty("detail", out _));
        }
    }
}

