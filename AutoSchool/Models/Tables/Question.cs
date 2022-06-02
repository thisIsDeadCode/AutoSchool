namespace AutoSchool.Models.Tables
{
    public class Question
    {
        public long Id { get; set; }
        public long QuestionImageId { get; set; }
        public string QuestionText { get; set; }


        public long TestId { get; set; }
        public Test Test { get; set; }

        public IEnumerable<Answer> Answers { get; set; }
        public IEnumerable<QuestionAnswers> QuestionAnswers { get; set; }
    }
}
