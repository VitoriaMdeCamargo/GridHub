using GridHub.API.Configuration;
using GridHub.API.Controllers;
using GridHub.Database.Models;
using GridHub.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

public class RelatorioControllerTest
{
    private readonly Mock<IRepository<Relatorio>> _mockRelatorioRepository;
    private readonly Mock<IRepository<Microgrid>> _mockMicrogridRepository;
    private readonly RelatorioController _controller;

    public RelatorioControllerTest()
    {
        // Mocking repositories
        _mockRelatorioRepository = new Mock<IRepository<Relatorio>>();
        _mockMicrogridRepository = new Mock<IRepository<Microgrid>>();

        // Injetando mocks no controlador
        _controller = new RelatorioController(_mockRelatorioRepository.Object, _mockMicrogridRepository.Object);
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenRelatorioDoesNotExist()
    {
        // Arrange
        int relatorioId = 1;
        _mockRelatorioRepository.Setup(repo => repo.GetById(relatorioId)).ReturnsAsync((Relatorio)null);

        // Act
        var result = await _controller.Get(relatorioId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<Relatorio>>>(result);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<Relatorio>>(notFoundResult.Value);
        Assert.Equal("Relatório não encontrado.", response.Message);
    }

    [Fact]
    public async Task Post_ReturnsBadRequest_WhenRelatorioIsNull()
    {
        // Arrange
        Relatorio relatorio = null;

        // Act
        var result = await _controller.Post(relatorio);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<Relatorio>>>(result);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<Relatorio>>(badRequestResult.Value);
        Assert.Equal("Dados inválidos.", response.Message);
    }

    [Fact]
    public async Task Post_ReturnsCreatedAtAction_WhenRelatorioIsCreated()
    {
        // Arrange
        var relatorio = new Relatorio
        {
            MicrogridId = 1,
            EnergiaGerada = 100,
            TempPainelSolar = 30,
            LucroGerado = 200
        };

        var microgrid = new Microgrid { MicrogridId = 1 };
        _mockMicrogridRepository.Setup(repo => repo.GetById(relatorio.MicrogridId)).ReturnsAsync(microgrid);
        _mockRelatorioRepository.Setup(repo => repo.Add(It.IsAny<Relatorio>())).ReturnsAsync(relatorio);

        // Act
        var result = await _controller.Post(relatorio);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<Relatorio>>>(result);
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<Relatorio>>(createdAtActionResult.Value);
        Assert.Equal("Relatório criado com sucesso.", response.Message);
    }

    [Fact]
    public async Task Put_ReturnsBadRequest_WhenIdsDoNotMatch()
    {
        // Arrange
        int relatorioId = 1;
        var relatorio = new Relatorio
        {
            RelatorioId = 2, 
            EnergiaGerada = 200,
            TempPainelSolar = 35,
            LucroGerado = 300
        };

        // Act
        var result = await _controller.Put(relatorioId, relatorio);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<Relatorio>>>(result);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<Relatorio>>(badRequestResult.Value);
        Assert.Equal("Dados inválidos ou ID não corresponde ao relatório.", response.Message);
    }

    [Fact]
    public async Task Put_ReturnsNotFound_WhenRelatorioDoesNotExist()
    {
        // Arrange
        int relatorioId = 1;
        var relatorio = new Relatorio
        {
            RelatorioId = relatorioId,
            EnergiaGerada = 200,
            TempPainelSolar = 35,
            LucroGerado = 300
        };
        _mockRelatorioRepository.Setup(repo => repo.GetById(relatorioId)).ReturnsAsync((Relatorio)null);

        // Act
        var result = await _controller.Put(relatorioId, relatorio);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<Relatorio>>>(result);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<Relatorio>>(notFoundResult.Value);
        Assert.Equal("Relatório não encontrado.", response.Message);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenRelatorioDoesNotExist()
    {
        // Arrange
        int relatorioId = 1;
        _mockRelatorioRepository.Setup(repo => repo.GetById(relatorioId)).ReturnsAsync((Relatorio)null);

        // Act
        var result = await _controller.Delete(relatorioId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<object>>>(result);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        var response = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
        Assert.Equal("Relatório não encontrado.", response.Message);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenRelatorioIsDeleted()
    {
        // Arrange
        int relatorioId = 1;
        var relatorio = new Relatorio
        {
            RelatorioId = relatorioId,
            EnergiaGerada = 100,
            TempPainelSolar = 25,
            LucroGerado = 150
        };
        _mockRelatorioRepository.Setup(repo => repo.GetById(relatorioId)).ReturnsAsync(relatorio);
        _mockRelatorioRepository.Setup(repo => repo.Delete(It.IsAny<Relatorio>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(relatorioId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<object>>>(result);
        Assert.IsType<NoContentResult>(actionResult.Result);
    }
}
