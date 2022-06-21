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
    [Authorize]
    public class GradeController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public GradeController(ILogger<UserController> logger,
            ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _logger = logger;
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("Grade/GetAllGradesByCourse")]
        public async Task<ActionResult<IEnumerable<GradesResponse>>> GetAllGrades(long courseId)
        {
            var grades = new List<GradesResponse>();


            User? userDb = await _dbContext.Users.Include(x => x.Teacher)
                                                .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
            var isTeacher = userDb.Teacher == null ? false : true;

            Course? course = await _dbContext.Courses
                                                .Include(x => x.Themes)
                                                    .ThenInclude(x => x.Test)
                                                        .ThenInclude(x => x.ResultTests)
                                                .Include(x => x.StudentsCoursies)
                                                    .ThenInclude(x => x.Student)
                                                        .ThenInclude(x => x.User)
                                                .FirstOrDefaultAsync(x => x.Id == courseId);

            if (userDb != null && course != null && isTeacher)
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
        [Route("Grade/GetReport")]
        public async Task<ActionResult<IEnumerable<GradesResponse>>> GetReport(long courseId)
        {
            var grades = new List<GradesResponse>();

            User? userDb = await _dbContext.Users.Include(x => x.Teacher)
                                                .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
            var isTeacher = userDb.Teacher == null ? false : true;

            Course? course = await _dbContext.Courses
                                                .Include(x => x.Themes)
                                                    .ThenInclude(x => x.Test)
                                                        .ThenInclude(x => x.ResultTests)
                                                .Include(x => x.StudentsCoursies)
                                                    .ThenInclude(x => x.Student)
                                                        .ThenInclude(x => x.User)
                                                .FirstOrDefaultAsync(x => x.Id == courseId);

            if (userDb != null && course != null && isTeacher)
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