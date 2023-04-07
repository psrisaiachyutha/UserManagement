using Business.Interfaces;
using Common.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Repository.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Business.Implementations
{
    public class JWTHelper : IJWTHelper
    {
        private readonly ApplicationSettings _applicationSettings;
        public JWTHelper(IOptions<ApplicationSettings> applicationSettings)
        {
            _applicationSettings = applicationSettings.Value;
        }
        
        public string GenerateJwtToken(User user)
        {

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
            };
            if (user.UserRoles.Any())
            {
                // Adding claims based on the roles the user having
                var userRoleClaims = user.UserRoles.Select(x => new Claim(ClaimTypes.Role, x.Role.Name)).ToList();

                claims.AddRange(userRoleClaims);
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_applicationSettings.SignatureKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }


        public bool VerifyPasswordHash(string password, string hash)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_applicationSettings.PasswordEncriptionKey));
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(Convert.FromBase64String(hash));
        }

        public string CreatePasswordHash(string password)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_applicationSettings.PasswordEncriptionKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hash);
        }

    }
}
