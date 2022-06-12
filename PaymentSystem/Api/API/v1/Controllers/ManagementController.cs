using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using API.v1.Models.Management;
using API.v1.Models;

using Application.DTO;
using Application.Services;
using Application.Exceptions;

namespace API.v1.Controllers
{
#if USE_AUTHENTICATION
    [Authorize(Policy = "AdminOnly")]
#endif
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _messages;
        public ManagementController(IConfiguration config, IUserService userService)
        {
            _userService = userService;
            _messages = config.GetSection("messages:management");
        }

        [HttpPost("add/")]
        public IActionResult Add([FromBody] AddRequest data)
        {
            string? idempotencyKey = HttpContext.Request.Headers["Idempotency-Key"];
            if (idempotencyKey == null)
                return new BadRequestObjectResult(new ErrorResponseModel() { code = 2, description = $"Idempotency-Key header is missing" });

            try
            {
                _userService.modifyBalance(new ModifyBalanceRequest
                {
                    id = (int)data.id,
                    amount = (int)data.amount,
                    description = data.description ?? _messages["default_payment_description:add"],
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
            return new OkResult();
        }

        [HttpPost("remove/")]
        public IActionResult Remove([FromBody] RemoveRequest data)
        {
            string? idempotencyKey = HttpContext.Request.Headers["Idempotency-Key"];
            if (idempotencyKey == null)
                return new BadRequestObjectResult(new ErrorResponseModel() { code = 2, description = $"Idempotency-Key header is missing" });

            try
            {
                _userService.modifyBalance(new ModifyBalanceRequest
                {
                    id = (int)data.id,
                    amount = -(int)data.amount,
                    description = data.description ?? _messages["default_payment_description:add"],
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
                return new BadRequestObjectResult(new ErrorResponseModel() { code = 30, description = $"Amount {data.amount} can't be removed from user with id {data.id}." });
            }
            return new OkResult();
        }
    }
}
