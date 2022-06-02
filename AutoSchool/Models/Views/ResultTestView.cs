namespace AutoSchool.Models.Views
{
    public class ResultTestView
    {
        public long Id { get; set; }
        public string Status { get; set; }
        public int AmountRightQuestions { get; set; }
        public int AmountWrongQuestions { get; set; }
        public double Result { get; set; }
        public DateTime Date { get; set; }
        public List<string> Errors { get; set; }
    }
}
