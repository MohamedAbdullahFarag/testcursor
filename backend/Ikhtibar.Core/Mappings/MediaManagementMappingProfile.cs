using AutoMapper;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Core.Mappings;

/// <summary>
/// AutoMapper profile for Media Management entities and DTOs
/// </summary>
public class MediaManagementMappingProfile : Profile
{
    public MediaManagementMappingProfile()
    {
        // Basic MediaFile to MediaFileDto mapping
        CreateMap<MediaFile, MediaFileDto>()
            .ForMember(dest => dest.UploadedByUserId, opt => opt.MapFrom(src => src.UploadedBy))
            .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(src => src.ModifiedAt))
            .ReverseMap();

        // CreateMediaFileDto to MediaFile mapping
        CreateMap<CreateMediaFileDto, MediaFile>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ReverseMap();

        // UpdateMediaFileDto to MediaFile mapping
        CreateMap<UpdateMediaFileDto, MediaFile>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.OriginalFileName, opt => opt.Ignore())
            .ForMember(dest => dest.StorageFileName, opt => opt.Ignore())
            .ForMember(dest => dest.FileSizeBytes, opt => opt.Ignore())
            .ForMember(dest => dest.ContentType, opt => opt.Ignore())
            .ForMember(dest => dest.StoragePath, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ReverseMap();
    }
}
