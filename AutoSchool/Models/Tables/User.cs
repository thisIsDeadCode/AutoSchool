namespace AutoSchool.Models.Tables
{
    public class User
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public  string Email { get; set; }
        public  bool EmailConfirmed { get; set; }
        public  string Password { get; set; }
        public string? Location { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserNameTelegram { get; set; }
        public string? FacebookURL { get; set; }
        public string? UserNameInstagram { get; set; }
        public string? UserNameTwitter { get; set; }
        public string? PhotoId { get; set; }


        public Student Student { get; set; }
        public Teacher Teacher { get; set; }
        public IEnumerable<File> Files { get; set; }
    }
}
