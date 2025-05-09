using Castle.Core.Logging;
using Financial.Domain.Dtos;
using Financial.Infra.Interfaces;
using Financial.Service;
using Microsoft.Extensions.Logging;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class TokenServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ILogger<TokenService>> _logger;
    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _logger = new Mock<ILogger<TokenService>>();
        _tokenService = new TokenService(_mockUserRepository.Object, _logger.Object );
    }

    [Fact]
    public void GenerateToken_ValidCredentials_ReturnsUsernameAndToken()
    {
        // Arrange
        var username = "testuser";
        var password = "testpassword";
        var user = new UserDto { Username = username, Password = password, Role = "User" };

        _mockUserRepository.Setup(repo => repo.Get(username, password)).Returns(user);

        // Act
        var result = _tokenService.GenerateToken(username, password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(username, result.Item1);
        Assert.NotNull(result.Item2);

        // Optional: Verify the token content (claims, expiration, signature) if needed for more detailed testing
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(result.Item2);
        
        Assert.Equal("User", token.Claims.FirstOrDefault(c => c.Type == "role")?.Value);
        
        Assert.True(token.ValidTo > DateTime.UtcNow);
    }

    [Fact]
    public void GenerateToken_InvalidCredentials_ThrowsApplicationException()
    {
        // Arrange
        var username = "wronguser";
        var password = "wrongpassword";

        _mockUserRepository.Setup(repo => repo.Get(username, password)).Returns((UserDto)null);

        // Act & Assert
        var exception = Assert.Throws<ApplicationException>(() => _tokenService.GenerateToken(username, password));
        Assert.Equal("Error: There is no user with informed credentials", exception.Message);
    }

    [Fact]
    public void GenerateToken_EmptyUsername_ThrowsApplicationException()
    {
        // Arrange
        var username = "";
        var password = "anypassword";

        _mockUserRepository.Setup(repo => repo.Get(username, password)).Returns((UserDto)null);

        // Act & Assert
        var exception = Assert.Throws<ApplicationException>(() => _tokenService.GenerateToken(username, password));
        Assert.Equal("Error: There is no user with informed credentials", exception.Message);
    }

    [Fact]
    public void GenerateToken_EmptyPassword_ThrowsApplicationException()
    {
        // Arrange
        var username = "anyuser";
        var password = "";

        _mockUserRepository.Setup(repo => repo.Get(username, password)).Returns((UserDto)null);

        // Act & Assert
        var exception = Assert.Throws<ApplicationException>(() => _tokenService.GenerateToken(username, password));
        Assert.Equal("Error: There is no user with informed credentials", exception.Message);
    }
}