namespace AutoSchool.Models.Views
{
    public class AnswersToQuestionView
    {
        public long QuestionId { get; set; }
        public string QuestionText { get; set; }

        public List<AnswerView> Answers { get; set; }
    }
}
