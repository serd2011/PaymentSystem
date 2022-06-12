using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using API.v1.Models.Account;
using API.v1.Models;

using Application.DTO;
using Application.Services;
using Application.Exceptions;

namespace API.v1.Controllers
{
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

#if USE_AUTHENTICATION
        [HttpGet]
        public IActionResult Get()
        {
            int userId = int.Parse(User.FindFirst("id").Value);
            return Get(userId);
        }
#endif

#if USE_AUTHENTICATION
        [Authorize(Policy = "AdminOnly")]
#endif
        [HttpGet("{userId:int}")]
        public IActionResult Get(int userId)
        {
            User user;
            try
            {
                user = _userService.getUserInfo(userId);
            }
            catch (UserNotFoundException e)
            {
                return new BadRequestObjectResult(new ErrorResponseModel() { code = 1, description = $"There is no user with id {e.userId}." });
            }
            return new OkObjectResult(new GetResponse() { balance = user.balance });
        }

#if USE_AUTHENTICATION
        [Authorize(Policy = "AdminOnly")]
#endif
        [HttpPost("/add/{userId:int}")]
        public IActionResult Post(int userId)
        {
            try
            {
                _userService.createUser(userId);
            }
            catch (Exception)
            {
                return new BadRequestObjectResult(new ErrorResponseModel() { code = 10, description = $"Can't create user with id {userId}." });
            }
            return new OkResult();
        }
    }
}
