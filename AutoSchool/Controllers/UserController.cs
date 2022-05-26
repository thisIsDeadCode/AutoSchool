using AutoSchool.Data;
using AutoSchool.Models.Tables;
using Microsoft.AspNetCore.Mvc;

namespace AutoSchool.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public UserController(ILogger<UserController> logger,
            ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("User/GetUsers")]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return _dbContext.Users.ToList();
        }
    }
}