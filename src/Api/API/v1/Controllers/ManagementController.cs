using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.v1.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly Infrastructure.Database.Context _dbContext;
        public ManagementController(Infrastructure.Database.Context context)
        {
            _dbContext = context;
        }

        [HttpPost]
        public void Add() { }

        [HttpPost]
        public void Remove() { }
    }
}
