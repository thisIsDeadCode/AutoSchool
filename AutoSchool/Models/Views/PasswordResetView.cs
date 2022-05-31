namespace AutoSchool.Models.Views
{
    public class PasswordResetView
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public List<string> Errors { get; set; }
    }
}
