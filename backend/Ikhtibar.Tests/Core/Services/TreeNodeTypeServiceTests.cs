using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ikhtibar.Core.Services.Implementations;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Shared.Entities;
using Xunit;

namespace Ikhtibar.Tests.Core.Services;

public class TreeNodeTypeServiceTests
{
    private readonly Mock<ITreeNodeTypeRepository> _repo;
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<ILogger<TreeNodeTypeService>> _logger;
    private readonly TreeNodeTypeService _service;

    public TreeNodeTypeServiceTests()
    {
        _repo = new Mock<ITreeNodeTypeRepository>();
        _mapper = new Mock<IMapper>();
        _logger = new Mock<ILogger<TreeNodeTypeService>>();
        _service = new TreeNodeTypeService(_repo.Object, _mapper.Object, _logger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllTypes()
    {
        var entities = new List<TreeNodeType>
        {
            new TreeNodeType { Id = 1, Name = "Subject", Description = "Subject category" },
            new TreeNodeType { Id = 2, Name = "Chapter", Description = "Chapter category" }
        };

        _repo.Setup(r => r.GetAllAsync(null, null)).ReturnsAsync(entities);

        var result = await _service.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_WhenFound()
    {
        var entity = new TreeNodeType { Id = 1, Name = "Subject" };
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Subject", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        _repo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((TreeNodeType?)null);

        var result = await _service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_CreatesEntity_WhenCodeUnique()
    {
        var toCreate = new TreeNodeType { Name = "New", Code = "new" };
        _repo.Setup(r => r.IsCodeUniqueAsync("new", null)).ReturnsAsync(true);
        _repo.Setup(r => r.AddAsync(It.IsAny<TreeNodeType>())).ReturnsAsync((TreeNodeType e) => { e.Id = 5; return e; });

        var result = await _service.CreateAsync(toCreate);

        Assert.NotNull(result);
        Assert.Equal(5, result.Id);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenCodeNotUnique()
    {
        var toCreate = new TreeNodeType { Name = "Existing", Code = "ex" };
        _repo.Setup(r => r.IsCodeUniqueAsync("ex", null)).ReturnsAsync(false);

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(toCreate));
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenDeleted()
    {
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new TreeNodeType { Id = 1, IsSystem = false });
        _repo.Setup(r => r.GetNodeCountByTypeAsync(1)).ReturnsAsync(0);
        _repo.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_Throws_WhenInUse()
    {
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new TreeNodeType { Id = 1, IsSystem = false });
        _repo.Setup(r => r.GetNodeCountByTypeAsync(1)).ReturnsAsync(2);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeleteAsync(1));
    }
}
