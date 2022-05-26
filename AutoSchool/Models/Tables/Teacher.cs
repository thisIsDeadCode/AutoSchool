namespace AutoSchool.Models.Tables
{
    public class Teacher
    {
        public long UserId { get; set; }
        public User User { get; set; }

        public IEnumerable<Course> Courses { get; set; }
    }
}
