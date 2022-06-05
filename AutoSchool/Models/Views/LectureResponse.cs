namespace AutoSchool.Models.Views
{
    public class LectureResponse : Response
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string TextHTML { get; set; }
        public string? Description { get; set; }
        public bool HasPrevious { get; set; }
        public bool NasNext { get; set; }
    }
}
