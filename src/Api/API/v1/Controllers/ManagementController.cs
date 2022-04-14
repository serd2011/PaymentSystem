using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using API.v1.Models.Management;
using API.v1.Models;
using API.Infrastructure.Database;

namespace API.v1.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly Context _dbContext;
        public ManagementController(Context context)
        {
            _dbContext = context;
        }

        [HttpPost("add/")]
        public IActionResult Add([FromBody] AddRequest data)
        {
            _dbContext.Database.BeginTransaction();
            var user = _dbContext.Users.Find(data.id);
            if (user == null)
            {
                user = new User() { Id = (int)data.id, Balance = (int)data.amount };
                _dbContext.Users.Add(user);
            }
            else
            {
                user.Balance += (int)data.amount;
            }
            _dbContext.SaveChanges();
            _dbContext.Database.CommitTransaction();
            return new OkObjectResult(new RemoveResponse() { balance = (uint)user.Balance });
        }

        [HttpPost("remove/")]
        public IActionResult Remove([FromBody] RemoveRequest data)
        {
            _dbContext.Database.BeginTransaction();
            var user = _dbContext.Users.Find(data.id);
            if (user == null)
            {
                _dbContext.Database.RollbackTransaction();
                return new BadRequestObjectResult(new ErrorResponseModel() { code = 40, description = $"There is no user with id {data.id}." });
            }
            if(user.Balance < data.amount) {
                _dbContext.Database.RollbackTransaction();
                return new BadRequestObjectResult(new ErrorResponseModel() { code = 41, description = $"Amount {data.amount} can't be removed from user with id {data.id}." });
            }
            user.Balance -= (int)data.amount;
            _dbContext.SaveChanges();
            _dbContext.Database.CommitTransaction();
            return new OkObjectResult(new RemoveResponse() { balance = (uint)user.Balance });
        }
    }
}
