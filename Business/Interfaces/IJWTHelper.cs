using Repository.Models.Entities;

namespace Business.Interfaces
{
    public interface IJWTHelper
    {
        public string GenerateJwtToken(User user);
        public string CreatePasswordHash(string password);
        public bool VerifyPasswordHash(string password, string hash);
    }
}
