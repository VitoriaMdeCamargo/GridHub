using GridHub.API.Configuration;
using GridHub.API.Controllers;
using GridHub.Database.Models;
using GridHub.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GridHub.API.Tests
{
    public class InvestimentoControllerTests
    {
        private readonly Mock<IRepository<Investimento>> _mockInvestimentoRepository;
        private readonly Mock<IRepository<Usuario>> _mockUsuarioRepository;
        private readonly Mock<IRepository<Microgrid>> _mockMicrogridRepository;
        private readonly InvestimentoController _controller;

        public InvestimentoControllerTests()
        {
            _mockInvestimentoRepository = new Mock<IRepository<Investimento>>();
            _mockUsuarioRepository = new Mock<IRepository<Usuario>>();
            _mockMicrogridRepository = new Mock<IRepository<Microgrid>>();
            _controller = new InvestimentoController(
                _mockInvestimentoRepository.Object,
                _mockUsuarioRepository.Object,
                _mockMicrogridRepository.Object
            );
        }

        [Fact]
        public async Task Get_ReturnsInvestimento_WhenFound()
        {
            // Arrange
            var investimentoId = 1;
            var mockInvestimento = new Investimento
            {
                InvestimentoId = investimentoId,
                UsuarioId = 1,
                MicrogridId = 1,
                DescricaoProposta = "Investimento na Microgrid"
            };
            _mockInvestimentoRepository.Setup(repo => repo.GetById(investimentoId)).ReturnsAsync(mockInvestimento);

            // Act
            var result = await _controller.Get(investimentoId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<Investimento>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var response = Assert.IsType<ApiResponse<Investimento>>(okResult.Value);
            Assert.Equal("Investimento na Microgrid", response.Data.DescricaoProposta);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenInvestimentoNotFound()
        {
            // Arrange
            var investimentoId = 999;
            _mockInvestimentoRepository.Setup(repo => repo.GetById(investimentoId)).ReturnsAsync((Investimento)null);

            // Act
            var result = await _controller.Get(investimentoId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<Investimento>>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            var response = Assert.IsType<ApiResponse<Investimento>>(notFoundResult.Value);
            Assert.Equal("Investimento não encontrado.", response.Message);
        }

        [Fact]
        public async Task Post_CreatesInvestimento_WhenValidData()
        {
            // Arrange
            var investimento = new Investimento
            {
                UsuarioId = 1,
                MicrogridId = 1,
                DescricaoProposta = "Investimento na Microgrid"
            };
            var mockUsuario = new Usuario("Nome do Usuário", "usuario@email.com");
            var mockMicrogrid = new Microgrid { MicrogridId = 1 };

            _mockUsuarioRepository.Setup(repo => repo.GetById(investimento.UsuarioId)).ReturnsAsync(mockUsuario);
            _mockMicrogridRepository.Setup(repo => repo.GetById(investimento.MicrogridId)).ReturnsAsync(mockMicrogrid);
            _mockInvestimentoRepository.Setup(repo => repo.Add(It.IsAny<Investimento>())).ReturnsAsync(investimento);

            // Act
            var result = await _controller.Post(investimento);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<Investimento>>>(result);
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var response = Assert.IsType<ApiResponse<Investimento>>(createdResult.Value);
            Assert.Equal("Investimento criado com sucesso.", response.Message);
        }

        [Fact]
        public async Task Post_ReturnsBadRequest_WhenUsuarioNotFound()
        {
            // Arrange
            var investimento = new Investimento
            {
                UsuarioId = 1,
                MicrogridId = 1,
                DescricaoProposta = "Investimento na Microgrid"
            };
            _mockUsuarioRepository.Setup(repo => repo.GetById(investimento.UsuarioId)).ReturnsAsync((Usuario)null);

            // Act
            var result = await _controller.Post(investimento);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<Investimento>>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var response = Assert.IsType<ApiResponse<Investimento>>(badRequestResult.Value);
            Assert.Equal("Usuário não encontrado.", response.Message);
        }

        [Fact]
        public async Task Put_UpdatesInvestimento_WhenValidData()
        {
            // Arrange
            var investimentoId = 1;
            var investimento = new Investimento
            {
                InvestimentoId = investimentoId,
                UsuarioId = 1,
                MicrogridId = 1,
                DescricaoProposta = "Investimento atualizado"
            };
            var mockInvestimento = new Investimento
            {
                InvestimentoId = investimentoId,
                UsuarioId = 1,
                MicrogridId = 1,
                DescricaoProposta = "Investimento antigo"
            };

            _mockInvestimentoRepository.Setup(repo => repo.GetById(investimentoId)).ReturnsAsync(mockInvestimento);
            _mockInvestimentoRepository.Setup(repo => repo.Update(It.IsAny<Investimento>())).ReturnsAsync(investimento);

            // Act
            var result = await _controller.Put(investimentoId, investimento);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<Investimento>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var response = Assert.IsType<ApiResponse<Investimento>>(okResult.Value);
            Assert.Equal("Investimento atualizado com sucesso.", response.Message);
        }

        [Fact]
        public async Task Delete_RemovesInvestimento_WhenFound()
        {
            // Arrange
            var investimentoId = 1;
            var mockInvestimento = new Investimento
            {
                InvestimentoId = investimentoId,
                UsuarioId = 1,
                MicrogridId = 1,
                DescricaoProposta = "Investimento para remoção"
            };
            _mockInvestimentoRepository.Setup(repo => repo.GetById(investimentoId)).ReturnsAsync(mockInvestimento);
            _mockInvestimentoRepository.Setup(repo => repo.Delete(mockInvestimento)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(investimentoId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<object>>>(result);
            var noContentResult = Assert.IsType<NoContentResult>(actionResult.Result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenInvestimentoNotFound()
        {
            // Arrange
            var investimentoId = 999;
            _mockInvestimentoRepository.Setup(repo => repo.GetById(investimentoId)).ReturnsAsync((Investimento)null);

            // Act
            var result = await _controller.Delete(investimentoId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<object>>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            var response = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Investimento não encontrado.", response.Message);
        }
    }
}
