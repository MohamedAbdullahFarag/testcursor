using AutoMapper;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Core.Mappings;

/// <summary>
/// AutoMapper profile for mapping between audit entities and DTOs
/// </summary>
public class AuditMappingProfile : Profile
{
    public AuditMappingProfile()
    {
        // Simple AuditLog to AuditLogDto mapping
        CreateMap<AuditLog, AuditLogDto>()
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.CreatedAt));

        // Simple AuditLogEntry to AuditLog mapping
        CreateMap<AuditLogEntry, AuditLog>()
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));
    }
}
