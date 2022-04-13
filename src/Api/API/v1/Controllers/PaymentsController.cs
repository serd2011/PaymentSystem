using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using API.v1.Models.Payments;
using API.v1.Models;

namespace API.v1.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly Infrastructure.Database.Context _dbContext;
        public PaymentsController(Infrastructure.Database.Context context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Get([FromBody] GetRequest data)
        {
            return new OkResult();
        }

        [HttpPost]
        public IActionResult Post([FromBody] PostRequest data)
        {
            return new OkResult();
        }
    }
}
