using Microsoft.AspNetCore.Mvc;

using API.v1.Models.Payments;
using API.v1.Models;

using Application.DTO;
using Application.Services;
using Application.Exceptions;

namespace API.v1.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentsService _paymentSevice;
        private readonly IConfiguration _messages;
        public PaymentsController(IConfiguration config, IPaymentsService paymentSevice)
        {
            _paymentSevice = paymentSevice;
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

            PaymentsServiceDTOs payments;
            try
            {
                payments = _paymentSevice.getUserPayments(new PaymentsRequest()
                {
                    id = userId,
                    limit = data.limit,
                    cursor = data.cursor
                });
            }
            catch (CursorInvalidException)
            {
                return new BadRequestObjectResult(new ErrorResponseModel() { code = 20, description = $"Cursor is invalid" });
            }

            return new OkObjectResult(payments);
        }

        [HttpPost]
        public IActionResult Post([FromBody] PostRequest data)
        {
#if USE_AUTHENTICATION
            int userId = int.Parse(User.FindFirst("id").Value);
#else
            int userId = (int)data.fromId;
#endif
            
            string? idempotencyKey = HttpContext.Request.Headers["Idempotency-Key"];
            if (idempotencyKey == null)
                return new BadRequestObjectResult(new ErrorResponseModel() { code = 2, description = $"Idempotency-Key header is missing" });

            try
            {
                _paymentSevice.createPayment(new CreatePaymentRequest()
                {
                    fromId = userId,
                    toId = (int)data.id,
                    amount = (uint)data.amount,
                    description = data.description ?? _messages["default_payment_description"],
                    idempotencyKey = idempotencyKey
                });
            }
            catch (UserNotFoundException e)
            {
                return new BadRequestObjectResult(new ErrorResponseModel() { code = 1, description = $"There is no user with id {e.userId}." });
            }
            catch (IdempotencyMismatchException)
            {
                return new UnprocessableEntityObjectResult(new ErrorResponseModel() { code = 3, description = $"Idempotency-Key mismatch" });
            }
            catch (NotEnoughBalanceException)
            {
                return new BadRequestObjectResult(new ErrorResponseModel() { code = 21, description = $"Amount {data.amount} can't be tranfered." });
            }
            return new OkResult();
        }
    }
}
