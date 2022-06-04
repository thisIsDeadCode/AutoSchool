using AutoSchool.Data;
using AutoSchool.Models.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoSchool.Extensions;
using AutoSchool.Models.Tables;
using AutoSchool.Services;
using Microsoft.AspNetCore.Authorization;

namespace AutoSchool.Controllers
{
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly HistoryService _historyService;

        public CourseController(ILogger<UserController> logger,
            HistoryService history,
            ApplicationDbContext dbContext)
        {
            _logger = logger;
            _historyService = history;
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("Course/GetAllCourses")]
        public async Task<ActionResult<IEnumerable<CourseResponse>>> GetAllCourses()
        {
            var courses = _dbContext.Courses
                .Include(t => t.Teacher)
                    .ThenInclude(u => u.User)
                .Include(st => st.StudentsCoursies.Take(5))
                    .ThenInclude(stc => stc.Student)
                        .ThenInclude(s => s.User)
                .ToList();

            var coursesView = new List<CourseResponse>();

            foreach (var course in courses)
            {
                coursesView.Add(course.ConvertCourseToCourseView());
            }

            User? userDb = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            if (userDb != null && User.Identity.IsAuthenticated)
            {
                coursesView.LoadProgressToCourses(_dbContext, userDb.Id);
            }

            return coursesView;
        }

        [Authorize]
        [HttpGet]
        [Route("Course/Get")]
        public async Task<ActionResult<CourseResponse>> Get(long Id)
        {
            Course? course = await _dbContext.Courses
                                             .Include(t => t.Teacher)
                                                .ThenInclude(u => u.User)
                                              .Include(st => st.StudentsCoursies)
                                                .ThenInclude(stc => stc.Student)
                                                  .ThenInclude(s => s.User)
                                              .FirstOrDefaultAsync(x=> x.Id == Id);

            User? userDb = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            if (course != null && userDb != null)
            {
                CourseResponse courseView = course.ConvertCourseToCourseView();
                courseView.LoadProgressToCourse(_dbContext, userDb.Id);

                await _historyService.SaveTohistory(userDb, course);

                return courseView;
            }
            else
            {
                return StatusCode(404);
            }
        }
    }
}