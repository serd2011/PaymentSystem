using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public void Get() { }

        [HttpPost]
        public void Post() { }
    }
}
