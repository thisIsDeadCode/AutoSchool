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
        public async Task<ActionResult<Response>> Login([FromBody] LoginRequest login)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == login.Email && u.Password == login.Password);

            if (user != null)
            {
                await Authenticate(login.Email); 
                return Ok();
            }
            else
            {
                var response = new Response()
                {
                    Errors = new List<string> { "Неверный логин или пароль" }
                };

                return response;
            }
        }      

        [Authorize]
        [HttpGet]
        [Route("Auth/Logout")]
        public async Task<ActionResult<Response>> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        [HttpPost]
        [Route("Auth/Registration")]
        public async Task<ActionResult<Response>> Registration([FromBody] RegistrationRequest registration)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == registration.Email);
            var response = new Response();

            if (user != null)
            {
                response.Errors = new List<string> { "Такой пользователь уже существует" };

                return response;
            }

            if (IsValidEmail(registration.Email, response.Errors) && IsValidPassword(registration.Password, response.Errors))
            {
                var userDB =  new User() { Email = registration.Email, Password = registration.Password, FullName = registration.FullName };

                _dbContext.Users.Add(userDB);
                _dbContext.Students.Add(new Student () { User = userDB});
                await _dbContext.SaveChangesAsync();

                await Authenticate(registration.Email);
                return Ok();
            }

            return response;
        }

        [Authorize]
        [HttpPost]
        [Route("Auth/PasswordReset")]
        public async Task<ActionResult<Response>> PasswordReset([FromBody] PasswordResetRequest passwordReset)
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
            var response = new Response();

            if (user != null)
            {
                if(IsValidPassword(passwordReset.Password, response.Errors) &&
                    passwordReset.Password == passwordReset.ConfirmPassword)
                {
                    user.Password = passwordReset.Password;
                    _dbContext.Users.Update(user);
                    await _dbContext.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    response.Errors = new List<string> { "Пароли не совпадают" };

                    return response;
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
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

        bool IsValidEmail(string email, List<string> errors)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                errors.Add("Введите правильный Email");
                return false;
            }
        }

        bool IsValidPassword(string password, List<string> errors)
        {
            if (password != null && password.Length > 6)
            {
                return true;
            }
            else
            {
                errors.Add("Пароль должен быть не меньше 6 символов");
                return false;
            }
        }
    }
}