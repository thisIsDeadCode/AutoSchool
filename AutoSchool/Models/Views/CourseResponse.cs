namespace AutoSchool.Models.Views
{
    public class CourseResponse : Response
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public double Progress { get; set; }
        public string Description { get; set; }
        public long AmountThemes { get; set; }
        public long AmountLecture { get; set; }

        public TeacherResponse Teacher { get; set; }
        public IEnumerable<StudentResponse> Students { get; set; }
    }
}
