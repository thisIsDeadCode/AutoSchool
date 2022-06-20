namespace AutoSchool.Models.Views
{
    public class GradeResponse
    {
        public long ThemeId { get; set; }
        public string NameTheme { get; set; }
        public double Progress { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
    }
}
