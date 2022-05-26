namespace AutoSchool.Models.Views
{
    public class CourseView
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double Progress { get; set; }
        public string Description { get; set; }

        public TeacherView Teacher { get; set; }
        public IEnumerable<StudentView> Students { get; set; }
        public List<string> Errors { get; set; }
    }
}
