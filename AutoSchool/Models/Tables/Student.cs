namespace AutoSchool.Models.Tables
{
    public class Student
    {
        public long UserId { get; set; }
        public User User { get; set; }

        public IEnumerable<StudentsCourses> StudentsCoursies { get; set; }
    }
}
