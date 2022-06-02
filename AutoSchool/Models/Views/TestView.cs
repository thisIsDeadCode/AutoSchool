namespace AutoSchool.Models.Views
{
    public class TestView
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public List<QuestionView> Questions { get; set; }
        public List<string> Errors { get; set; }
    }
}
