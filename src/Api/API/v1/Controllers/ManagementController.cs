using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using API.Infrastructure.Database;

using API.v1.Models.Management;
using API.v1.Models;
using API.v1.Other;

namespace API.v1.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly Context _dbContext;
        private readonly IConfiguration _messages;
        public ManagementController(IConfiguration config, Context context)
        {
            _dbContext = context;
            _messages = config.GetSection("messages:management");
        }

        [HttpPost("add/")]
        public IActionResult Add([FromBody] AddRequest data)
        {
            _dbContext.Database.BeginTransaction();
            var user = _dbContext.getUserOrCreateNew(int.Parse(User.FindFirst("id").Value));
            user.Balance += (int)data.amount;           
            _dbContext.Payments.Add(new Payment() { Amount = (int)data.amount, Description = data.description ?? _messages["default_payment_description:add"], ToId = (int)data.id });
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
            if (user.Balance < data.amount)
            {
                _dbContext.Database.RollbackTransaction();
                return new BadRequestObjectResult(new ErrorResponseModel() { code = 41, description = $"Amount {data.amount} can't be removed from user with id {data.id}." });
            }
            user.Balance -= (int)data.amount;
            _dbContext.Payments.Add(new Payment() { Amount = (int)data.amount, Description = data.description ?? _messages["default_payment_description:remove"], FromId = (int)data.id });
            _dbContext.SaveChanges();
            _dbContext.Database.CommitTransaction();
            return new OkObjectResult(new RemoveResponse() { balance = (uint)user.Balance });
        }
    }
}
