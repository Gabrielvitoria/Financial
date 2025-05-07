using Financial.Common.Dto;
using Financial.Service.Interfaces;
using Financial.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

public class AuthenticateControllerTests
{
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly AuthenticateController _controller;

    public AuthenticateControllerTests()
    {
        _mockTokenService = new Mock<ITokenService>();
        _controller = new AuthenticateController(_mockTokenService.Object);
    }

    [Fact]
    public async Task Authenticate_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var loginDto = new LoginUserDto { UserName = "testuser", Password = "testpassword" };
        var expectedToken = Tuple.Create("testuser", "mocked_token");

        _mockTokenService.Setup(service => service.GenerateToken(loginDto.UserName, loginDto.Password))
            .Returns(expectedToken);

        // Act
        var actionResult = await _controller.Authenticate(loginDto);
        var okResult = actionResult as OkObjectResult;
        var resultValue = okResult?.Value as dynamic;

        // Assert
        Assert.NotNull(okResult);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(resultValue);
    }

    [Fact]
    public async Task Authenticate_InvalidCredentials_ApplicationExceptionThrown_ReturnsNotFoundWithMessage()
    {
        // Arrange
        var loginDto = new LoginUserDto { UserName = "wronguser", Password = "wrongpassword" };
        var errorMessage = "Error: There is no user with informed credentials";

        _mockTokenService.Setup(service => service.GenerateToken(loginDto.UserName, loginDto.Password))
            .Throws(new ApplicationException(errorMessage));

        // Act
        var actionResult = await _controller.Authenticate(loginDto);
        var notFoundResult = actionResult as BadRequestObjectResult;

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task Authenticate_GenericExceptionThrown_ReturnsNotFoundWithMessage()
    {
        // Arrange
        var loginDto = new LoginUserDto { UserName = "testuser", Password = "errorpassword" };
        var errorMessage = "An unexpected error occurred during authentication.";

        _mockTokenService.Setup(service => service.GenerateToken(loginDto.UserName, loginDto.Password))
            .Throws(new Exception(errorMessage));

        // Act
        var actionResult = await _controller.Authenticate(loginDto);
        var notFoundResult = actionResult as BadRequestResult;

        // Assert
        Assert.NotNull(notFoundResult);
        Assert.Equal(StatusCodes.Status400BadRequest, notFoundResult.StatusCode);
        
    }

    [Fact]
    public async Task Authenticate_NullLoginDto_ReturnsBadRequest()
    {
        // Arrange
        LoginUserDto loginDto = null;

        // Act
        var actionResult = await _controller.Authenticate(loginDto);
        var badRequestResult = actionResult as BadRequestResult;

        // Assert
        Assert.NotNull(badRequestResult);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }
}