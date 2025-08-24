using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Moq;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Shared.Entities;
using AutoMapper;

namespace Ikhtibar.Tests.TestHelpers;

public static class DefaultMockSetups
{
    public static void ApplyDefaults(this Mock<IUserRepository> mock, bool preSeed = false)
    {
        // Lightweight in-memory behaviour per-mock so sequence of repository calls behaves predictably in tests.
    // In-memory user store with a pre-seeded admin user that many tests expect.
    var users = new ConcurrentDictionary<int, User>();
    // Optionally pre-seed an admin user for integration tests that rely on it.
    if (preSeed)
    {
        var admin = new User { UserId = 1, Username = "admin", Email = "admin@example.com", FirstName = "System", LastName = "Administrator", IsActive = true };
        users[admin.UserId] = admin;
    }
    var nextUserId = users.Any() ? users.Keys.Max() : 0;

        mock.Setup(x => x.GetAllAsync(It.IsAny<string?>(), It.IsAny<object?>())).ReturnsAsync(() => users.Values.ToList());
        mock.Setup(x => x.GetByRoleAsync(It.IsAny<int>())).ReturnsAsync(new List<User>());
    mock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int id) => users.TryGetValue(id, out var u) ? u : null);
        mock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((string email) => users.Values.FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)));
        mock.Setup(x => x.AddAsync(It.IsAny<User>())).ReturnsAsync((User u) =>
        {
            if (u.UserId == 0)
            {
                u.UserId = Interlocked.Increment(ref nextUserId);
            }
            users[u.UserId] = u;
            return u;
        });
        mock.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User u) =>
        {
            users[u.UserId] = u;
            return u;
        });
        mock.Setup(x => x.DeleteAsync(It.IsAny<int>())).ReturnsAsync((int id) => users.TryRemove(id, out _));
        // Route EmailExistsAsync through GetByEmailAsync on the same mock so tests that setup/verify GetByEmailAsync see the invocation.
        mock.Setup(x => x.EmailExistsAsync(It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync((string email, int? excludeId) =>
        {
            var found = mock.Object.GetByEmailAsync(email).GetAwaiter().GetResult();
            if (found == null) return false;
            return excludeId.HasValue ? found.UserId != excludeId.Value : true;
        });
        mock.Setup(x => x.UserExistsAsync(It.IsAny<int>())).ReturnsAsync((int id) =>
        {
            var found = mock.Object.GetByIdAsync(id).GetAwaiter().GetResult();
            return found != null;
        });
        mock.Setup(x => x.ExistsAsync(It.IsAny<int>())).ReturnsAsync((int id) =>
        {
            var found = mock.Object.GetByIdAsync(id).GetAwaiter().GetResult();
            return found != null;
        });
    }

    public static void ApplyDefaults(this Mock<IRoleRepository> mock, bool preSeed = true)
    {
        // Lightweight in-memory role store per-mock
    var roles = new ConcurrentDictionary<int, Role>();
    // Optionally pre-seed common roles used across integration tests
    if (preSeed)
    {
        var r1 = new Role { RoleId = 1, Code = "admin", Name = "Administrator", IsSystemRole = true, IsActive = true, Description = "System administrator" };
        var r2 = new Role { RoleId = 2, Code = "user", Name = "User", IsSystemRole = false, IsActive = true, Description = "Regular user" };
        var r3 = new Role { RoleId = 3, Code = "role3", Name = "Role 3", IsSystemRole = false, IsActive = true, Description = "Role 3" };
        roles[r1.RoleId] = r1;
        roles[r2.RoleId] = r2;
        roles[r3.RoleId] = r3;
    }
    var nextRoleId = roles.Any() ? roles.Keys.Max() : 0;

        mock.Setup(x => x.GetAllAsync(It.IsAny<string?>(), It.IsAny<object?>())).ReturnsAsync(() => roles.Values.ToList());
        mock.Setup(x => x.IsRoleCodeInUseAsync(It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync((string code, int? excludeId) => roles.Values.Any(r => string.Equals(r.Code, code, StringComparison.OrdinalIgnoreCase) && (!excludeId.HasValue || r.RoleId != excludeId.Value)));
        // Route CodeExistsAsync through IsRoleCodeInUseAsync so tests that only setup IsRoleCodeInUseAsync still behave correctly
        mock.Setup(x => x.CodeExistsAsync(It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync((string code, int? excludeId) =>
        {
            try
            {
                return mock.Object.IsRoleCodeInUseAsync(code, excludeId).GetAwaiter().GetResult();
            }
            catch
            {
                // Fallback to checking the in-memory store if IsRoleCodeInUseAsync was not specially setup
                return roles.Values.Any(r => string.Equals(r.Code, code, StringComparison.OrdinalIgnoreCase) && (!excludeId.HasValue || r.RoleId != excludeId.Value));
            }
        });
        mock.Setup(x => x.GetByCodeAsync(It.IsAny<string>())).ReturnsAsync((string code) => roles.Values.FirstOrDefault(r => string.Equals(r.Code, code, StringComparison.OrdinalIgnoreCase)));
        mock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int id) => roles.TryGetValue(id, out var r) ? r : null);
        mock.Setup(x => x.ExistsAsync(It.IsAny<int>())).ReturnsAsync((int id) =>
        {
            var found = mock.Object.GetByIdAsync(id).GetAwaiter().GetResult();
            return found != null;
        });
        mock.Setup(x => x.AddAsync(It.IsAny<Role>())).ReturnsAsync((Role r) =>
        {
            if (r.RoleId == 0) r.RoleId = Interlocked.Increment(ref nextRoleId);
            roles[r.RoleId] = r;
            return r;
        });
        mock.Setup(x => x.UpdateAsync(It.IsAny<Role>())).ReturnsAsync((Role r) => { roles[r.RoleId] = r; return r; });
    }

    public static void ApplyDefaults(this Mock<IUserRoleRepository> mock, bool preSeed = true)
    {
        // In-memory user-role relations per-mock
    var relations = new ConcurrentDictionary<(int userId, int roleId), UserRole>();
    // Optionally pre-assign admin user to admin role for integration tests
    if (preSeed)
    {
        relations[(1,1)] = new UserRole { UserId = 1, RoleId = 1, AssignedAt = DateTime.UtcNow };
    }

        mock.Setup(x => x.UserHasRoleAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((int userId, int roleId) => relations.ContainsKey((userId, roleId)));
        mock.Setup(x => x.GetUserRolesAsync(It.IsAny<int>())).ReturnsAsync((int userId) => relations.Values.Where(r => r.UserId == userId).ToList());
        mock.Setup(x => x.GetRoleUsersAsync(It.IsAny<int>())).ReturnsAsync((int roleId) => relations.Values.Where(r => r.RoleId == roleId).ToList());
        mock.Setup(x => x.RemoveAllUserRolesAsync(It.IsAny<int>())).Returns((int userId) =>
        {
            var keys = relations.Keys.Where(k => k.userId == userId).ToList();
            foreach (var k in keys) relations.TryRemove(k, out _);
            return Task.CompletedTask;
        });
        mock.Setup(x => x.AssignRoleAsync(It.IsAny<int>(), It.IsAny<int>())).Returns((int userId, int roleId) =>
        {
            var ur = new UserRole { UserId = userId, RoleId = roleId, AssignedAt = DateTime.UtcNow };
            relations[(userId, roleId)] = ur;
            return Task.CompletedTask;
        });
        mock.Setup(x => x.RemoveRoleAsync(It.IsAny<int>(), It.IsAny<int>())).Returns((int userId, int roleId) =>
        {
            relations.TryRemove((userId, roleId), out _);
            return Task.CompletedTask;
        });
        mock.Setup(x => x.GetUserRoleAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((int userId, int roleId) => relations.TryGetValue((userId, roleId), out var ur) ? ur : null);
    }

    public static void ApplyDefaults(this Mock<IMapper> mock)
    {
    // caches to ensure repeated mapping of the same entity returns the same DTO instance
    var permissionCache = new System.Collections.Concurrent.ConcurrentDictionary<int, Ikhtibar.Core.DTOs.PermissionDto>();
        mock.Setup(m => m.Map<Ikhtibar.Core.DTOs.UserDto>(It.IsAny<User>()))
            .Returns((User u) => new Ikhtibar.Core.DTOs.UserDto { UserId = u.UserId, Username = u.Username ?? $"user{u.UserId}", Email = u.Email, FirstName = u.FirstName, LastName = u.LastName, Roles = new List<string>() });

        mock.Setup(m => m.Map<Ikhtibar.Core.DTOs.RoleDto>(It.IsAny<Role>()))
            .Returns((Role r) => new Ikhtibar.Core.DTOs.RoleDto { RoleId = r.RoleId, Code = r.Code, Name = r.Name, Description = r.Description });

        // Map collections into corresponding DTO lists using the element mapping above
        mock.Setup(m => m.Map<IEnumerable<Ikhtibar.Core.DTOs.UserDto>>(It.IsAny<IEnumerable<User>>()))
            .Returns((IEnumerable<User> users) => users.Select(u => new Ikhtibar.Core.DTOs.UserDto { UserId = u.UserId, Username = u.Username, Email = u.Email, FirstName = u.FirstName, LastName = u.LastName, Roles = new List<string>() }).ToList());

        mock.Setup(m => m.Map<IEnumerable<Ikhtibar.Core.DTOs.RoleDto>>(It.IsAny<IEnumerable<Role>>()))
                .Returns((IEnumerable<Role> roles) => roles.Select(r => new Ikhtibar.Core.DTOs.RoleDto { RoleId = r.RoleId, Code = r.Code, Name = r.Name, Description = r.Description, IsActive = r.IsActive }).ToList());

            // Permission mappings used by PermissionService tests - return cached DTOs so HashSet behaves as in production
            mock.Setup(m => m.Map<Ikhtibar.Core.DTOs.PermissionDto>(It.IsAny<Ikhtibar.Shared.Entities.Permission>()))
                .Returns((Ikhtibar.Shared.Entities.Permission p) => permissionCache.GetOrAdd(p.PermissionId, id => new Ikhtibar.Core.DTOs.PermissionDto { PermissionId = p.PermissionId, Code = p.Code, Name = p.Name, Description = p.Description }));

            mock.Setup(m => m.Map<IEnumerable<Ikhtibar.Core.DTOs.PermissionDto>>(It.IsAny<IEnumerable<Ikhtibar.Shared.Entities.Permission>>() ))
                    .Returns((IEnumerable<Ikhtibar.Shared.Entities.Permission> perms) =>
                    {
                        if (perms == null) return new List<Ikhtibar.Core.DTOs.PermissionDto>();
                        // Build cached DTOs and deduplicate by PermissionId to match production HashSet semantics
                        var list = perms.Select(p => permissionCache.GetOrAdd(p.PermissionId, id => new Ikhtibar.Core.DTOs.PermissionDto { PermissionId = p.PermissionId, Code = p.Code, Name = p.Name, Description = p.Description })).ToList();
                        var seen = new HashSet<int>();
                        var dedup = new List<Ikhtibar.Core.DTOs.PermissionDto>();
                        foreach (var dto in list)
                        {
                            if (dto == null) continue;
                            if (seen.Add(dto.PermissionId)) dedup.Add(dto);
                        }
                        return dedup;
                    });
    }
}
