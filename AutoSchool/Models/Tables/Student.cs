namespace AutoSchool.Models.Tables
{
    public class Student
    {
        public long UserId { get; set; }
        public User User { get; set; }

        public bool AccessToCourses { get; set; } = true;

        public IEnumerable<StudentsCourses> StudentsCoursies { get; set; }
        public IEnumerable<ResultTest> ResultTests { get; set; }
    }
}
