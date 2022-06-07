using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using API.Infrastructure.Database;

using API.v1.Models.Account;
using API.v1.Models;

using API.v1.Other;

namespace API.v1.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly Context _dbContext;
        public AccountController(Context context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var user = _dbContext.getUserOrCreateNew(int.Parse(User.FindFirst("id").Value));
            return new OkObjectResult(new GetResponse() { balance = (uint)_dbContext.UsersBalances.Where(b => b.Id == user.Id).First().Balance });
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("{userId:int}")]
        public IActionResult Get(int userId)
        {
            var userBalance = _dbContext.UsersBalances.Where(b => b.Id == userId).FirstOrDefault();
            if (userBalance == null)
                return new BadRequestObjectResult(new ErrorResponseModel() { code = 1, description = $"There is no user with id {userId}." });
            return new OkObjectResult(new GetResponse() { balance = (uint)userBalance.Balance });
        }
    }
}
