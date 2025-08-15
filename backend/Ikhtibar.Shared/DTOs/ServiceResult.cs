using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// Generic service result wrapper for consistent API responses
/// Provides success/failure status with optional data and error information
/// </summary>
/// <typeparam name="T">Type of data returned on success</typeparam>
public class ServiceResult<T>
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Data returned on successful operation
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Error message if operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Error code for programmatic error handling
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Additional error details or validation errors
    /// </summary>
    public Dictionary<string, string[]>? ValidationErrors { get; set; }

    /// <summary>
    /// Timestamp when the result was created
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Creates a successful result with data
    /// </summary>
    /// <param name="data">Data to return</param>
    /// <returns>Successful service result</returns>
    public static ServiceResult<T> Success(T data)
    {
        return new ServiceResult<T>
        {
            IsSuccess = true,
            Data = data
        };
    }

    /// <summary>
    /// Creates a successful result without data
    /// </summary>
    /// <returns>Successful service result</returns>
    public static ServiceResult<T> Success()
    {
        return new ServiceResult<T>
        {
            IsSuccess = true
        };
    }

    /// <summary>
    /// Creates a failure result with error message
    /// </summary>
    /// <param name="errorMessage">Error description</param>
    /// <param name="errorCode">Optional error code</param>
    /// <returns>Failed service result</returns>
    public static ServiceResult<T> Failure(string errorMessage, string? errorCode = null)
    {
        return new ServiceResult<T>
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode
        };
    }

    /// <summary>
    /// Creates a failure result with validation errors
    /// </summary>
    /// <param name="validationErrors">Dictionary of field names and error messages</param>
    /// <param name="errorCode">Optional error code</param>
    /// <returns>Failed service result with validation details</returns>
    public static ServiceResult<T> ValidationFailure(Dictionary<string, string[]> validationErrors, string? errorCode = null)
    {
        return new ServiceResult<T>
        {
            IsSuccess = false,
            ErrorCode = errorCode ?? "VALIDATION_ERROR",
            ValidationErrors = validationErrors
        };
    }

    /// <summary>
    /// Creates a failure result from an exception
    /// </summary>
    /// <param name="ex">Exception that occurred</param>
    /// <param name="errorCode">Optional error code</param>
    /// <returns>Failed service result with exception details</returns>
    public static ServiceResult<T> Failure(Exception ex, string? errorCode = null)
    {
        return new ServiceResult<T>
        {
            IsSuccess = false,
            ErrorMessage = ex.Message,
            ErrorCode = errorCode ?? "EXCEPTION"
        };
    }
}

/// <summary>
/// Non-generic service result for operations that don't return data
/// </summary>
public class ServiceResult
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Error message if operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Error code for programmatic error handling
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Additional error details or validation errors
    /// </summary>
    public Dictionary<string, string[]>? ValidationErrors { get; set; }

    /// <summary>
    /// Timestamp when the result was created
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Creates a successful result
    /// </summary>
    /// <returns>Successful service result</returns>
    public static ServiceResult Success()
    {
        return new ServiceResult
        {
            IsSuccess = true
        };
    }

    /// <summary>
    /// Creates a failure result with error message
    /// </summary>
    /// <param name="errorMessage">Error description</param>
    /// <param name="errorCode">Optional error code</param>
    /// <returns>Failed service result</returns>
    public static ServiceResult Failure(string errorMessage, string? errorCode = null)
    {
        return new ServiceResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode
        };
    }

    /// <summary>
    /// Creates a failure result with validation errors
    /// </summary>
    /// <param name="validationErrors">Dictionary of field names and error messages</param>
    /// <param name="errorCode">Optional error code</param>
    /// <returns>Failed service result with validation details</returns>
    public static ServiceResult ValidationFailure(Dictionary<string, string[]> validationErrors, string? errorCode = null)
    {
        return new ServiceResult
        {
            IsSuccess = false,
            ErrorCode = errorCode ?? "VALIDATION_ERROR",
            ValidationErrors = validationErrors
        };
    }

    /// <summary>
    /// Creates a failure result from an exception
    /// </summary>
    /// <param name="ex">Exception that occurred</param>
    /// <param name="errorCode">Optional error code</param>
    /// <returns>Failed service result with exception details</returns>
    public static ServiceResult Failure(Exception ex, string? errorCode = null)
    {
        return new ServiceResult
        {
            IsSuccess = false,
            ErrorMessage = ex.Message,
            ErrorCode = errorCode ?? "EXCEPTION"
        };
    }
}
