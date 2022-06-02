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

            if(user != null)
            {
                registration.Errors = new List<string>()
                {
                    "Такой пользователь уже существует"
                };
                return registration;
            }

            if (IsValidEmail(registration.Email, registration.Errors) && IsValidPassword(registration.Password, registration.Errors))
            {
                var userDB =  new User() { Email = registration.Email, Password = registration.Password, FullName = registration.FullName };

                _dbContext.Users.Add(userDB);
                _dbContext.Students.Add(new Student () { User = userDB});
                await _dbContext.SaveChangesAsync();

                await Authenticate(registration.Email);
                return Ok();
            }

            return registration;
        }

        [Authorize]
        [HttpPost]
        [Route("Auth/PasswordReset")]
        public async Task<ActionResult<PasswordResetView>> PasswordReset([FromBody] PasswordResetView passwordReset)
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
            if (user != null)
            {
                if(IsValidPassword(passwordReset.Password, passwordReset.Errors) &&
                    passwordReset.Password == passwordReset.ConfirmPassword)
                {
                    user.Password = passwordReset.Password;
                    _dbContext.Users.Update(user);
                    await _dbContext.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    passwordReset.Errors = new List<string>()
                    {
                        "Пароли не совпадают"
                    };
                    return passwordReset;
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