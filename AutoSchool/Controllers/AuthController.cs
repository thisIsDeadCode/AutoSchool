using AutoSchool.Data;
using AutoSchool.Extensions;
using AutoSchool.Models.Tables;
using AutoSchool.Models.Views;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AutoSchool.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthController(ILogger<UserController> logger,
            ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _logger = logger;
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Auth/Login")]
        public async Task<ActionResult<AuthenticateResponse>> Login([FromBody] LoginRequest login)
        {
            var response = Authenticate(login.Email, login.Password);

            if (response != null)
            {
                return response;
            }
            else
            {
                response = new AuthenticateResponse()
                {
                    Errors = new List<string> { "Неверный логин или пароль" }
                };

                return response;
            }
        }

        [HttpPost]
        [Route("Auth/Registration")]
        public async Task<ActionResult<AuthenticateResponse>> Registration([FromBody] RegistrationRequest registration)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == registration.Email);
            AuthenticateResponse response = new AuthenticateResponse();

            if (user != null)
            {
                response.Errors = new List<string> { "Такой пользователь уже существует" };
            }
            else if (IsValidEmail(registration.Email, response.Errors) && IsValidPassword(registration.Password, response.Errors))
            {
                var userDB = new User() { Email = registration.Email, Password = registration.Password, FullName = registration.FullName };

                _dbContext.Users.Add(userDB);
                _dbContext.Students.Add(new Student() { User = userDB });
                await _dbContext.SaveChangesAsync();

                response = Authenticate(registration.Email, registration.Password);
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
                if (IsValidPassword(passwordReset.Password, response.Errors) &&
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

        private AuthenticateResponse Authenticate(string email, string password)
        {
            var identity = GetIdentity(email, password);

            if (identity != null)
            {
                var now = DateTime.UtcNow;

                var jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        notBefore: now,
                        claims: identity?.Claims,
                        expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                var response = new AuthenticateResponse()
                {
                    Token = encodedJwt,
                    Email = identity.Name
                };
                return response;
            }
            else
            {
                return null;
            }
        }

        private ClaimsIdentity GetIdentity(string email, string password)
        {
            User user = _dbContext.Users.FirstOrDefault(x => x.Email == email && x.Password == password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    //new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            return null;
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
            if (password != null && password.Length >= 6)
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