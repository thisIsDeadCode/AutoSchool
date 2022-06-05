namespace AutoSchool.Models.Views
{
    public class ThemeResponse : Response
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public long AmountLecture { get; set; }
        public DateTime? TestDate { get; set; }
        public bool IsActiveButtonLastResultTest { get; set; }
    }
}
