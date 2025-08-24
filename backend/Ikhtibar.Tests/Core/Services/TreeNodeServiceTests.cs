using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Ikhtibar.Core.Services.Implementations;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Shared.Entities;
using Xunit;

namespace Ikhtibar.Tests.Core.Services
{
    public class TreeNodeServiceTests
    {
        private readonly Mock<ITreeNodeRepository> _mockRepo;
        private readonly Mock<ITreeNodeTypeRepository> _mockTypeRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<TreeNodeService>> _mockLogger;
        private readonly TreeNodeService _service;

        public TreeNodeServiceTests()
        {
            _mockRepo = new Mock<ITreeNodeRepository>();
            _mockTypeRepo = new Mock<ITreeNodeTypeRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<TreeNodeService>>();

            _service = new TreeNodeService(_mockRepo.Object, _mockTypeRepo.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllAsync_WhenRepositoryReturnsEntities_ReturnsSameEntities()
        {
            var entities = new List<TreeNode>
            {
                new TreeNode { Id = 1, Name = "A", NodeType = "T1" },
                new TreeNode { Id = 2, Name = "B", NodeType = "T2" }
            };

            _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<string?>(), It.IsAny<object?>())).ReturnsAsync(entities);

            var result = await _service.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(entities, result);
            _mockRepo.Verify(r => r.GetAllAsync(It.IsAny<string?>(), It.IsAny<object?>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((TreeNode?)null);

            var result = await _service.GetByIdAsync(999);

            Assert.Null(result);
            _mockRepo.Verify(r => r.GetByIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task GetRootNodesAsync_ReturnsRoots()
        {
            var roots = new List<TreeNode>
            {
                new TreeNode { Id = 1, Name = "R1", NodeType = "X", ParentId = null },
                new TreeNode { Id = 2, Name = "R2", NodeType = "X", ParentId = null }
            };

            _mockRepo.Setup(r => r.GetRootNodesAsync()).ReturnsAsync(roots);

            var result = await _service.GetRootNodesAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockRepo.Verify(r => r.GetRootNodesAsync(), Times.Once);
        }
    }
}
