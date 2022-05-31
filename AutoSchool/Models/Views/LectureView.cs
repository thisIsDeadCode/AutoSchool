namespace AutoSchool.Models.Views
{
    public class LectureView
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string TextHTML { get; set; }
        public string? Description { get; set; }
        public List<string> Errors { get; set; }
    }
}
