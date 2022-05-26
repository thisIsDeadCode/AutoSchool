namespace AutoSchool.Models.Tables
{
    public class StudentsCourses
    {
        public long StudentId { get; set; }
        public long CourseId { get; set; }
        public float Progress { get; set; }


        public Student Student { get; set; }
        public Course Course { get; set; }
    }
}
