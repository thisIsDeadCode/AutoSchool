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
        [Route("Button/GetButtonsForLeftMenu")]
        public async Task<ActionResult<SectionsForLeftMenuResponse>> GetButtonsForLeftMenu()
        {
            User? userDb = await _dbContext.Users.Include(x => x.Teacher)
                                                .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            var myCourses = _dbContext.Courses.Include(x => x.Teacher).ToList();
            var myCoursesForTeacher = myCourses.Where(x => x.Teacher.UserId == userDb.Id).ToList();

            var isTeacher = userDb.Teacher == null ? false : true;

            var sections = new SectionsForLeftMenuResponse();
            sections.Sections = new List<SectionResponse>();

            if (userDb != null && User.Identity.IsAuthenticated)
            {
                if(isTeacher)
                {
                    var gradeSection = new SectionResponse()
                    {
                        Name = "Оценки",
                        Buttons = new List<LinkResponse>()
                    };

                    foreach(var myCourse in myCoursesForTeacher)
                    {
                        gradeSection.Buttons.Add(new LinkResponse()
                        {
                            Title = myCourse.Name,
                            Href = $"Grade/GetReport?Id={myCourse.Id}"
                        });
                    }
                    sections.Sections.Add(gradeSection);
                }

                var myCoursesSection = new SectionResponse()
                {
                    Name = "Мои курсы",
                    Buttons = new List<LinkResponse>()
                };

                foreach (var myCourse in myCourses)
                {
                    myCoursesSection.Buttons.Add(new LinkResponse()
                    {
                        Title = myCourse.Name,
                        Href = $"Course/Get?Id={myCourse.Id}"
                    });
                }

                sections.Sections.Add(myCoursesSection);

                return sections;
            }
            else
            {
                return Ok();
            }
        }
    }
}