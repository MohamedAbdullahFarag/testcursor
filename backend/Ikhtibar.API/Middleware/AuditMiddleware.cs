using Ikhtibar.Core.Services;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Enums;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using System.Text;

namespace Ikhtibar.API.Middleware;

/// <summary>
/// Middleware for automatically logging API requests and responses
/// </summary>
public class AuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditMiddleware> _logger;
    
    // These endpoint paths will not be audited to prevent excessive logging
    private static readonly HashSet<string> _excludedPaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "/api/health",
        "/api/health/ping",
        "/metrics",
        "/swagger",
    };
    
    // These endpoint paths will have their request/response bodies excluded from the audit log
    private static readonly HashSet<string> _sensitiveDataPaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "/api/auth/login",
        "/api/auth/register",
        "/api/users/password",
        "/api/auth/refresh-token",
    };
    
    /// <summary>
    /// Constructor for AuditMiddleware
    /// </summary>
    /// <param name="next">Request delegate</param>
    /// <param name="logger">Logger instance</param>
    public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    /// <summary>
    /// Invokes the middleware
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="auditService">Audit service</param>
    /// <returns>Task</returns>
    public async Task InvokeAsync(HttpContext context, IAuditService auditService)
    {
        // Skip logging for excluded paths
        if (ShouldSkipLogging(context.Request.Path))
        {
            await _next(context);
            return;
        }
        
        // Get basic request information
        var requestPath = context.Request.Path.Value ?? "";
        var method = context.Request.Method;
        var requestTime = DateTime.UtcNow;
        var requestId = context.TraceIdentifier;
        
        // Determine if this is a sensitive request that should have its body content masked
        bool isSensitiveRequest = IsSensitiveDataPath(context.Request.Path);
        
        // Capture request body
        string? requestBody = null;
        if (context.Request.ContentLength > 0 && context.Request.Body.CanRead)
        {
            context.Request.EnableBuffering();
            
            using (var reader = new StreamReader(
                context.Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true))
            {
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
                
                if (isSensitiveRequest && !string.IsNullOrEmpty(requestBody))
                {
                    requestBody = "*** Sensitive Data Redacted ***";
                }
            }
        }
        
        // Capture the original response body stream
        var originalBodyStream = context.Response.Body;
        
        // Create a new memory stream to capture the response
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        
        try
        {
            // Continue processing the request
            await _next(context);
            
            // Capture response details
            var statusCode = context.Response.StatusCode;
            var responseTime = DateTime.UtcNow;
            var duration = responseTime - requestTime;
            
            string? responseContent = null;
            
            // Only read response body for content types we care about
            if (context.Response.ContentType != null && 
                (context.Response.ContentType.Contains("application/json") || 
                 context.Response.ContentType.Contains("text/plain")))
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                responseContent = await new StreamReader(responseBody).ReadToEndAsync();
                responseBody.Seek(0, SeekOrigin.Begin);
                
                if (isSensitiveRequest && !string.IsNullOrEmpty(responseContent))
                {
                    responseContent = "*** Sensitive Data Redacted ***";
                }
            }
            
            // Determine if this is a security-relevant request
            bool isSecurityRelevant = IsSecurityRelevantRequest(requestPath, method);
            
            // Get user ID if authenticated
            int? userId = null;
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int id))
                {
                    userId = id;
                }
            }
            
            // Build API request details for audit
            var requestDetails = new
            {
                Method = method,
                Path = requestPath,
                QueryString = context.Request.QueryString.ToString(),
                RequestBody = requestBody,
                Headers = GetSafeHeaders(context.Request.Headers),
                IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                UserAgent = context.Request.Headers["User-Agent"].ToString()
            };
            
            // Build API response details for audit
            var responseDetails = new
            {
                StatusCode = statusCode,
                ContentType = context.Response.ContentType,
                ResponseBody = responseContent,
                Duration = duration.TotalMilliseconds
            };
            
            // Create audit log entries based on the request type
            if (isSecurityRelevant)
            {
                // Log security-relevant API calls with higher severity
                await auditService.LogSecurityEventAsync(
                    context.User?.Identity?.Name ?? "Unknown",
                    $"API_{method}",
                    $"{method} {requestPath} - Status {statusCode}",
                    AuditSeverity.Medium);
            }
            else
            {
                // Log standard API calls
                await auditService.LogSystemActionAsync(
                    $"API_{method}",
                    $"{method} {requestPath} - Status {statusCode}",
                    "API");
            }
        }
        catch (Exception ex)
        {
            // Log exception and rethrow
            _logger.LogError(ex, "Error in audit middleware");
            
            // Try to log the error in the audit log
            try
            {
                await auditService.LogSystemActionAsync(
                    "MIDDLEWARE_ERROR",
                    $"Error in audit middleware: {ex.Message}",
                    "Middleware");
            }
            catch
            {
                // If logging fails, we've already logged to the regular logger, so continue
            }
            
            throw;
        }
        finally
        {
            // Copy the response to the original stream and restore it
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
    }
    
    /// <summary>
    /// Determines if a request path should be excluded from logging
    /// </summary>
    /// <param name="path">Request path</param>
    /// <returns>True if logging should be skipped</returns>
    private bool ShouldSkipLogging(PathString path)
    {
        var pathValue = path.Value ?? "";
        
        return _excludedPaths.Any(excludedPath => pathValue.StartsWith(excludedPath, StringComparison.OrdinalIgnoreCase));
    }
    
    /// <summary>
    /// Determines if a request path contains sensitive data
    /// </summary>
    /// <param name="path">Request path</param>
    /// <returns>True if the path is sensitive</returns>
    private bool IsSensitiveDataPath(PathString path)
    {
        var pathValue = path.Value ?? "";
        
        return _sensitiveDataPaths.Any(sensitivePath => pathValue.StartsWith(sensitivePath, StringComparison.OrdinalIgnoreCase));
    }
    
    /// <summary>
    /// Determines if a request is security-relevant
    /// </summary>
    /// <param name="path">Request path</param>
    /// <param name="method">HTTP method</param>
    /// <returns>True if the request is security relevant</returns>
    private bool IsSecurityRelevantRequest(string path, string method)
    {
        // Authentication and authorization endpoints
        if (path.Contains("/api/auth/", StringComparison.OrdinalIgnoreCase))
            return true;
            
        // User management endpoints
        if (path.Contains("/api/users/", StringComparison.OrdinalIgnoreCase) && 
            (method == "POST" || method == "PUT" || method == "DELETE"))
            return true;
            
        // Role management endpoints
        if (path.Contains("/api/roles/", StringComparison.OrdinalIgnoreCase))
            return true;
            
        // Permission management endpoints
        if (path.Contains("/api/permissions/", StringComparison.OrdinalIgnoreCase))
            return true;
            
        // Settings endpoints
        if (path.Contains("/api/settings/", StringComparison.OrdinalIgnoreCase) &&
            (method == "POST" || method == "PUT"))
            return true;
            
        return false;
    }
    
    /// <summary>
    /// Gets safe headers from the request (excluding sensitive headers)
    /// </summary>
    /// <param name="headers">Request headers</param>
    /// <returns>Dictionary of safe headers</returns>
    private Dictionary<string, string> GetSafeHeaders(IHeaderDictionary headers)
    {
        var result = new Dictionary<string, string>();
        
        foreach (var header in headers)
        {
            // Skip sensitive headers
            if (header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase) ||
                header.Key.Equals("Cookie", StringComparison.OrdinalIgnoreCase) ||
                header.Key.Contains("Api-Key", StringComparison.OrdinalIgnoreCase))
            {
                result[header.Key] = "*** Redacted ***";
            }
            else
            {
                result[header.Key] = header.Value.ToString();
            }
        }
        
        return result;
    }
}
