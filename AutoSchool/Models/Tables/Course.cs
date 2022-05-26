using System.ComponentModel.DataAnnotations;

namespace AutoSchool.Models.Tables
{
    public class Course
    {
        public long Id { get; set; }
        public string Name { get; set; }    
        public string Description { get; set; }


        public long TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        public IEnumerable<StudentsCourses> StudentsCoursies { get; set; }
    }
}
