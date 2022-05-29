namespace AutoSchool.Models.Tables
{
    public class VisitHistory
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }


        public long? CourseId { get; set; }
        public Course? Course { get; set; }
        public long? ThemeId { get; set; }
        public Theme? Theme { get; set; }
        public long? LectureId { get; set; }
        public Lecture? Lecture { get; set; }
        public long? TestId { get; set; }
        public Test? Test { get; set; }
    }
}
