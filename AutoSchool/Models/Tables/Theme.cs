namespace AutoSchool.Models.Tables
{
    public class Theme
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string? Description { get; set; }
        public DateTime? LastVisit { get; set; }


        public long CourseId { get; set; }
        public Course Course { get; set; }

        public Test Test { get; set; }

        public IEnumerable<Lecture> Lectures { get; set; }
    }
}
