using AutoSchool.Models.Interfaces;

namespace AutoSchool.Models.Tables
{
    public class Course : IModelToSaveInVisitHistory
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }


        public long TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        public IEnumerable<Theme> Themes { get; set; }
        public IEnumerable<StudentsCourses> StudentsCoursies { get; set; }
        public IEnumerable<VisitHistory> Visits { get; set; }

    }
}
