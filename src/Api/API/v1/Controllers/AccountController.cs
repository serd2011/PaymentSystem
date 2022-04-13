using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using API.v1.Models.Account;
using API.v1.Models;

namespace API.v1.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly Infrastructure.Database.Context _dbContext;
        public AccountController(Infrastructure.Database.Context context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new OkObjectResult(new GetResponse() { balance = 10 });
        }

        [HttpGet("{userId:int}")]
        public IActionResult Get(int userId)
        {
            return new BadRequestObjectResult(new ErrorResponseModel() { code = 1, description = "There is no user with id " + userId.ToString() });
        }
    }
}
