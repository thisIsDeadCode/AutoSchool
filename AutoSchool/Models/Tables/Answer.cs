namespace AutoSchool.Models.Tables
{
    public class Answer
    {
        public long Id { get; set; }
        public bool IsRight { get; set; }
        public string TextAnswer { get; set; }

        public long QuestionId { get; set; }
        public Question Question { get; set; }
    }
}
