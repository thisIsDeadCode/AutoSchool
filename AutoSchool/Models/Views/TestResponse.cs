namespace AutoSchool.Models.Views
{
    public class TestResponse : Response
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public List<QuestionResponse> Questions { get; set; }
    }
}
