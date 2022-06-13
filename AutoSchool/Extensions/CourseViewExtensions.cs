using AutoSchool.Data;
using AutoSchool.Models.Tables;
using AutoSchool.Models.Views;

namespace AutoSchool.Extensions
{
    public static class CourseViewExtensions
    {
        public static void LoadProgressToCourses(this List<CourseResponse> courses, ApplicationDbContext dbContext,  long UserId)
        {
            List<StudentsCourses> studentsCourses = dbContext.StudentsCourses.Where(x=>x.StudentId == UserId).ToList();


            foreach (CourseResponse course in courses)
            {
                var studentCourse = studentsCourses.FirstOrDefault(ctc => ctc.CourseId == course.Id);
                course.Progress = studentCourse == null ? 0 : studentCourse.Progress;
                course.Status = studentCourse == null ? "Курс не начат" : studentCourse.Status;
            }
        }

        public static void LoadProgressToCourse(this CourseResponse course, ApplicationDbContext dbContext, long UserId)
        {
            StudentsCourses? studentCourse = dbContext.StudentsCourses.FirstOrDefault(x => x.StudentId == UserId && x.CourseId == course.Id);
            course.Progress = studentCourse == null ? 0 : studentCourse.Progress;
            course.Status = studentCourse == null ? "Курс не начат" : studentCourse.Status;
        }
    }
}
