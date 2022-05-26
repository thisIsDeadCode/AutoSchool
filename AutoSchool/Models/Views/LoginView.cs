namespace AutoSchool.Models.Views
{
    public class LoginView
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public List<string> Errors { get; set; }
    }
}
