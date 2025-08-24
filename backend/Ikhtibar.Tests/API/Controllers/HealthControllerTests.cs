using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Ikhtibar.API.Controllers;
using System.Text.Json;

namespace Ikhtibar.Tests.API.Controllers;

/// <summary>
/// Tests for HealthController functionality
/// </summary>
public class HealthControllerTests
{
    private readonly Mock<ILogger<HealthController>> _mockLogger;
    private readonly HealthController _controller;

    public HealthControllerTests()
    {
        _mockLogger = new Mock<ILogger<HealthController>>();
        _controller = new HealthController(_mockLogger.Object);
    }

    [Fact]
    public void Ping_ShouldReturn200Ok()
    {
        // Act
        var result = _controller.Ping();

    // Assert
    var ok = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
    Assert.Equal(200, ok.StatusCode);
    }

    [Fact]
    public void Ping_ShouldReturnSuccessResponse()
    {
        // Act
        var result = _controller.Ping();
        var okResult = result as OkObjectResult;
        var responseData = okResult?.Value;

        // Assert
        Assert.NotNull(responseData);
        
        // Use reflection to check the response structure
        var responseType = responseData?.GetType();
        var successProperty = responseType?.GetProperty("success");
        var messageProperty = responseType?.GetProperty("message");
        var dataProperty = responseType?.GetProperty("data");
        
        Assert.NotNull(successProperty);
        Assert.NotNull(messageProperty);
        Assert.NotNull(dataProperty);
    }

    [Fact]
    public void Ping_ShouldReturnHealthStatus()
    {
        // Act
        var result = _controller.Ping();
        var okResult = result as OkObjectResult;
        var responseData = okResult?.Value;

        // Assert
        var dataProperty = responseData?.GetType().GetProperty("data");
        var dataValue = dataProperty?.GetValue(responseData);
        
        Assert.NotNull(dataValue);
        
        // Check if data contains expected health information
        var dataType = dataValue?.GetType();
        var statusProperty = dataType?.GetProperty("Status");
        var versionProperty = dataType?.GetProperty("Version");
        var environmentProperty = dataType?.GetProperty("Environment");
        
        Assert.NotNull(statusProperty);
        Assert.NotNull(versionProperty);
        Assert.NotNull(environmentProperty);
    }

    [Fact]
    public void Ping_ShouldLogInformation()
    {
        // Act
        _controller.Ping();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>() ),
            Times.Once);
    }

    [Fact]
    public void Status_ShouldReturn200Ok()
    {
        // Act
        var result = _controller.Status();

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.Equal(200, okResult?.StatusCode);
    }

    [Fact]
    public void Status_ShouldReturnDetailedHealthInformation()
    {
        // Act
        var result = _controller.Status();
        var okResult = result as OkObjectResult;
        var responseData = okResult?.Value;

        // Assert
        Assert.NotNull(responseData);
        
        var dataProperty = responseData?.GetType().GetProperty("data");
        var dataValue = dataProperty?.GetValue(responseData);
        
        Assert.NotNull(dataValue);
        
        // Check if data contains detailed health information
        var dataType = dataValue?.GetType();
        var statusProperty = dataType?.GetProperty("Status");
        var versionProperty = dataType?.GetProperty("Version");
        var machineProperty = dataType?.GetProperty("Machine");
        var processorCountProperty = dataType?.GetProperty("ProcessorCount");
        var workingSetProperty = dataType?.GetProperty("WorkingSet");
        var uptimeProperty = dataType?.GetProperty("Uptime");
        
        Assert.NotNull(statusProperty);
        Assert.NotNull(versionProperty);
        Assert.NotNull(machineProperty);
        Assert.NotNull(processorCountProperty);
        Assert.NotNull(workingSetProperty);
        Assert.NotNull(uptimeProperty);
    }

    [Fact]
    public void Status_ShouldLogInformation()
    {
        // Act
        _controller.Status();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>() ),
            Times.Once);
    }

    [Fact]
    public void Status_ShouldReturnValidResponseStructure()
    {
        // Act
        var result = _controller.Status();
        var okResult = result as OkObjectResult;
        var responseData = okResult?.Value;

        // Assert
        var responseType = responseData?.GetType();
        var successProperty = responseType?.GetProperty("success");
        var messageProperty = responseType?.GetProperty("message");
        var dataProperty = responseType?.GetProperty("data");
        var timestampProperty = responseType?.GetProperty("timestamp");
        
        Assert.NotNull(successProperty);
        Assert.NotNull(messageProperty);
        Assert.NotNull(dataProperty);
        Assert.NotNull(timestampProperty);
    }

    [Fact]
    public void Status_ShouldReturnHealthyStatus()
    {
        // Act
        var result = _controller.Status();
        var okResult = result as OkObjectResult;
        var responseData = okResult?.Value;

        // Assert
        var dataProperty = responseData?.GetType().GetProperty("data");
        var dataValue = dataProperty?.GetValue(responseData);
        var statusProperty = dataValue?.GetType().GetProperty("Status");
        var statusValue = statusProperty?.GetValue(dataValue);

        Assert.Equal("Healthy", statusValue);
    }

    [Fact]
    public void Status_ShouldReturnCorrectVersion()
    {
        // Act
        var result = _controller.Status();
        var okResult = result as OkObjectResult;
        var responseData = okResult?.Value;

        // Assert
        var dataProperty = responseData?.GetType().GetProperty("data");
        var dataValue = dataProperty?.GetValue(responseData);
        var versionProperty = dataValue?.GetType().GetProperty("Version");
        var versionValue = versionProperty?.GetValue(dataValue);

        Assert.Equal("1.0.0", versionValue);
    }

    [Fact]
    public void Status_ShouldReturnSystemInformation()
    {
        // Act
        var result = _controller.Status();
    var okResult = result as OkObjectResult;
    var responseData = okResult?.Value;

        // Assert
        var dataProperty = responseData?.GetType().GetProperty("data");
        var dataValue = dataProperty?.GetValue(responseData);
        
        // Check system-specific properties
        var machineProperty = dataValue?.GetType().GetProperty("Machine");
        var processorCountProperty = dataValue?.GetType().GetProperty("ProcessorCount");
        
        Assert.NotNull(machineProperty);
        Assert.NotNull(processorCountProperty);
        
        // Verify these properties have values
        var machineValue = machineProperty?.GetValue(dataValue);
        var processorCountValue = processorCountProperty?.GetValue(dataValue);
        
        Assert.NotNull(machineValue);
        Assert.NotNull(processorCountValue);
    }
}
