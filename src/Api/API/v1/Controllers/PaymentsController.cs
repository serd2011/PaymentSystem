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
#if USE_AUTHENTICATION
            int userId = int.Parse(User.FindFirst("id").Value);
#else
            int userId = (int)data.id;
#endif

            var user = _dbContext.getUserOrCreateNew(userId);
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
#if USE_AUTHENTICATION
            int userId = int.Parse(User.FindFirst("id").Value);
#else
            int userId = (int)data.fromId;
#endif
            string desctiption = data.description ?? _messages["default_payment_description"];

            // Idempotensy key Check
            string? idempotencyKey = HttpContext.Request.Headers["Idempotency-Key"];
            if (idempotencyKey == null)
                return new BadRequestObjectResult(new ErrorResponseModel() { code = 101, description = $"Idempotency-Key header is missing" });
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                var duplicatePayment = _dbContext.Payments.Where(p => p.IdempotencyKey == idempotencyKey && p.FromId == userId).FirstOrDefault();
                if (duplicatePayment != null)
                {
                    if (duplicatePayment.Amount == data.amount && duplicatePayment.ToId == data.id && duplicatePayment.Description == desctiption)
                        return new OkObjectResult(new PostResponse() { });
                    return new UnprocessableEntityObjectResult(new ErrorResponseModel() { code = 102, description = $"Idempotency-Key mismatch" });
                }

                // Checking if payment can be done
                var destinationUser = _dbContext.Users.Find(data.id);
                if (destinationUser == null)
                    return new BadRequestObjectResult(new ErrorResponseModel() { code = 20, description = $"There is no user with id {data.id}." });
                var user = _dbContext.getUserOrCreateNew(userId);
                if ((uint)_dbContext.UsersBalances.Where(b => b.Id == user.Id).First().Balance < data.amount)
                    return new BadRequestObjectResult(new ErrorResponseModel() { code = 21, description = $"Amount {data.amount} can't be tranfered." });

                //Performing operation           
                _dbContext.Payments.Add(new Infrastructure.Database.Payment() { Amount = (int)data.amount, Description = desctiption, FromId = userId, ToId = data.id, IdempotencyKey = idempotencyKey });
                _dbContext.SaveChanges();
                transaction.Commit();
                return new OkObjectResult(new PostResponse() { });
            }
        }
    }
}
