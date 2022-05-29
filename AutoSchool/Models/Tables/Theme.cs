using AutoSchool.Models.Interfaces;

namespace AutoSchool.Models.Tables
{
    public class Theme : IModelToSaveInVisitHistory
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }


        public long CourseId { get; set; }
        public Course Course { get; set; }

        public Test Test { get; set; }

        public IEnumerable<Lecture> Lectures { get; set; }
        public IEnumerable<VisitHistory> Visits { get; set; }
    }
}
