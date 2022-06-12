namespace AutoSchool.Models.Views
{
    public class QuestionResponse
    {
        public long Id { get; set; }
        public long? QuestionImageId { get; set; }
        public string QuestionText { get; set; }

        public List<AnswerResponce> Answers { get; set; }
    }
}
