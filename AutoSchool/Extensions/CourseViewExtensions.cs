using AutoSchool.Models.Views;

namespace AutoSchool.Extensions
{
    public static class CourseViewExtensions
    {
        public static void LoadProgressToCourses(this List<CourseView> courses)
        {
            var rnd = new Random();    
            foreach(CourseView course in courses)
            {
                course.Progress = rnd.Next(0, 100);
            }
        }
    }
}
