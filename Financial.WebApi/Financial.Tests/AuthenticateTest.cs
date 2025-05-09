using Financial.Domain.Dtos;
using Financial.Infra.Interfaces;
using Financial.Service;
using Microsoft.Extensions.Logging;
using Moq;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;

namespace Financial.Tests
{
    public class AuthenticateTest
    {

        [Fact]
        [Description("Deve criar um token")]
        public void DeveCriarUmToken()
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            var _logger = new Mock<ILogger<TokenService>>();

             
            var username = "master";
            var password = "master";
            var user = new UserDto
            {
                Id = Guid.CreateVersion7(),
                Username = username,
                Password = password,
                Role = "gerente"
            };

            mockUserRepository.Setup(repo => repo.Get(It.IsAny<string>(), It.IsAny<string>())).Returns(user);

            
            var tokenService = new TokenService(mockUserRepository.Object, _logger.Object);

            // Act
            var result = tokenService.GenerateToken(username, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(username, result.Item1);
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(result.Item2);
            Assert.Contains(token.Claims, c => c.Type == "role" && c.Value == user.Role);
        }
    }
}
