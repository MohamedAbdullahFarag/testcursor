using AutoMapper;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;

using Ikhtibar.Shared.Models;
namespace Ikhtibar.Core.Mappings;

/// <summary>
/// AutoMapper profile for mapping between tree entities and DTOs
/// </summary>
public class TreeManagementMappingProfile : Profile
{
    public TreeManagementMappingProfile()
    {
        // TreeNodeType mappings (simple entity without BaseEntity inheritance)
        CreateMap<TreeNodeType, TreeNodeTypeDto>();

        CreateMap<CreateTreeNodeTypeDto, TreeNodeType>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TreeNodes, opt => opt.Ignore());

        CreateMap<UpdateTreeNodeTypeDto, TreeNodeType>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TreeNodes, opt => opt.Ignore());

        // TreeNode mappings
        CreateMap<TreeNode, TreeNodeDto>()
            .ForMember(dest => dest.TreeNodeTypeName, opt => opt.Ignore()); // Will be set by service if needed

        CreateMap<TreeNode, TreeNodeDetailDto>()
            .ForMember(dest => dest.TreeNodeTypeName, opt => opt.Ignore()) // Will be set by service if needed
            .ForMember(dest => dest.Children, opt => opt.Ignore()) // Will be loaded separately
            .ForMember(dest => dest.CurriculumAlignments, opt => opt.Ignore()); // Will be loaded separately

        CreateMap<CreateTreeNodeDto, TreeNode>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Path, opt => opt.Ignore()) // Will be calculated by service
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));

        CreateMap<UpdateTreeNodeDto, TreeNode>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Path, opt => opt.Ignore()) // Path should not be updated directly
            .ForMember(dest => dest.ParentId, opt => opt.Ignore()) // Use move operation for parent changes
            .ForMember(dest => dest.OrderIndex, opt => opt.Ignore()) // Use reorder operation for order changes
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        // CurriculumAlignment mappings
        CreateMap<CurriculumAlignment, CurriculumAlignmentDto>();

        CreateMap<CreateCurriculumAlignmentDto, CurriculumAlignment>()
            .ForMember(dest => dest.CurriculumAlignmentId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));

        CreateMap<UpdateCurriculumAlignmentDto, CurriculumAlignment>()
            .ForMember(dest => dest.CurriculumAlignmentId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        // TreeNodeStatistics mapping (Repository to DTO)
        CreateMap<TreeNodeStatistics, TreeNodeStatistics>();
    }
}
