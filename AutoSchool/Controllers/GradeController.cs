using AutoSchool.Data;
using AutoSchool.Extensions;
using AutoSchool.Models.Tables;
using AutoSchool.Models.Views;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoSchool.Controllers
{
    [ApiController]
    [Authorize(Roles = "Teacher")]
    public class GradeController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public GradeController(ILogger<UserController> logger,
            ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("Grade/GetAllGradesByCourse")]
        public async Task<ActionResult<IEnumerable<GradesResponse>>> GetAllGrades(long courseId)
        {
            var grades = new List<GradesResponse>();


            User? userDb = await _dbContext.Users.Include(x => x.Teacher)
                                                .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            Course? course = await _dbContext.Courses
                                                .Include(x => x.Themes)
                                                    .ThenInclude(x => x.Test)
                                                        .ThenInclude(x => x.ResultTests)
                                                .Include(x => x.StudentsCoursies)
                                                    .ThenInclude(x => x.Student)
                                                        .ThenInclude(x => x.User)
                                                .FirstOrDefaultAsync(x => x.Id == courseId);

            if (userDb != null && course != null)
            {
                grades = course.ConvertCourseToGradesResponse();
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            return grades;
        }

        [HttpGet]
        [Route("Grade/GetReportByCourse")]
        public async Task<ActionResult<IEnumerable<GradesResponse>>> GetReportByCourse(long courseId)
        {
            var grades = new List<GradesResponse>();

            User? userDb = await _dbContext.Users.Include(x => x.Teacher)
                                                .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            Course? course = await _dbContext.Courses
                                                .Include(x => x.Themes)
                                                    .ThenInclude(x => x.Test)
                                                        .ThenInclude(x => x.ResultTests)
                                                .Include(x => x.StudentsCoursies)
                                                    .ThenInclude(x => x.Student)
                                                        .ThenInclude(x => x.User)
                                                .FirstOrDefaultAsync(x => x.Id == courseId);

            if (userDb != null && course != null)
            {
                var courseGrades = course.ConvertCourseToGradesResponse();
                foreach(var grade in courseGrades)
                {
                    var results = grade.Grades.GroupBy(x => x.NameTheme).OrderBy(x => x.Key).ToList();

                    grade.Grades = new List<GradeResponse>();

                    foreach(var result in results)
                    {
                        var lastRightResult = result.OrderByDescending(x => x.Date).FirstOrDefault(x => x.Progress == 1);
                        var lastResult = result.OrderByDescending(x => x.Date).FirstOrDefault();

                        if (lastRightResult != null)
                        {
                            grade.Grades.Add(lastRightResult);
                        }
                        else
                        {
                            grade.Grades.Add(lastResult);
                        }
                    }
                    
                    grades.Add(grade);
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            return grades;
        }
    }
}