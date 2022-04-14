using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using API.v1.Models.Management;
using API.v1.Models;

namespace API.v1.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly Infrastructure.Database.Context _dbContext;
        public ManagementController(Infrastructure.Database.Context context)
        {
            _dbContext = context;
        }

        [HttpPost("add/")]
        public IActionResult Add([FromBody] AddRequest data)
        {
            return new OkResult();
        }

        [HttpPost("remove/")]
        public IActionResult Remove([FromBody] RemoveRequest data)
        {
            return new OkResult();
        }
    }
}
