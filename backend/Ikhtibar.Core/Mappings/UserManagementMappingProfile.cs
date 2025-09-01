using AutoMapper;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Core.DTOs;
using Ikhtibar.Shared.Models;
namespace Ikhtibar.Core.Mappings;

/// <summary>
/// AutoMapper profile for mapping between entities and DTOs
/// </summary>
public class UserManagementMappingProfile : Profile
{
    public UserManagementMappingProfile()
    {
        // User mappings
        CreateMap<Ikhtibar.Shared.Entities.User, Ikhtibar.Core.DTOs.UserDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.PreferredLanguage, opt => opt.MapFrom(src => src.PreferredLanguage ?? "en"))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.EmailVerified, opt => opt.MapFrom(src => src.EmailVerified))
            .ForMember(dest => dest.PhoneVerified, opt => opt.MapFrom(src => src.PhoneVerified))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name)))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(src => src.ModifiedAt ?? src.CreatedAt))
            .ForMember(dest => dest.LastLoginAt, opt => opt.MapFrom(src => src.LastLoginAt));

        CreateMap<Ikhtibar.Core.DTOs.CreateUserDto, Ikhtibar.Shared.Entities.User>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

        CreateMap<Ikhtibar.Core.DTOs.UpdateUserDto, Ikhtibar.Shared.Entities.User>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.Username, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Username)))
            .ForMember(dest => dest.FirstName, opt => opt.Condition(src => !string.IsNullOrEmpty(src.FirstName)))
            .ForMember(dest => dest.LastName, opt => opt.Condition(src => !string.IsNullOrEmpty(src.LastName)))
            .ForMember(dest => dest.PhoneNumber, opt => opt.Condition(src => !string.IsNullOrEmpty(src.PhoneNumber)))
            .ForMember(dest => dest.PreferredLanguage, opt => opt.Condition(src => !string.IsNullOrEmpty(src.PreferredLanguage)));

        // Role mappings
        CreateMap<Ikhtibar.Shared.Entities.Role, Ikhtibar.Core.DTOs.RoleDto>()
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId));

        CreateMap<Ikhtibar.Core.DTOs.CreateRoleDto, Ikhtibar.Shared.Entities.Role>()
            .ForMember(dest => dest.RoleId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.IsSystemRole, opt => opt.MapFrom(src => false));

        CreateMap<Ikhtibar.Core.DTOs.UpdateRoleDto, Ikhtibar.Shared.Entities.Role>()
            .ForMember(dest => dest.RoleId, opt => opt.Ignore())
            .ForMember(dest => dest.Code, opt => opt.Ignore()) // Code cannot be changed via update
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.IsSystemRole, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Name)))
            .ForMember(dest => dest.Description, opt => opt.Condition(src => src.Description != null));

        // Permission mappings
        CreateMap<Ikhtibar.Shared.Entities.Permission, Ikhtibar.Core.DTOs.PermissionDto>()
            .ForMember(dest => dest.PermissionId, opt => opt.MapFrom(src => src.PermissionId))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true)) // Default to true since there's no IsActive column
            .ForMember(dest => dest.IsSystemPermission, opt => opt.MapFrom(src => false)) // Default to false
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(src => src.ModifiedAt ?? src.CreatedAt));

        CreateMap<Ikhtibar.Core.DTOs.CreatePermissionDto, Ikhtibar.Shared.Entities.Permission>()
            .ForMember(dest => dest.PermissionId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));

        CreateMap<Ikhtibar.Core.DTOs.UpdatePermissionDto, Ikhtibar.Shared.Entities.Permission>()
            .ForMember(dest => dest.PermissionId, opt => opt.Ignore())
            .ForMember(dest => dest.Code, opt => opt.Ignore()) // Code cannot be changed via update
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Name)))
            .ForMember(dest => dest.Description, opt => opt.Condition(src => src.Description != null))
            .ForMember(dest => dest.Category, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Category)));
    }
}
