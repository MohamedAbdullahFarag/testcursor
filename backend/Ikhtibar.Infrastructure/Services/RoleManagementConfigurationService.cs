/**
 * Role Management Configuration Verification Service
 * Verifies that all required services are properly configured and available
 */

using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Dapper;

namespace Ikhtibar.Infrastructure.Services;

/// <summary>
/// Service to verify role management configuration is complete
/// </summary>
public interface IRoleManagementConfigurationService
{
    /// <summary>
    /// Verifies all role management services are properly registered
    /// </summary>
    Task<ConfigurationValidationResult> ValidateConfigurationAsync();
    
    /// <summary>
    /// Seeds default roles if not already present
    /// </summary>
    Task<bool> EnsureDefaultRolesAsync();
}

/// <summary>
/// Implementation of role management configuration verification
/// </summary>
public class RoleManagementConfigurationService : IRoleManagementConfigurationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RoleManagementConfigurationService> _logger;
    private readonly IRoleService _roleService;
    private readonly IDbConnectionFactory _connectionFactory;

    public RoleManagementConfigurationService(
        IServiceProvider serviceProvider,
        ILogger<RoleManagementConfigurationService> logger,
        IRoleService roleService,
        IDbConnectionFactory connectionFactory)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _roleService = roleService;
        _connectionFactory = connectionFactory;
    }

    /// <summary>
    /// Validates that all required services are properly configured
    /// </summary>
    public async Task<ConfigurationValidationResult> ValidateConfigurationAsync()
    {
        var result = new ConfigurationValidationResult();
        
        try
        {
            _logger.LogInformation("Starting role management configuration validation");

            // Check service registrations
            await ValidateServiceRegistrations(result);
            
            // Check database connectivity
            await ValidateDatabaseConnectivity(result);
            
            // Check default roles existence
            await ValidateDefaultRoles(result);
            
            _logger.LogInformation("Role management configuration validation completed. Status: {Status}", 
                result.IsValid ? "Valid" : "Invalid");
                
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during configuration validation");
            result.AddError("Configuration validation failed", ex.Message);
            return result;
        }
    }

    /// <summary>
    /// Ensures default system roles are present in the database
    /// </summary>
    public async Task<bool> EnsureDefaultRolesAsync()
    {
        try
        {
            _logger.LogInformation("Ensuring default roles are present");
            
            var seededCount = await _roleService.SeedDefaultRolesAsync();
            
            if (seededCount > 0)
            {
                _logger.LogInformation("Seeded {Count} default roles", seededCount);
            }
            else
            {
                _logger.LogInformation("Default roles already exist");
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to ensure default roles");
            return false;
        }
    }

    private Task ValidateServiceRegistrations(ConfigurationValidationResult result)
    {
        var requiredServices = new[]
        {
            typeof(IRoleService),
            typeof(IUserRoleService), 
            typeof(IRoleRepository),
            typeof(IUserRoleRepository),
            typeof(IDbConnectionFactory)
        };

        foreach (var serviceType in requiredServices)
        {
            var service = _serviceProvider.GetService(serviceType);
            if (service == null)
            {
                result.AddError("Service Registration", $"Service {serviceType.Name} is not registered");
            }
            else
            {
                result.AddSuccess("Service Registration", $"Service {serviceType.Name} is properly registered");
            }
        }
        
        return Task.CompletedTask;
    }

    private async Task ValidateDatabaseConnectivity(ConfigurationValidationResult result)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            
            // Test basic connectivity
            var testQuery = "SELECT 1";
            await connection.QueryFirstAsync<int>(testQuery);
            
            result.AddSuccess("Database Connectivity", "Database connection successful");
            
            // Check if roles table exists
            var tableExistsQuery = @"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_NAME = 'Roles'";
                
            var tableExists = await connection.QueryFirstAsync<int>(tableExistsQuery) > 0;
            
            if (tableExists)
            {
                result.AddSuccess("Database Schema", "Roles table exists");
            }
            else
            {
                result.AddError("Database Schema", "Roles table does not exist");
            }
        }
        catch (Exception ex)
        {
            result.AddError("Database Connectivity", $"Database connection failed: {ex.Message}");
        }
    }

    private async Task ValidateDefaultRoles(ConfigurationValidationResult result)
    {
        try
        {
            var systemRoles = await _roleService.GetSystemRolesAsync();
            var systemRolesList = systemRoles.ToList();
            
            var expectedSystemRoles = new[]
            {
                "system-admin",
                "exam-manager", 
                "reviewer",
                "creator",
                "supervisor",
                "student",
                "grader"
            };

            foreach (var expectedRole in expectedSystemRoles)
            {
                if (systemRolesList.Any(r => r.Code == expectedRole))
                {
                    result.AddSuccess("Default Roles", $"System role '{expectedRole}' exists");
                }
                else
                {
                    result.AddWarning("Default Roles", $"System role '{expectedRole}' is missing");
                }
            }
        }
        catch (Exception ex)
        {
            result.AddError("Default Roles", $"Failed to validate default roles: {ex.Message}");
        }
    }
}

/// <summary>
/// Result of configuration validation
/// </summary>
public class ConfigurationValidationResult
{
    public bool IsValid => !Errors.Any();
    public List<ValidationItem> Successes { get; } = new();
    public List<ValidationItem> Warnings { get; } = new();
    public List<ValidationItem> Errors { get; } = new();

    public void AddSuccess(string category, string message)
    {
        Successes.Add(new ValidationItem(category, message));
    }

    public void AddWarning(string category, string message)
    {
        Warnings.Add(new ValidationItem(category, message));
    }

    public void AddError(string category, string message)
    {
        Errors.Add(new ValidationItem(category, message));
    }
}

/// <summary>
/// Individual validation result item
/// </summary>
public record ValidationItem(string Category, string Message);
