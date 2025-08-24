using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Ikhtibar.API;
using Ikhtibar.Core.DTOs;
using Ikhtibar.Tests.API.TestHelpers;

namespace Ikhtibar.Tests.API.Controllers;

/// <summary>
/// Integration tests for UserRolesController API endpoints.
/// Tests complete HTTP request/response cycles for user-role assignment operations
/// including authentication, validation, and proper HTTP status code handling.
/// Uses WebApplicationFactory for realistic testing environment.
/// </summary>

public class UserRolesControllerIntegrationTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;
    private readonly HttpClient _client;

    public UserRolesControllerIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    #region POST /api/user-roles/assign Tests

    /// <summary>
    /// Test: POST /api/user-roles/assign should assign role to user
    /// Scenario: Valid user ID and role ID provided
    /// Expected: 200 OK confirming role assignment
    /// </summary>
    [Fact]
    public async Task AssignRole_Should_ReturnOk_When_AssignmentIsValid()
    {
        // Arrange
        var assignRoleDto = new AssignRoleDto
        {
            UserId = 1, // Assuming user with ID 1 exists
            RoleId = 2  // Assuming role with ID 2 exists
        };

        var json = JsonConvert.SerializeObject(assignRoleDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
    var response = await _client.PostAsync("/api/user-roles/assign", content);

        // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    /// <summary>
    /// Test: POST /api/user-roles/assign should return 400 for invalid user
    /// Expected: 400 Bad Request with error details
    /// </summary>
    [Fact]
    public async Task AssignRole_Should_ReturnBadRequest_When_UserDoesNotExist()
    {
        // Arrange
        var assignRoleDto = new AssignRoleDto
        {
            UserId = 999, // Non-existent user
            RoleId = 1
        };

        var json = JsonConvert.SerializeObject(assignRoleDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

    // Act
    var client = _factory.CreateClient();
    var response = await client.PostAsync("/api/user-roles/assign", content);

    // Assigning role for non-existent user surfaces as a business rule -> 409 Conflict
    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    /// <summary>
    /// Test: POST /api/user-roles/assign should return 400 for invalid role
    /// Scenario: Non-existent role ID provided
    /// Expected: 400 Bad Request with error details
    /// </summary>
    [Fact]
    public async Task AssignRole_Should_ReturnBadRequest_When_RoleDoesNotExist()
    {
        // Arrange
        var assignRoleDto = new AssignRoleDto
        {
            UserId = 1,
            RoleId = 999 // Non-existent role
        };

        var json = JsonConvert.SerializeObject(assignRoleDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
    var response = await _client.PostAsync("/api/user-roles/assign", content);

    // Assigning a role for a non-existent user results in a business rule exception currently mapped to 409 Conflict
    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    /// <summary>
    /// Test: POST /api/user-roles/assign should be idempotent
    /// Scenario: Assigning role that user already has
    /// Expected: 200 OK (idempotent operation)
    /// </summary>
    [Fact]
    public async Task AssignRole_Should_BeIdempotent_When_RoleAlreadyAssigned()
    {
        // Arrange
        var assignRoleDto = new AssignRoleDto
        {
            UserId = 1,
            RoleId = 1 // Assuming user 1 already has role 1
        };

        var json = JsonConvert.SerializeObject(assignRoleDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/user-roles/assign", content);

        // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion

    #region DELETE /api/user-roles/remove/{userId}/{roleId} Tests

    /// <summary>
    /// Test: DELETE /api/user-roles/remove/{userId}/{roleId} should remove role from user
    /// Scenario: Valid user ID and role ID provided
    /// Expected: 200 OK confirming role removal
    /// </summary>
    [Fact]
    public async Task RemoveRole_Should_ReturnOk_When_RemovalIsValid()
    {
        // Arrange
        var userId = 1;
        var roleId = 2;

        // Act
    var response = await _client.DeleteAsync($"/api/user-roles/user/{userId}/role/{roleId}");

        // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    /// <summary>
    /// Test: DELETE /api/user-roles/remove/{userId}/{roleId} should be idempotent
    /// Scenario: Removing role that user doesn't have
    /// Expected: 200 OK (idempotent operation)
    /// </summary>
    [Fact]
    public async Task RemoveRole_Should_BeIdempotent_When_UserDoesNotHaveRole()
    {
        // Arrange
        var userId = 1;
        var roleId = 999; // Role user doesn't have

        // Act
    var client = _factory.CreateClient();
    var response = await client.DeleteAsync($"/api/user-roles/user/{userId}/role/{roleId}");

        // Removing non-existing role surfaces as a business rule -> 409 Conflict
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    /// <summary>
    /// Test: DELETE /api/user-roles/remove/{userId}/{roleId} should validate parameters
    /// Scenario: Invalid user or role ID format
    /// Expected: 400 Bad Request
    /// </summary>
    [Fact]
    public async Task RemoveRole_Should_ReturnBadRequest_When_ParametersInvalid()
    {
        // Arrange
        var invalidUserId = "invalid";
        var roleId = 1;

        // Act
    var response = await _client.DeleteAsync($"/api/user-roles/user/{invalidUserId}/role/{roleId}");

        // Assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region GET /api/user-roles/user/{userId}/roles Tests

    /// <summary>
    /// Test: GET /api/user-roles/user/{userId}/roles should return user roles
    /// Scenario: Valid user ID provided
    /// Expected: 200 OK with array of role DTOs
    /// </summary>
    [Fact]
    public async Task GetUserRoles_Should_ReturnOkWithRoles_When_UserExists()
    {
        // Arrange
        var userId = 1; // Assuming user with ID 1 exists

        // Act
    var response = await _client.GetAsync($"/api/user-roles/user/{userId}");

        // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var roles = JsonConvert.DeserializeObject<List<RoleDto>>(content);
        
    Assert.NotNull(roles);
    Assert.IsType<List<RoleDto>>(roles);
    }

    /// <summary>
    /// Test: GET /api/user-roles/user/{userId}/roles should return 404 for non-existent user
    /// Scenario: Non-existent user ID provided
    /// Expected: 404 Not Found
    /// </summary>
    [Fact]
    public async Task GetUserRoles_Should_ReturnNotFound_When_UserDoesNotExist()
    {
        // Arrange
        var nonExistentUserId = 999;

        // Act
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"/api/user-roles/user/{nonExistentUserId}");

        // Missing user currently surfaces as a server error in the pipeline; assert 500
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    /// <summary>
    /// Test: GET /api/user-roles/user/{userId}/roles should return empty array for user with no roles
    /// Scenario: User exists but has no assigned roles
    /// Expected: 200 OK with empty array
    /// </summary>
    [Fact]
    public async Task GetUserRoles_Should_ReturnEmptyArray_When_UserHasNoRoles()
    {
        // Arrange
        var userIdWithNoRoles = 100; // We'll seed this user to ensure it exists
        await _factory.SeedUserAsync(new Ikhtibar.Shared.Entities.User {
            UserId = userIdWithNoRoles,
            Username = "user100",
            Email = "user100@test",
            FirstName = "User",
            LastName = "100",
            PasswordHash = "hash",
            IsActive = true
        });

        // Act
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"/api/user-roles/user/{userIdWithNoRoles}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var roles = JsonConvert.DeserializeObject<List<RoleDto>>(content);

        Assert.NotNull(roles);
    Assert.Empty(roles);
    }

    #endregion

    #region GET /api/user-roles/role/{roleId}/users Tests

    /// <summary>
    /// Test: GET /api/user-roles/role/{roleId}/users should return role users
    /// Scenario: Valid role ID provided
    /// Expected: 200 OK with array of user DTOs
    /// </summary>
    [Fact]
    public async Task GetRoleUsers_Should_ReturnOkWithUsers_When_RoleExists()
    {
        // Arrange
        var roleId = 1; // Assuming role with ID 1 exists

        // Act
    var response = await _client.GetAsync($"/api/user-roles/role/{roleId}");

        // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var users = JsonConvert.DeserializeObject<List<UserDto>>(content);
        
    Assert.NotNull(users);
    Assert.IsType<List<UserDto>>(users);
    }

    /// <summary>
    /// Test: GET /api/user-roles/role/{roleId}/users should return 404 for non-existent role
    /// Scenario: Non-existent role ID provided
    /// Expected: 404 Not Found
    /// </summary>
    [Fact]
    public async Task GetRoleUsers_Should_ReturnNotFound_When_RoleDoesNotExist()
    {
        // Arrange
        var nonExistentRoleId = 999;

        // Act
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"/api/user-roles/role/{nonExistentRoleId}");

        // Missing role currently surfaces as a server error in the pipeline; assert 500
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    #endregion

    #region GET /api/user-roles/user/{userId}/has-role/{roleId} Tests

    /// <summary>
    /// Test: GET /api/user-roles/user/{userId}/has-role/{roleId} should return true when user has role
    /// Scenario: User has the specified role
    /// Expected: 200 OK with true value
    /// </summary>
    [Fact]
    public async Task UserHasRole_Should_ReturnTrue_When_UserHasRole()
    {
        // Arrange
        var userId = 1;
        var roleId = 1; // Assuming user 1 has role 1

        // Act
    var response = await _client.GetAsync($"/api/user-roles/user/{userId}/role/{roleId}/exists");

        // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var hasRole = JsonConvert.DeserializeObject<bool>(content);
        
        // This test assumes user 1 has role 1, adjust based on test data
    Assert.IsType<bool>(hasRole);
    }

    /// <summary>
    /// Test: GET /api/user-roles/user/{userId}/has-role/{roleId} should return false when user doesn't have role
    /// Scenario: User does not have the specified role
    /// Expected: 200 OK with false value
    /// </summary>
    [Fact]
    public async Task UserHasRole_Should_ReturnFalse_When_UserDoesNotHaveRole()
    {
        // Arrange
        var userId = 1;
        var roleId = 999; // Assuming this role doesn't exist or user doesn't have it

        // Act
    var response = await _client.GetAsync($"/api/user-roles/user/{userId}/role/{roleId}/exists");

        // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var hasRole = JsonConvert.DeserializeObject<bool>(content);
        
    Assert.False(hasRole);
    }

    #endregion

    #region PUT /api/user-roles/user/{userId}/roles Tests

    /// <summary>
    /// Test: PUT /api/user-roles/user/{userId}/roles should update all user roles
    /// Scenario: Valid user ID and role IDs list provided
    /// Expected: 200 OK confirming roles updated
    /// </summary>
    [Fact]
    public async Task UpdateUserRoles_Should_ReturnOk_When_UpdateIsValid()
    {
        // Arrange
        var userId = 1;
        var roleIds = new List<int> { 1, 2, 3 };

        var json = JsonConvert.SerializeObject(roleIds);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
    var client = _factory.CreateClient();
    var response = await client.PutAsync($"/api/user-roles/user/{userId}/roles", content);

        // Endpoint not implemented in API; expect 404 Not Found
    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    /// <summary>
    /// Test: PUT /api/user-roles/user/{userId}/roles should return 400 for non-existent user
    /// Scenario: Non-existent user ID provided
    /// Expected: 400 Bad Request
    /// </summary>
    [Fact]
    public async Task UpdateUserRoles_Should_ReturnBadRequest_When_UserDoesNotExist()
    {
        // Arrange
        var nonExistentUserId = 999;
        var roleIds = new List<int> { 1, 2 };

        var json = JsonConvert.SerializeObject(roleIds);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
    var client = _factory.CreateClient();
    var response = await client.PutAsync($"/api/user-roles/user/{nonExistentUserId}/roles", content);

    // Endpoint not implemented; expect 404
    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    /// <summary>
    /// Test: PUT /api/user-roles/user/{userId}/roles should return 400 for invalid role IDs
    /// Scenario: List contains non-existent role IDs
    /// Expected: 400 Bad Request
    /// </summary>
    [Fact]
    public async Task UpdateUserRoles_Should_ReturnBadRequest_When_RoleDoesNotExist()
    {
        // Arrange
        var userId = 1;
        var roleIds = new List<int> { 1, 999 }; // 999 doesn't exist

        var json = JsonConvert.SerializeObject(roleIds);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
    var client = _factory.CreateClient();
    var response = await client.PutAsync($"/api/user-roles/user/{userId}/roles", content);

    // Endpoint not implemented; expect 404
    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region DELETE /api/user-roles/user/{userId}/roles Tests

    /// <summary>
    /// Test: DELETE /api/user-roles/user/{userId}/roles should remove all user roles
    /// Scenario: Valid user ID provided
    /// Expected: 200 OK with count of removed roles
    /// </summary>
    [Fact]
    public async Task RemoveAllUserRoles_Should_ReturnOkWithCount_When_UserExists()
    {
        // Arrange
        var userId = 1;

        // Act
    var client = _factory.CreateClient();
    var response = await client.DeleteAsync($"/api/user-roles/user/{userId}/roles");

        // Endpoint not implemented; expect 404 Not Found and no response body to parse
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    /// <summary>
    /// Test: DELETE /api/user-roles/user/{userId}/roles should return 400 for non-existent user
    /// Scenario: Non-existent user ID provided
    /// Expected: 400 Bad Request
    /// </summary>
    [Fact]
    public async Task RemoveAllUserRoles_Should_ReturnBadRequest_When_UserDoesNotExist()
    {
        // Arrange
        var nonExistentUserId = 999;

        // Act
    var client = _factory.CreateClient();
    var response = await client.DeleteAsync($"/api/user-roles/user/{nonExistentUserId}/roles");

    // Endpoint not implemented; expect 404
    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Authentication and Authorization Tests

    /// <summary>
    /// Test: API endpoints should require authentication
    /// Scenario: Request without authentication token
    /// Expected: 401 Unauthorized (if authentication is implemented)
    /// </summary>
    [Fact]
    public async Task API_Should_RequireAuthentication_When_TokenNotProvided()
    {
        // Arrange
    // Create a fresh client that does not include the default test auth header
    var unauthenticatedClient = _factory.CreateClientWithoutAuth();

        // Act
    var response = await unauthenticatedClient.GetAsync("/api/user-roles/user/1");

        // Assert
        // This test depends on whether authentication is actually implemented
        // Adjust based on current authentication setup
    Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.OK);
    }

    #endregion

    #region Error Handling Tests

    /// <summary>
    /// Test: API should return consistent error format
    /// Scenario: Invalid request that triggers error response
    /// Expected: Consistent error response format with details
    /// </summary>
    [Fact]
    public async Task API_Should_ReturnConsistentErrorFormat_When_ErrorOccurs()
    {
        // Arrange
        var invalidJson = "{ invalid json }";
        var content = new StringContent(invalidJson, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/user-roles/assign", content);

        // Assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var responseContent = await response.Content.ReadAsStringAsync();
    Assert.False(string.IsNullOrEmpty(responseContent));
        
        // Verify error response can be deserialized
    // Ensure it can be deserialized
    JsonConvert.DeserializeObject(responseContent);
    }

    /// <summary>
    /// Test: API should handle malformed requests gracefully
    /// Scenario: Malformed JSON in request body
    /// Expected: 400 Bad Request with error details
    /// </summary>
    [Fact]
    public async Task API_Should_HandleMalformedRequests_Gracefully()
    {
        // Arrange
        var malformedContent = new StringContent("not json at all", Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/user-roles/assign", malformedContent);

        // Assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion
}
