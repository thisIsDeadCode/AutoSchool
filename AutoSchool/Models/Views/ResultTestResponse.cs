namespace AutoSchool.Models.Views
{
    public class ResultTestResponse : Response
    {
        public long Id { get; set; }
        public string Status { get; set; }
        public int AmountRightQuestions { get; set; }
        public int AmountWrongQuestions { get; set; }
        public double Result { get; set; }
        public DateTime Date { get; set; }
    }
}
