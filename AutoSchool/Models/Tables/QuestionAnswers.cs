namespace AutoSchool.Models.Tables
{
    public class QuestionAnswers
    {
        public long Id { get; set; }

        public long ResultTestId { get; set; }
        public ResultTest ResultTest { get; set; }

        public long QuestionId { get; set; }
        public Question Question { get; set; }

        public long AnswerId { get; set; }
        public Answer Answer { get; set; }

    }
}
