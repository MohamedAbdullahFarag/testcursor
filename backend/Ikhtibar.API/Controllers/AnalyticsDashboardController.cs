using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Ikhtibar.API.Controllers.Base;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsDashboardController : ApiControllerBase
    {
        private readonly IAnalyticsDashboardService _analyticsDashboardService;
        private readonly ILogger<AnalyticsDashboardController> _logger;

        public AnalyticsDashboardController(IAnalyticsDashboardService analyticsDashboardService, ILogger<AnalyticsDashboardController> logger)
        {
            _analyticsDashboardService = analyticsDashboardService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] DashboardFilterDto? filter = null)
        {
            _logger.LogInformation("Analytics dashboard GET request received");
            var result = await _analyticsDashboardService.ExecuteAsync(filter ?? new object());
            if (result.IsSuccess)
                return SuccessResponse(result.Data, "Analytics dashboard data retrieved successfully");
            return ErrorResponse(result.ErrorMessage ?? "Failed to retrieve analytics dashboard data", 500);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] object request)
        {
            _logger.LogInformation("Analytics dashboard POST request received");
            var result = await _analyticsDashboardService.ExecuteAsync(request);
            if (result.IsSuccess)
                return SuccessResponse(result.Data, "Analytics dashboard data retrieved successfully");
            return ErrorResponse(result.ErrorMessage ?? "Failed to retrieve analytics dashboard data", 500);
        }
    }
}
