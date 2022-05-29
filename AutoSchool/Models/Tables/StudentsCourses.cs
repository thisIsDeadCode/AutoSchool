namespace AutoSchool.Models.Tables
{
    public class StudentsCourses
    {
        public long StudentId { get; set; }
        public long CourseId { get; set; }
        public string Status { get; set; }
        public double Progress { get; set; }


        public Student Student { get; set; }
        public Course Course { get; set; }
    }
}
