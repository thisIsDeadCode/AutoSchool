using AutoSchool.Models.Interfaces;

namespace AutoSchool.Models.Tables
{
    public class Lecture : IModelToSaveInVisitHistory
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string TextHTML { get; set; }
        public string? Description { get; set; }


        public long ThemeId { get; set; }
        public Theme Theme { get; set; }
        public IEnumerable<VisitHistory> Visits { get; set; }
    }
}
