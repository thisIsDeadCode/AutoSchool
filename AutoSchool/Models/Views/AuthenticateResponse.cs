using AutoSchool.Models.Tables;

namespace AutoSchool.Models.Views
{
    public class AuthenticateResponse : Response
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
