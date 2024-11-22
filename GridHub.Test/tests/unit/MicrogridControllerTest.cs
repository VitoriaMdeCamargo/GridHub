using GridHub.API.Configuration;
using GridHub.API.Controllers;
using GridHub.Database.Models;
using GridHub.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace tests.unit
{
    public class MicrogridControllerTests
    {
        private readonly Mock<IRepository<Microgrid>> _mockRepository;
        private readonly Mock<IRepository<Usuario>> _mockUsuarioRepository;
        private readonly Mock<IRepository<Espaco>> _mockEspacoRepository;
        private readonly MicrogridController _controller;

        public MicrogridControllerTests()
        {
            _mockRepository = new Mock<IRepository<Microgrid>>();
            _mockUsuarioRepository = new Mock<IRepository<Usuario>>();
            _mockEspacoRepository = new Mock<IRepository<Espaco>>();
            _controller = new MicrogridController(_mockRepository.Object, _mockUsuarioRepository.Object, _mockEspacoRepository.Object);
        }

        [Fact]
        public async Task Get_ReturnsMicrogrid_WhenMicrogridExists()
        {
            // Arrange
            var microgridId = 1;
            var microgrid = new Microgrid
            {
                MicrogridId = microgridId,
                UsuarioId = 1,
                EspacoId = 1,
                NomeMicrogrid = "Microgrid Teste",
                FotoMicrogrid = "foto_microgrid_test.jpg",
                RadiacaoSolarNecessaria = 5.0,
                TopografiaNecessaria = "Terreno plano",
                AreaTotalNecessaria = 4000.0,
                VelocidadeVentoNecessaria = 12.0,
                FonteEnergia = "Solar",
                MetaFinanciamento = 100000.0
            };

            _mockRepository.Setup(repo => repo.GetById(microgridId))
                .ReturnsAsync(microgrid);

            // Act
            var result = await _controller.Get(microgridId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<Microgrid>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var response = Assert.IsType<ApiResponse<Microgrid>>(okResult.Value);

            Assert.True(response.Success);
            Assert.Equal("Microgrid Teste", response.Data.NomeMicrogrid);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenMicrogridDoesNotExist()
        {
            // Arrange
            var microgridId = 1;
            _mockRepository.Setup(repo => repo.GetById(microgridId)).ReturnsAsync((Microgrid)null);

            // Act
            var result = await _controller.Get(microgridId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<Microgrid>>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            var response = Assert.IsType<ApiResponse<Microgrid>>(notFoundResult.Value);

            Assert.False(response.Success);
            Assert.Equal("Microgrid não encontrada.", response.Message);
        }

        [Fact]
        public async Task Post_CreatesMicrogrid_WithValidData()
        {
            // Arrange
            var novoMicrogrid = new Microgrid
            {
                UsuarioId = 1,
                EspacoId = 1,
                NomeMicrogrid = "Novo Microgrid",
                FotoMicrogrid = "foto_microgrid_novo.jpg",
                RadiacaoSolarNecessaria = 5.5,
                TopografiaNecessaria = "Terreno leve",
                AreaTotalNecessaria = 3500.0,
                VelocidadeVentoNecessaria = 15.0,
                FonteEnergia = "Eólica",
                MetaFinanciamento = 120000.0
            };

            _mockRepository.Setup(repo => repo.Add(novoMicrogrid))
                .ReturnsAsync(novoMicrogrid);

            var mockUsuarioRepository = new Mock<IRepository<Usuario>>();
            mockUsuarioRepository.Setup(repo => repo.GetById(1)) 
                .ReturnsAsync(new Usuario("usuario@teste.com", "Usuário Teste"));

            var mockEspacoRepository = new Mock<IRepository<Espaco>>();
            mockEspacoRepository.Setup(repo => repo.GetById(1)) 
                .ReturnsAsync(new Espaco { EspacoId = 1, NomeEspaco = "Espaço Teste" });

            var controller = new MicrogridController(_mockRepository.Object, mockUsuarioRepository.Object, mockEspacoRepository.Object);

            // Act
            var result = await controller.Post(novoMicrogrid);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<Microgrid>>>(result);
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var response = Assert.IsType<ApiResponse<Microgrid>>(createdResult.Value);

            Assert.True(response.Success);
            Assert.Equal("Novo Microgrid", response.Data.NomeMicrogrid);
            Assert.Equal(1, response.Data.UsuarioId);
        }

        [Fact]
        public async Task Post_ReturnsBadRequest_WhenMicrogridIsNull()
        {
            // Arrange
            Microgrid microgrid = null;

            // Act
            var result = await _controller.Post(microgrid);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<Microgrid>>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var response = Assert.IsType<ApiResponse<Microgrid>>(badRequestResult.Value);

            Assert.False(response.Success);
            Assert.Equal("Dados inválidos.", response.Message);
        }

        [Fact]
        public async Task Put_ReturnsOk_WhenMicrogridIsUpdated()
        {
            // Arrange
            var idValido = 1;
            var microgridExistente = new Microgrid
            {
                MicrogridId = idValido,
                UsuarioId = 1,
                EspacoId = 1,
                NomeMicrogrid = "Microgrid Antigo",
                FotoMicrogrid = "foto_microgrid_antigo.jpg",
                RadiacaoSolarNecessaria = 4.0,
                TopografiaNecessaria = "Terreno plano",
                AreaTotalNecessaria = 3000.0,
                VelocidadeVentoNecessaria = 10.0,
                FonteEnergia = "Solar",
                MetaFinanciamento = 50000.0
            };

            var microgridAtualizado = new Microgrid
            {
                MicrogridId = idValido,
                UsuarioId = 1,
                EspacoId = 1,
                NomeMicrogrid = "Microgrid Atualizado",
                FotoMicrogrid = "foto_microgrid_atualizado.jpg",
                RadiacaoSolarNecessaria = 5.0,
                TopografiaNecessaria = "Terreno leve",
                AreaTotalNecessaria = 3500.0,
                VelocidadeVentoNecessaria = 12.0,
                FonteEnergia = "Eólica",
                MetaFinanciamento = 70000.0
            };

            _mockRepository.Setup(repo => repo.GetById(idValido)).ReturnsAsync(microgridExistente);
            _mockRepository.Setup(repo => repo.Update(microgridExistente)).ReturnsAsync(microgridAtualizado);

            // Act
            var result = await _controller.Put(idValido, microgridAtualizado);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<Microgrid>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var response = Assert.IsType<ApiResponse<Microgrid>>(okResult.Value);

            Assert.True(response.Success);
            Assert.Equal("Microgrid atualizada com sucesso.", response.Message);
            Assert.Equal("Microgrid Atualizado", response.Data.NomeMicrogrid);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenMicrogridIsDeleted()
        {
            // Arrange
            var idValido = 1;
            var microgridExistente = new Microgrid
            {
                MicrogridId = idValido,
                UsuarioId = 1,
                EspacoId = 1,
                NomeMicrogrid = "Microgrid a Ser Deletado",
                FotoMicrogrid = "foto_microgrid_deletar.jpg",
                RadiacaoSolarNecessaria = 5.0,
                TopografiaNecessaria = "Terreno plano",
                AreaTotalNecessaria = 3000.0,
                VelocidadeVentoNecessaria = 10.0,
                FonteEnergia = "Solar",
                MetaFinanciamento = 50000.0
            };

            _mockRepository.Setup(repo => repo.GetById(idValido)).ReturnsAsync(microgridExistente);
            _mockRepository.Setup(repo => repo.Delete(microgridExistente)).Verifiable();

            // Act
            var result = await _controller.Delete(idValido);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<object>>>(result);
            var noContentResult = Assert.IsType<NoContentResult>(actionResult.Result);

            _mockRepository.Verify(repo => repo.Delete(microgridExistente), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenMicrogridDoesNotExist()
        {
            // Arrange
            var microgridId = 1;
            _mockRepository.Setup(repo => repo.GetById(microgridId)).ReturnsAsync((Microgrid)null);

            // Act
            var result = await _controller.Delete(microgridId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<object>>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            var response = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);

            Assert.False(response.Success);
            Assert.Equal("Microgrid não encontrada.", response.Message);
        }
    }
}
