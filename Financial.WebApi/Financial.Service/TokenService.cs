using Financial.Common;
using Financial.Infra.Interfaces;
using Financial.Service.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Financial.Service
{
    public class TokenService : ITokenService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IUserRepository userRepository, ILogger<TokenService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public Tuple<string, string> GenerateToken(string username, string password)
        {
            var user = _userRepository.Get(username, password);

            if (user == null)
            {
                _logger.LogWarning($"User not found: {username}");
                throw new ApplicationException($"Error: There is no user with informed credentials");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Settings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            _logger.LogInformation($"Token generated for user: {user.Username}");
            return Tuple.Create<string, string>(user.Username, tokenHandler.WriteToken(token));
        }
    }
}
