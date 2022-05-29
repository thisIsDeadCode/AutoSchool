namespace AutoSchool.Models.Tables
{
    public class Lecture
    {
        public long Id { get; set; }
        public string TextHTML { get; set; }

        public long ThemId { get; set; }
        public Theme Theme { get; set; }
    }
}
