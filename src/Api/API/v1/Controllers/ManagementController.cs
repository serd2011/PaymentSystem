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
            string desctiption = data.description ?? _messages["default_payment_description:add"];

            // Idempotensy key Check
            string? idempotencyKey = HttpContext.Request.Headers["Idempotency-Key"];
            if (idempotencyKey == null)
                return new BadRequestObjectResult(new ErrorResponseModel() { code = 101, description = $"Idempotency-Key header is missing" });
            _dbContext.Database.BeginTransaction();
            var duplicatePayment = _dbContext.Payments.Where(p => p.IdempotencyKey == idempotencyKey && p.FromId == 0).FirstOrDefault();
            if (duplicatePayment != null)
            {
                _dbContext.Database.RollbackTransaction();
                if (duplicatePayment.Amount == data.amount && duplicatePayment.ToId == data.id && duplicatePayment.Description == desctiption)
                    return new OkObjectResult(new AddResponse() { });
                else
                    return new UnprocessableEntityObjectResult(new ErrorResponseModel() { code = 102, description = $"Idempotency-Key mismatch" });
            }

            // Performing action
            var user = _dbContext.getUserOrCreateNew(int.Parse(User.FindFirst("id").Value));
            _dbContext.Payments.Add(new Payment() { Amount = (int)data.amount, Description = desctiption, ToId = (int)data.id, IdempotencyKey = idempotencyKey });
            _dbContext.SaveChanges();
            _dbContext.Database.CommitTransaction();
            return new OkObjectResult(new AddResponse() { });
        }

        [HttpPost("remove/")]
        public IActionResult Remove([FromBody] RemoveRequest data)
        {
            string desctiption = data.description ?? _messages["default_payment_description:remove"];

            // Idempotensy key Check
            string? idempotencyKey = HttpContext.Request.Headers["Idempotency-Key"];
            if (idempotencyKey == null)
                return new BadRequestObjectResult(new ErrorResponseModel() { code = 101, description = $"Idempotency-Key header is missing" });
            _dbContext.Database.BeginTransaction();
            var duplicatePayment = _dbContext.Payments.Where(p => p.IdempotencyKey == idempotencyKey && p.FromId == 0).FirstOrDefault();
            if (duplicatePayment != null)
            {
                _dbContext.Database.RollbackTransaction();
                if (duplicatePayment.Amount == data.amount && duplicatePayment.ToId == data.id && duplicatePayment.Description == desctiption)
                    return new OkObjectResult(new AddResponse() { });
                else
                    return new UnprocessableEntityObjectResult(new ErrorResponseModel() { code = 102, description = $"Idempotency-Key mismatch" });
            }

            // Checking if can be done
            var user = _dbContext.Users.Find(data.id);
            if (user == null)
            {
                _dbContext.Database.RollbackTransaction();
                return new BadRequestObjectResult(new ErrorResponseModel() { code = 40, description = $"There is no user with id {data.id}." });
            }
            if ((uint)_dbContext.UsersBalances.Where(b => b.Id == user.Id).First().Balance < data.amount)
            {
                _dbContext.Database.RollbackTransaction();
                return new BadRequestObjectResult(new ErrorResponseModel() { code = 41, description = $"Amount {data.amount} can't be removed from user with id {data.id}." });
            }

            // Performing action
            _dbContext.Payments.Add(new Payment() { Amount = (int)data.amount, Description = desctiption, FromId = (int)data.id, IdempotencyKey = idempotencyKey });
            _dbContext.SaveChanges();
            _dbContext.Database.CommitTransaction();
            return new OkObjectResult(new RemoveResponse() { });
        }
    }
}
