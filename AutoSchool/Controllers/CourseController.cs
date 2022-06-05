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

        [Authorize]
        [HttpGet]
        [Route("Course/GetCoursesByStatus")]
        public async Task<ActionResult<IEnumerable<CourseResponse>>> GetCoursesByStatus(string status)
        {
            List<CourseResponse> result = new List<CourseResponse>();

            if (status == "ongoing")
            {
                result = await GetCourses(true);
            }
            else if(status == "finished")
            {
                result = await GetCourses(false, true);
            }
            else
            {
                return BadRequest();
            }
            return result;
        }

        private async Task<List<CourseResponse>> GetCourses(bool ongoing = false, bool finished = false)
        {
            User? userDb = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
            List<CourseResponse> result = new List<CourseResponse>();

            if (userDb != null)
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

                coursesView.LoadProgressToCourses(_dbContext, userDb.Id);


                if(ongoing == true && finished == false)
                {
                    result = coursesView.Where(x => x.Progress > 0).ToList();
                }
                else if(ongoing == false && finished == true)
                {
                    result = coursesView.Where(x => x.Progress == 0).ToList();
                }
                else
                {
                    return null;
                }
            }
            return result;
        }
    }
}