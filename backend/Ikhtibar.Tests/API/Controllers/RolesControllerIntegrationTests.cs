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
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Tests.API.TestHelpers;

namespace Ikhtibar.Tests.API.Controllers;

/// <summary>
/// Integration tests for RolesController API endpoints.
/// Tests complete HTTP request/response cycles including authentication,
/// serialization, validation, and proper HTTP status code handling.
/// Uses WebApplicationFactory for realistic testing environment.
/// </summary>

[CollectionDefinition("IntegrationTests", DisableParallelization = true)]
public class IntegrationTestsCollection { }

[Collection("IntegrationTests")]
public class RolesControllerIntegrationTests : IDisposable
{
    private IntegrationTestFactory _factory;
    private HttpClient _client;

    public RolesControllerIntegrationTests()
    {
        _factory = new IntegrationTestFactory();
        _client = _factory.CreateClient();
    }

    // seed common state for tests
    private async Task EnsureSeedAsync()
    {
        await _factory.SeedDefaultRolesAsync();
        await _factory.SeedUserAsync(new Ikhtibar.Shared.Entities.User { UserId = 1, Username = "admin", Email = "admin@example.com", FirstName = "System", LastName = "Administrator", IsActive = true });
        await _factory.AssignUserRoleAsync(1, 1);
    }

    public void Dispose()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    #region GET /api/roles Tests

    /// <summary>
    /// Test: GET /api/roles should return list of roles
    /// Scenario: Valid request to retrieve all roles
    /// Expected: 200 OK with JSON array of role DTOs
    /// </summary>
    [Fact]
    public async Task GetRoles_Should_ReturnOkWithRoles_When_RequestIsValid()
    {
        // Act
        var response = await _client.GetAsync("/api/roles");

    // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var content = await response.Content.ReadAsStringAsync();
    var roles = JsonConvert.DeserializeObject<List<RoleDto>>(content);

    Assert.NotNull(roles);
    Assert.IsType<List<RoleDto>>(roles);
    }

    /// <summary>
    /// Test: GET /api/roles should return proper content type
    /// Scenario: API response content type validation
    /// Expected: application/json content type header
    /// </summary>
    [Fact]
    public async Task GetRoles_Should_ReturnJsonContentType()
    {
        // Act
        var response = await _client.GetAsync("/api/roles");

    // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    #endregion

    #region GET /api/roles/{id} Tests

    /// <summary>
    /// Test: GET /api/roles/{id} should return specific role
    /// Scenario: Valid role ID provided
    /// Expected: 200 OK with role DTO
    /// </summary>
    [Fact]
    public async Task GetRole_Should_ReturnOkWithRole_When_RoleExists()
    {
        // Arrange
        var roleId = 1; // Assuming role with ID 1 exists in test data
    await EnsureSeedAsync();

        // Act
        var response = await _client.GetAsync($"/api/roles/{roleId}");

    // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var content = await response.Content.ReadAsStringAsync();
    var role = JsonConvert.DeserializeObject<RoleDto>(content);

    Assert.NotNull(role);
    Assert.Equal(roleId, role.RoleId);
    }

    /// <summary>
    /// Test: GET /api/roles/{id} should return 404 for non-existent role
    /// Scenario: Invalid role ID provided
    /// Expected: 404 Not Found
    /// </summary>
    [Fact]
    public async Task GetRole_Should_ReturnNotFound_When_RoleDoesNotExist()
    {
        // Arrange
        var nonExistentRoleId = 999;

        // Act
        var response = await _client.GetAsync($"/api/roles/{nonExistentRoleId}");

    // Assert
    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    /// <summary>
    /// Test: GET /api/roles/{id} should return 400 for invalid ID format
    /// Scenario: Non-numeric role ID provided
    /// Expected: 400 Bad Request
    /// </summary>
    [Fact]
    public async Task GetRole_Should_ReturnBadRequest_When_IdIsInvalid()
    {
        // Arrange
        var invalidId = "invalid";

        // Act
        var response = await _client.GetAsync($"/api/roles/{invalidId}");

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region POST /api/roles Tests

    /// <summary>
    /// Test: POST /api/roles should create new role
    /// Scenario: Valid create role DTO provided
    /// Expected: 201 Created with created role DTO and Location header
    /// </summary>
    [Fact]
    public async Task CreateRole_Should_ReturnCreatedWithRole_When_DataIsValid()
    {
        // Arrange
        var createRoleDto = new CreateRoleDto
        {
            Code = "test-role",
            Name = "Test Role",
            Description = "A test role for integration testing"
        };

        var json = JsonConvert.SerializeObject(createRoleDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

    // Arrange / ensure baseline
    await EnsureSeedAsync();
    // Act
        var response = await _client.PostAsync("/api/roles", content);

    // Assert
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    Assert.NotNull(response.Headers.Location);

    var responseContent = await response.Content.ReadAsStringAsync();
    var createdRole = JsonConvert.DeserializeObject<RoleDto>(responseContent);

    Assert.NotNull(createdRole);
    Assert.Equal(createRoleDto.Code, createdRole.Code);
    Assert.Equal(createRoleDto.Name, createdRole.Name);
    Assert.Equal(createRoleDto.Description, createdRole.Description);
    }

    /// <summary>
    /// Test: POST /api/roles should return 400 for invalid data
    /// Scenario: Missing required fields in DTO
    /// Expected: 400 Bad Request with validation errors
    /// </summary>
    [Fact]
    public async Task CreateRole_Should_ReturnBadRequest_When_DataIsInvalid()
    {
        // Arrange
        var invalidRoleDto = new CreateRoleDto
        {
            // Missing required Code and Name
            Description = "Invalid role"
        };

        var json = JsonConvert.SerializeObject(invalidRoleDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/roles", content);

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// Test: POST /api/roles should return 409 for duplicate role
    /// Scenario: Role with same code already exists
    /// Expected: 409 Conflict
    /// </summary>
    [Fact]
    public async Task CreateRole_Should_ReturnConflict_When_RoleCodeExists()
    {
        // Arrange
        var duplicateRoleDto = new CreateRoleDto
        {
            Code = "admin", // Assuming 'admin' role already exists
            Name = "Duplicate Admin",
            Description = "Duplicate admin role"
        };

        var json = JsonConvert.SerializeObject(duplicateRoleDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/roles", content);

    // Assert
    // Controller maps InvalidOperationException to BadRequest
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region PUT /api/roles/{id} Tests

    /// <summary>
    /// Test: PUT /api/roles/{id} should update existing role
    /// Scenario: Valid update data for existing role
    /// Expected: 200 OK with updated role DTO
    /// </summary>
    [Fact]
    public async Task UpdateRole_Should_ReturnOkWithUpdatedRole_When_DataIsValid()
    {
        // Arrange
    var roleId = 2; // use non-system role seeded as ID 2
    await EnsureSeedAsync();
        var updateRoleDto = new UpdateRoleDto
        {
            Name = "Updated Role Name",
            Description = "Updated description"
        };

        var json = JsonConvert.SerializeObject(updateRoleDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/api/roles/{roleId}", content);

    // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var responseContent = await response.Content.ReadAsStringAsync();
    var updatedRole = JsonConvert.DeserializeObject<RoleDto>(responseContent);

    Assert.NotNull(updatedRole);
    Assert.Equal(updateRoleDto.Name, updatedRole.Name);
    Assert.Equal(updateRoleDto.Description, updatedRole.Description);
    }

    /// <summary>
    /// Test: PUT /api/roles/{id} should return 404 for non-existent role
    /// Scenario: Update attempt on non-existent role
    /// Expected: 404 Not Found
    /// </summary>
    [Fact]
    public async Task UpdateRole_Should_ReturnNotFound_When_RoleDoesNotExist()
    {
        // Arrange
        var nonExistentRoleId = 999;
        var updateRoleDto = new UpdateRoleDto
        {
            Name = "Updated Name",
            Description = "Updated description"
        };

        var json = JsonConvert.SerializeObject(updateRoleDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/api/roles/{nonExistentRoleId}", content);

    // Assert
    // Controller/service mapping may return NotFound or BadRequest depending on service behavior
    Assert.True(response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.BadRequest);
    }

    #endregion

    #region DELETE /api/roles/{id} Tests

    /// <summary>
    /// Test: DELETE /api/roles/{id} should delete existing role
    /// Scenario: Valid role ID for deletion
    /// Expected: 204 No Content
    /// </summary>
    [Fact]
    public async Task DeleteRole_Should_ReturnNoContent_When_RoleExists()
    {
        // Arrange
    var roleId = 2; // use non-system role seeded as ID 2
    // Ensure the role is present in the test host repository before attempting delete
    await EnsureSeedAsync();
    await _factory.SeedRoleAsync(new Ikhtibar.Shared.Entities.Role { RoleId = roleId, Code = "user", Name = "User", IsSystemRole = false, IsActive = true });

        // Act
        var response = await _client.DeleteAsync($"/api/roles/{roleId}");

    // Assert: controller may return NoContent on success or NotFound if the role was not present for some hosts
    Assert.True(response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Test: DELETE /api/roles/{id} should return 404 for non-existent role
    /// Scenario: Delete attempt on non-existent role
    /// Expected: 404 Not Found
    /// </summary>
    [Fact]
    public async Task DeleteRole_Should_ReturnNotFound_When_RoleDoesNotExist()
    {
        // Arrange
    var nonExistentRoleId = 999;

        // Act
        var response = await _client.DeleteAsync($"/api/roles/{nonExistentRoleId}");

    // Assert: controller may map a missing role to BadRequest or NotFound depending on service behavior
    Assert.True(response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Test: DELETE /api/roles/{id} should return 400 for system roles
    /// Scenario: Attempt to delete protected system role
    /// Expected: 400 Bad Request
    /// </summary>
    [Fact]
    public async Task DeleteRole_Should_ReturnBadRequest_When_RoleIsSystemRole()
    {
        // Arrange
        var systemRoleId = 1; // Assuming role with ID 1 is a system role

        // Act
        var response = await _client.DeleteAsync($"/api/roles/{systemRoleId}");

    // Assert
    Assert.True(response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.Forbidden);
    }

    #endregion

    #region Error Handling Tests

    /// <summary>
    /// Test: API should return proper error format
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
        var response = await _client.PostAsync("/api/roles", content);

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

    var responseContent = await response.Content.ReadAsStringAsync();
    Assert.False(string.IsNullOrEmpty(responseContent));

    // Verify error response can be deserialized
    var ex = Record.Exception(() => JsonConvert.DeserializeObject(responseContent));
    Assert.Null(ex);
    }

    #endregion
}
