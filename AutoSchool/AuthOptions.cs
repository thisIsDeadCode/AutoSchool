using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AutoSchool
{
    public class AuthOptions
    {
        public const string ISSUER = "Auto-School";
        public const string AUDIENCE = "User";
        const string KEY = "asdqwrasdqweqwdqasd";
        public const int LIFETIME = 10080;
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
