namespace AutoSchool.Models.Views
{
    public class RegistrationView
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public List<string> Errors { get; set; }
    }
}
