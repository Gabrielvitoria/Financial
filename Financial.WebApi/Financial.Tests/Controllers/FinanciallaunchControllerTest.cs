using Financial.Domain.Dtos;
using Financial.Service.Interfaces;
using Financial.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

public class FinancialControllerTests
{
    private readonly Mock<IProcessLaunchservice> _mockProcessLaunchService;
    private readonly FinancialController _controller;

    public FinancialControllerTests()
    {
        _mockProcessLaunchService = new Mock<IProcessLaunchservice>();
        _controller = new FinancialController(_mockProcessLaunchService.Object);
    }

    [Fact]
    public async Task NewFinancialLaunchAsync_ValidInput_ReturnsOkWithResult()
    {
        // Arrange
        var createFinanciallaunchDto = new CreateFinanciallaunchDto { /* Propriedades do DTO */ };
        var expectedResult = new FinanciallaunchDto { /* Propriedades do DTO */ };
       
        _mockProcessLaunchService.Setup(service => service.ProcessNewLaunchAsync(createFinanciallaunchDto))
            .ReturnsAsync(expectedResult);

        // Act
        var actionResult = await _controller.NewFinancialLaunchAsync(createFinanciallaunchDto);
        var okResult = actionResult as OkObjectResult;

        // Assert
        Assert.NotNull(okResult);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task NewFinancialLaunchAsync_ApplicationExceptionThrown_ReturnsNotFoundWithMessage()
    {
        // Arrange
        var createFinanciallaunchDto = new CreateFinanciallaunchDto { /* Propriedades do DTO */ };
        var errorMessage = "Erro de aplicação simulado.";

        _mockProcessLaunchService.Setup(service => service.ProcessNewLaunchAsync(createFinanciallaunchDto))
            .ThrowsAsync(new ApplicationException(errorMessage));

        // Act
        var actionResult = await _controller.NewFinancialLaunchAsync(createFinanciallaunchDto);
        var notFoundResult = actionResult as NotFoundObjectResult;

        // Assert
        Assert.NotNull(notFoundResult);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        Assert.Equal(errorMessage, notFoundResult.Value);
    }

    [Fact]
    public async Task NewFinancialLaunchAsync_GenericExceptionThrown_ThrowsException()
    {
        // Arrange
        var createFinanciallaunchDto = new CreateFinanciallaunchDto { /* Propriedades do DTO */ };
        var exceptionMessage = "Erro genérico simulado.";

        _mockProcessLaunchService.Setup(service => service.ProcessNewLaunchAsync(createFinanciallaunchDto))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _controller.NewFinancialLaunchAsync(createFinanciallaunchDto));
    }

    //   Adicionar mais testes para diferentes cenários, como:
    // - Validação falhando (se houver lógica de validação no controller)
    // - Diferentes tipos de exceções lançadas pelo serviço
    // - Testar o comportamento com dados de entrada nulos ou inválidos (se o controller tratar isso explicitamente)
}