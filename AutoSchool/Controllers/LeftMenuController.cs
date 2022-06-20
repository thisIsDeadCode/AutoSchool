using AutoSchool.Data;
using AutoSchool.Extensions;
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
    public class LeftMenuController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public LeftMenuController(ILogger<UserController> logger,
            ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("Button/GetHrefForButtons")]
        public async Task<ActionResult<LinksLeftMenuResponse>> GetHrefForButtons()
        {
            User? userDb = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            if (userDb != null && User.Identity.IsAuthenticated)
            {

                IQueryable<VisitHistory> query = _dbContext.VisitHistories
                                                .Include(x => x.Course)
                                                .Include(x => x.Theme)
                                                .Include(x => x.Lecture)
                                                .Include(x => x.Test)
                                                .Where(x => x.UserId == userDb.Id)
                                                .OrderByDescending(x => x.Date);

                List<VisitHistory> visitHistory = new List<VisitHistory>();
                visitHistory.AddRange(query.Where(x => x.CourseId != null).Take(3));
                visitHistory.AddRange(query.Where(x => x.ThemeId != null).Take(3));
                visitHistory.AddRange(query.Where(x => x.LectureId != null).Take(3));
                visitHistory.AddRange(query.Where(x => x.TestId != null).Take(3)); 


                return visitHistory.ConvertVisitHistoriesToLinksLeftMenuResponse();
            }
            else
            {
                return Ok();
            }
        }
    }
}