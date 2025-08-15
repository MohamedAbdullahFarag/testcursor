using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Ikhtibar.API.Controllers.Base;

namespace Ikhtibar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomReportController : ApiControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok("CustomReportController - Get method not implemented");
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] object request)
        {
            return Ok("CustomReportController - Post method not implemented");
        }
    }
}
