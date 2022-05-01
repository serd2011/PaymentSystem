using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using API.Infrastructure.Database;

using API.v1.Models.Payments;
using API.v1.Models;
using API.v1.Other;

namespace API.v1.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly Context _dbContext;
        private readonly IConfiguration _messages;
        public PaymentsController(IConfiguration config, Context context)
        {
            _dbContext = context;
            _messages = config.GetSection("messages:payments");
        }

        [HttpGet]
        public IActionResult Get([FromQuery] GetRequest data)
        {
            var user = _dbContext.getUserOrCreateNew(int.Parse(User.FindFirst("id").Value));
            if (data.cursor == null)
            {
                data.cursor = int.MaxValue;
            }
            else
            {
                var cursorPayment = _dbContext.Payments.Find(data.cursor);
                if (cursorPayment == null || (cursorPayment.FromId != user.Id && cursorPayment.ToId != user.Id))
                    return new BadRequestObjectResult(new ErrorResponseModel() { code = 10, description = $"Cursor is invalid" });
            }
            var payments = _dbContext.Payments.Where(p => p.Id < data.cursor && (p.FromId == user.Id || p.ToId == user.Id))
                .OrderByDescending(p => p.Id)
                .Take((int)data.limit)
                .Select(p => new API.v1.Models.Payments.Payment()
                {
                    id = p.Id,
                    amount = (uint)p.Amount,
                    date = p.Date,
                    description = p.Description,
                    isPaid = p.FromId == user.Id,
                    userId = (p.FromId == user.Id ? p.ToId : p.FromId) ?? 0
                });
            int? cursor = null;
            if (payments.Count() > 0 && payments.Count() == data.limit)
                cursor = payments.Last().id;
            return new OkObjectResult(new GetResponse() { cursor = cursor, operations = payments.ToList() });
        }

        [HttpPost]
        public IActionResult Post([FromBody] PostRequest data)
        {
            _dbContext.Database.BeginTransaction();
            var destinationUser = _dbContext.Users.Find(data.id);
            if (destinationUser == null)
            {
                _dbContext.Database.RollbackTransaction();
                return new BadRequestObjectResult(new ErrorResponseModel() { code = 20, description = $"There is no user with id {data.id}." });
            }
            int userId = int.Parse(User.FindFirst("id").Value);
            var user = _dbContext.getUserOrCreateNew(userId);
            if (user.Balance < data.amount)
            {
                _dbContext.Database.RollbackTransaction();
                return new BadRequestObjectResult(new ErrorResponseModel() { code = 21, description = $"Amount {data.amount} can't be tranfered." });
            }
            user.Balance -= (int)data.amount;
            destinationUser.Balance += (int)data.amount;
            _dbContext.Payments.Add(new Infrastructure.Database.Payment() { Amount = (int)data.amount, Description = data.description ?? _messages["default_payment_description"], FromId = userId, ToId = data.id });
            _dbContext.SaveChanges();
            _dbContext.Database.CommitTransaction();
            return new OkObjectResult(new PostResponse() { balance = (uint)user.Balance });
        }
    }
}
