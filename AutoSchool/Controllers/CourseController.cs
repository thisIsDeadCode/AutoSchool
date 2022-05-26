using AutoSchool.Data;
using AutoSchool.Models.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoSchool.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CourseController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public CourseController(ILogger<UserController> logger,
            ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet(Name = "GetCourses")]
        public IEnumerable<CourseView> Get()
        {
             var courses = _dbContext.Courses
                .Include(t => t.Teacher)    
                    .ThenInclude(u=>u.User)
                .Include(st => st.StudentsCoursies)
                    .ThenInclude(stc => stc.Student)
                        .ThenInclude(s => s.User)
                .ToList();

            var coursesView = new List<CourseView>();

            foreach (var course in courses)
            {
                var courseView = new CourseView()
                {
                    Id = course.Id,
                    Name = course.Name,
                    Teacher = new TeacherView()
                    {
                        Id = course.Teacher.UserId,
                        UserName = course.Teacher.User.UserName,
                        Email = course.Teacher.User.Email,
                        FullName = course.Teacher.User.FullName,
                        Location = course.Teacher.User.Location,
                        PhoneNumber = course.Teacher.User.PhoneNumber,
                        PhotoId = null,
                        FacebookURL = course.Teacher.User.FacebookURL,
                        UserNameInstagram = course.Teacher.User.UserNameInstagram,
                        UserNameTelegram = course.Teacher.User.UserNameTelegram,
                        UserNameTwitter = course.Teacher.User.UserNameTwitter,
                    },
                    Description = course.Description,
                    Students = course.StudentsCoursies.Select(s => new StudentView()
                    {
                        Id = s.Student.UserId,
                        UserName = s.Student.User.UserName,
                        Email = s.Student.User.Email,
                        FullName = s.Student.User.FullName,
                        Location = s.Student.User.Location,
                        PhoneNumber = s.Student.User.PhoneNumber,
                        PhotoId = null,
                        FacebookURL = s.Student.User.FacebookURL,
                        UserNameInstagram = s.Student.User.UserNameInstagram,
                        UserNameTelegram = s.Student.User.UserNameTelegram,
                        UserNameTwitter = s.Student.User.UserNameTwitter,
                    })
                };

                coursesView.Add(courseView);
            }


            return coursesView;
        }
    }
}