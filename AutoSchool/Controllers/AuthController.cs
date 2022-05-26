using AutoSchool.Data;
using AutoSchool.Models.Tables;
using AutoSchool.Models.Views;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AutoSchool.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public AuthController(ILogger<UserController> logger,
            ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpPost]
        [Route("Auth/Login")]
        public async Task<ActionResult<LoginView>> Login([FromBody] LoginView login)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == login.Email && u.Password == login.Password);

            if (user != null)
            {
                await Authenticate(login.Email); 
                return Ok();
            }
            else
            {
                login.Errors = new List<string>()
                    {
                        "Неверный логин или пароль"
                    };

                return login;
            }
        }

      

        [Authorize]
        [HttpGet]
        [Route("Auth/Logout")]
        public async Task<ActionResult<ResponseView>> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return new ResponseView();
        }

        [HttpPost]
        [Route("Auth/Registration")]
        public async Task<ActionResult<RegistrationView>> Registration([FromBody] RegistrationView registration)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == registration.Email);
            if (user == null)
            {
                _dbContext.Users.Add(new User { Email = registration.Email, Password = registration.Password, FullName = registration.FullName });
                await _dbContext.SaveChangesAsync();

                await Authenticate(registration.Email);
                return Ok();
            }
            else
            {
                registration.Errors = new List<string>()
                {
                    "Такой пользователь уже существует"
                };
                return registration;
            }
        }

        private async Task Authenticate(string email)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, email)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}