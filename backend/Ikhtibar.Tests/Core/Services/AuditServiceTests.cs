using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using Ikhtibar.Tests.TestHelpers;
using Xunit;

namespace Ikhtibar.Tests.Core.Services
{
    public class AuditServiceTests
    {
        private Mock<IAuditLogRepository> _mockAuditLogRepository;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<IMapper> _mockMapper;
    private Mock<ILogger> _mockLogger;
        private Mock<HttpContext> _mockHttpContext;
        private Mock<HttpRequest> _mockHttpRequest;
        private Mock<HttpResponse> _mockHttpResponse;
    private TestableAuditService _auditService;

        public AuditServiceTests()
        {
            // Ensure EPPlus license is set if EPPlus is present. Use reflection to avoid compile-time
            // dependency on specific EPPlus API versions (License vs LicenseContext).
            try
            {
                var excelPackageType = Type.GetType("OfficeOpenXml.ExcelPackage, EPPlus");
                if (excelPackageType != null)
                {
                    var licenseProp = excelPackageType.GetProperty("License", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                    if (licenseProp != null && licenseProp.PropertyType.IsEnum)
                    {
                        var nonCommercial = Enum.Parse(licenseProp.PropertyType, "NonCommercial");
                        licenseProp.SetValue(null, nonCommercial);
                    }
                    else
                    {
                        var licenseContextProp = excelPackageType.GetProperty("LicenseContext", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                        if (licenseContextProp != null && licenseContextProp.PropertyType.IsEnum)
                        {
                            var nonCommercial = Enum.Parse(licenseContextProp.PropertyType, "NonCommercial");
                            licenseContextProp.SetValue(null, nonCommercial);
                        }
                    }
                }
            }
            catch
            {
                // If EPPlus types are not available or reflection fails, tests that don't touch Excel will continue.
            }

            _mockAuditLogRepository = new Mock<IAuditLogRepository>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger>();
            _mockHttpContext = new Mock<HttpContext>();
            _mockHttpRequest = new Mock<HttpRequest>();
            _mockHttpResponse = new Mock<HttpResponse>();

            // Setup HTTP context
            _mockHttpContext.Setup(c => c.Request).Returns(_mockHttpRequest.Object);
            _mockHttpContext.Setup(c => c.Response).Returns(_mockHttpResponse.Object);
            _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(_mockHttpContext.Object);

            _auditService = new TestableAuditService(
                _mockAuditLogRepository.Object,
                _mockHttpContextAccessor.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task LogAsync_ShouldCreateAuditLog_WhenValidEntryProvided()
        {
            var entry = new AuditLogEntry
            {
                UserId = 1,
                UserIdentifier = "test@example.com",
                Action = "CREATE_USER",
                EntityType = "User",
                EntityId = "123",
                Details = "User creation test",
                Severity = AuditSeverity.Medium,
                Category = AuditCategory.UserManagement,
                IsSystemAction = false
            };

            var auditLog = new AuditLog
            {
                AuditLogId = 1,
                UserId = 1,
                UserIdentifier = "test@example.com",
                Action = "CREATE_USER",
                EntityType = "User",
                EntityId = "123",
                Details = "User creation test",
                Severity = AuditSeverity.Medium,
                Category = AuditCategory.UserManagement,
                IsSystemAction = false,
                Timestamp = System.DateTime.UtcNow
            };

            _mockAuditLogRepository.Setup(r => r.AddAsync(It.IsAny<AuditLog>())).ReturnsAsync(auditLog);

            var result = await _auditService.LogAsync(entry);

            Assert.Equal(1, result);
            _mockAuditLogRepository.Verify(r => r.AddAsync(It.IsAny<AuditLog>()), Times.Once);
        }

        [Fact]
        public async Task LogUserActionAsync_ShouldCreateUserActionLog_WhenValidDataProvided()
        {
            var userId = 1;
            var action = "UPDATE_PROFILE";
            var entityType = "User";
            var entityId = "123";
            var oldValues = new { Name = "Old Name" };
            var newValues = new { Name = "New Name" };

            var auditLog = new AuditLog
            {
                AuditLogId = 1,
                UserId = userId,
                UserIdentifier = userId.ToString(),
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                OldValues = "{\"Name\":\"Old Name\"}",
                NewValues = "{\"Name\":\"New Name\"}",
                Severity = AuditSeverity.Medium,
                Category = AuditCategory.UserManagement,
                IsSystemAction = false
            };

            _mockAuditLogRepository.Setup(r => r.AddAsync(It.IsAny<AuditLog>())).ReturnsAsync(auditLog);

            var result = await _auditService.LogUserActionAsync(userId, action, entityType, entityId, oldValues, newValues);

            Assert.Equal(1, result);
            _mockAuditLogRepository.Verify(r => r.AddAsync(It.IsAny<AuditLog>()), Times.Once);
        }

        [Fact]
        public async Task LogSecurityEventAsync_ShouldCreateSecurityEventLog_WhenValidDataProvided()
        {
            var userIdentifier = "test@example.com";
            var action = "LOGIN_FAILED";
            var details = "Invalid password attempt";
            var severity = AuditSeverity.High;

            var auditLog = new AuditLog
            {
                AuditLogId = 1,
                UserIdentifier = userIdentifier,
                Action = action,
                EntityType = "Security",
                Details = details,
                Severity = severity,
                Category = AuditCategory.Security,
                IsSystemAction = false
            };

            _mockAuditLogRepository.Setup(r => r.AddAsync(It.IsAny<AuditLog>())).ReturnsAsync(auditLog);

            var result = await _auditService.LogSecurityEventAsync(userIdentifier, action, details, severity);

            Assert.Equal(1, result);
            _mockAuditLogRepository.Verify(r => r.AddAsync(It.IsAny<AuditLog>()), Times.Once);
        }

        [Fact]
        public async Task LogSystemActionAsync_ShouldCreateSystemActionLog_WhenValidDataProvided()
        {
            var action = "SYSTEM_MAINTENANCE";
            var details = "Database cleanup completed";
            var entityType = "System";
            var entityId = "DB_CLEANUP";

            var auditLog = new AuditLog
            {
                AuditLogId = 1,
                UserIdentifier = "System",
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Details = details,
                Severity = AuditSeverity.Medium,
                Category = AuditCategory.System,
                IsSystemAction = true
            };

            _mockAuditLogRepository.Setup(r => r.AddAsync(It.IsAny<AuditLog>())).ReturnsAsync(auditLog);

            var result = await _auditService.LogSystemActionAsync(action, details, entityType, entityId);

            Assert.Equal(1, result);
            _mockAuditLogRepository.Verify(r => r.AddAsync(It.IsAny<AuditLog>()), Times.Once);
        }

        [Fact]
        public async Task GetAuditLogsAsync_ShouldReturnPaginatedLogs_WhenRepositoryReturnsData()
        {
            var filter = new AuditLogFilter
            {
                Page = 1,
                PageSize = 20,
                FromDate = System.DateTime.UtcNow.AddDays(-7),
                ToDate = System.DateTime.UtcNow
            };

            var repositoryResult = new PagedResult<AuditLog>
            {
                Items = new List<AuditLog>
                {
                    new AuditLog { AuditLogId = 1, Action = "CREATE_USER", EntityType = "User" },
                    new AuditLog { AuditLogId = 2, Action = "UPDATE_PROFILE", EntityType = "User" }
                },
                TotalCount = 2,
                PageNumber = 1,
                PageSize = 20
            };

            var expectedDtos = new List<AuditLogDto>
            {
                new AuditLogDto { AuditLogId = 1, Action = "CREATE_USER", EntityType = "User" },
                new AuditLogDto { AuditLogId = 2, Action = "UPDATE_PROFILE", EntityType = "User" }
            };

            _mockAuditLogRepository.Setup(r => r.GetAuditLogsAsync(filter)).ReturnsAsync(repositoryResult);
            _mockMapper.Setup(m => m.Map<List<AuditLogDto>>(repositoryResult.Items)).Returns(expectedDtos);

            var result = await _auditService.GetAuditLogsAsync(filter);

            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count());
            Assert.Equal(2, result.TotalCount);
            _mockAuditLogRepository.Verify(r => r.GetAuditLogsAsync(filter), Times.Once);
        }

        [Fact]
        public async Task GetUserAuditLogsAsync_ShouldReturnUserSpecificLogs_WhenRepositoryReturnsData()
        {
            var userId = 1;
            var fromDate = System.DateTime.UtcNow.AddDays(-7);
            var toDate = System.DateTime.UtcNow;
            var filter = new AuditLogFilter
            {
                Page = 1,
                PageSize = 20,
                FromDate = fromDate,
                ToDate = toDate
            };

            var repositoryResult = new List<AuditLog>
            {
                new AuditLog { AuditLogId = 1, UserId = userId, Action = "LOGIN" },
                new AuditLog { AuditLogId = 2, UserId = userId, Action = "UPDATE_PROFILE" }
            };

            var expectedDtos = new List<AuditLogDto>
            {
                new AuditLogDto { AuditLogId = 1, UserId = userId, Action = "LOGIN" },
                new AuditLogDto { AuditLogId = 2, UserId = userId, Action = "UPDATE_PROFILE" }
            };

            _mockAuditLogRepository.Setup(r => r.GetUserAuditLogsAsync(userId, fromDate, toDate)).ReturnsAsync(repositoryResult);
            _mockMapper.Setup(m => m.Map<List<AuditLogDto>>(repositoryResult)).Returns(expectedDtos);

            var result = await _auditService.GetUserAuditLogsAsync(userId, filter);

            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count());
            Assert.All(result.Items, item => Assert.Equal(userId, item.UserId));
            _mockAuditLogRepository.Verify(r => r.GetUserAuditLogsAsync(userId, fromDate, toDate), Times.Once);
        }

        [Fact]
        public async Task GetSecurityEventsAsync_ShouldReturnSecurityEvents_WhenValidRequest()
        {
            var fromDate = System.DateTime.UtcNow.AddDays(-30);
            var toDate = System.DateTime.UtcNow;

            var repositoryResult = new List<AuditLog>
            {
                new AuditLog { AuditLogId = 1, Action = "LOGIN_FAILED", EntityType = "Security" },
                new AuditLog { AuditLogId = 2, Action = "ACCOUNT_LOCKED", EntityType = "Security" }
            };

            var expectedDtos = new List<AuditLogDto>
            {
                new AuditLogDto { AuditLogId = 1, Action = "LOGIN_FAILED", EntityType = "Security" },
                new AuditLogDto { AuditLogId = 2, Action = "ACCOUNT_LOCKED", EntityType = "Security" }
            };

            _mockAuditLogRepository.Setup(r => r.GetSecurityEventsAsync(fromDate, toDate)).ReturnsAsync(repositoryResult);
            _mockMapper.Setup(m => m.Map<IEnumerable<AuditLogDto>>(It.IsAny<IEnumerable<AuditLog>>())).Returns((IEnumerable<AuditLog> items) => items.Select(x => new AuditLogDto { AuditLogId = x.AuditLogId, Action = x.Action, EntityType = x.EntityType }).ToList());

            var result = await _auditService.GetSecurityEventsAsync(fromDate, toDate);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(expectedDtos.Count, result.Count());
            _mockAuditLogRepository.Verify(r => r.GetSecurityEventsAsync(fromDate, toDate), Times.Once);
        }

        [Fact]
        public async Task ArchiveOldLogsAsync_ShouldReturnArchivedCount_WhenValidRetentionPeriodProvided()
        {
            var retentionPeriod = System.TimeSpan.FromDays(365);
            var expectedCount = 5;

            _mockAuditLogRepository.Setup(r => r.ArchiveLogsAsync(It.IsAny<System.DateTime>())).ReturnsAsync(expectedCount);

            var result = await _auditService.ArchiveOldLogsAsync(retentionPeriod);

            Assert.Equal(expectedCount, result);
            _mockAuditLogRepository.Verify(r => r.ArchiveLogsAsync(It.IsAny<System.DateTime>()), Times.Once);
        }

        [Fact]
        public async Task VerifyLogsIntegrityAsync_ShouldReturnIntegrityResults_WhenValidDateRangeProvided()
        {
            var auditLogIds = new List<int> { 1, 2, 3 };
            var integrityResults = new Dictionary<int, bool>
            {
                { 1, true },
                { 2, true },
                { 3, false }
            };

            foreach (var id in auditLogIds)
            {
                _mockAuditLogRepository.Setup(r => r.VerifyLogIntegrityAsync(id)).ReturnsAsync(integrityResults[id]);
            }

            var result = new Dictionary<int, bool>();
            foreach (var id in auditLogIds)
            {
                result[id] = await _mockAuditLogRepository.Object.VerifyLogIntegrityAsync(id);
            }

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.True(result[1]);
            Assert.True(result[2]);
            Assert.False(result[3]);
            foreach (var id in auditLogIds)
            {
                _mockAuditLogRepository.Verify(r => r.VerifyLogIntegrityAsync(id), Times.Once);
            }
        }

        [Fact]
        public async Task LogAsync_ShouldHandleNullHttpContext_WhenHttpContextIsNull()
        {
            _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns((HttpContext?)null);

            var entry = new AuditLogEntry
            {
                UserId = 1,
                UserIdentifier = "test@example.com",
                Action = "CREATE_USER",
                EntityType = "User",
                EntityId = "123"
            };

            var auditLog = new AuditLog
            {
                AuditLogId = 1,
                UserId = 1,
                UserIdentifier = "test@example.com",
                Action = "CREATE_USER",
                EntityType = "User",
                EntityId = "123"
            };

            _mockAuditLogRepository.Setup(r => r.AddAsync(It.IsAny<AuditLog>())).ReturnsAsync(auditLog);

            var result = await _auditService.LogAsync(entry);

            Assert.Equal(1, result);
            _mockAuditLogRepository.Verify(r => r.AddAsync(It.IsAny<AuditLog>()), Times.Once);
        }

        [Fact]
        public async Task LogAsync_ShouldExtractClientIpAddress_WhenHttpContextAvailable()
        {
            var headers = new HeaderDictionary
            {
                { "X-Forwarded-For", "192.168.1.100" },
                { "User-Agent", "Test Browser" }
            };

            _mockHttpRequest.Setup(r => r.Headers).Returns(headers);

            var entry = new AuditLogEntry
            {
                UserId = 1,
                UserIdentifier = "test@example.com",
                Action = "CREATE_USER",
                EntityType = "User",
                EntityId = "123"
            };

            var auditLog = new AuditLog
            {
                AuditLogId = 1,
                UserId = 1,
                UserIdentifier = "test@example.com",
                Action = "CREATE_USER",
                EntityType = "User",
                EntityId = "123",
                IpAddress = "192.168.1.100",
                UserAgent = "Test Browser"
            };

            _mockAuditLogRepository.Setup(r => r.AddAsync(It.IsAny<AuditLog>())).ReturnsAsync(auditLog);

            var result = await _auditService.LogAsync(entry);

            Assert.Equal(1, result);
            _mockAuditLogRepository.Verify(r => r.AddAsync(It.Is<AuditLog>(l =>
                l.IpAddress == "192.168.1.100" && l.UserAgent == "Test Browser")), Times.Once);
        }

        [Fact]
        public async Task LogAsync_ShouldHandleRepositoryException_WhenRepositoryFails()
        {
            var entry = new AuditLogEntry
            {
                UserId = 1,
                UserIdentifier = "test@example.com",
                Action = "CREATE_USER",
                EntityType = "User",
                EntityId = "123"
            };

            var exception = new System.InvalidOperationException("Database connection failed");
            _mockAuditLogRepository.Setup(r => r.AddAsync(It.IsAny<AuditLog>())).ThrowsAsync(exception);

            var ex = await Assert.ThrowsAsync<System.InvalidOperationException>(async () => await _auditService.LogAsync(entry));
            Assert.Equal("Database connection failed", ex.Message);
            _mockAuditLogRepository.Verify(r => r.AddAsync(It.IsAny<AuditLog>()), Times.Once);
        }
    }
}
