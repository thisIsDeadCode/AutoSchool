namespace AutoSchool.Models.Views
{
    public class GradesResponse : Response
    {
        public long CourseId { get; set; }
        public long UserId { get; set; }
        public string NameCourse { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        public List<GradeResponse> Grades { get; set; }
    }
}
