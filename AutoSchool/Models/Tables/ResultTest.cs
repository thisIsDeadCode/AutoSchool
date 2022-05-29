namespace AutoSchool.Models.Tables
{
    public class ResultTest
    {
        public long Id { get; set; }
        public string Status { get; set; }
        public int? AmountRightQuestions { get; set; }
        public int? AmountWrongQuestions { get; set; }
        public double? TestResult { get; set; }
        public DateTime Date { get; set; }


        public long TestId { get; set; }
        public Test Test { get; set; }

        public long StudentUserId { get; set; }
        public Student Student { get; set; }

        public IEnumerable<Question> Questions { get; set; }
        public IEnumerable<Answer> Answers { get; set; }
    }
}
