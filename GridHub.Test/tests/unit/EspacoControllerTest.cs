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
    public class EspacoControllerTests
    {
        private readonly Mock<IRepository<Espaco>> _mockRepository;
        private readonly Mock<IRepository<Usuario>> _mockUsuarioRepository;
        private readonly EspacoController _controller;

        public EspacoControllerTests()
        {
            _mockUsuarioRepository = new Mock<IRepository<Usuario>>();
            _mockRepository = new Mock<IRepository<Espaco>>();
            var mockUsuarioRepository = new Mock<IRepository<Usuario>>(); 
            _controller = new EspacoController(_mockRepository.Object, mockUsuarioRepository.Object); 
        }


        [Fact]
        public async Task Get_ReturnsEspaco_WhenEspacoExists()
        {
            // Arrange
            var espacoId = 1;
            var espaco = new Espaco
            {
                EspacoId = espacoId,
                NomeEspaco = "Espaco Teste",
                Endereco = "Rua Fictícia, 123, Bairro Fictício",
                FonteEnergia = "Energia Solar",
                OrientacaoSolar = "Sul",
                MediaSolar = 4.5,
                Topografia = "Terreno plano e regular",
                AreaTotal = 5000.0,
                DirecaoVento = "Vento predominante do norte",
                VelocidadeVento = 15.0
            };

            _mockRepository.Setup(repo => repo.GetById(espacoId))
                .ReturnsAsync(espaco);

            // Act
            var result = await _controller.Get(espacoId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<Espaco>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var response = Assert.IsType<ApiResponse<Espaco>>(okResult.Value);

            Assert.True(response.Success);
            Assert.Equal("Espaco Teste", response.Data.NomeEspaco);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenEspacoDoesNotExist()
        {
            // Arrange
            var espacoId = 1;
            _mockRepository.Setup(repo => repo.GetById(espacoId)).ReturnsAsync((Espaco)null);

            // Act
            var result = await _controller.Get(espacoId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<Espaco>>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            var response = Assert.IsType<ApiResponse<Espaco>>(notFoundResult.Value);

            Assert.False(response.Success);
            Assert.Equal("Espaço não encontrado.", response.Message);
        }

        [Fact]
        public async Task Post_CreatesEspaco_WithValidData()
        {
            // Arrange
            var novoEspaco = new Espaco
            {
                UsuarioId = 1, 
                NomeEspaco = "Novo Espaco",
                Endereco = "Rua Nova, 456, Bairro Novo",
                FotoEspaco = "foto_padrao.jpg",
                FonteEnergia = "Energia Solar",
                OrientacaoSolar = "Sul",
                MediaSolar = 4.5,
                Topografia = "Terreno plano",
                AreaTotal = 3000.0,
                DirecaoVento = "Vento predominante do leste",
                VelocidadeVento = 10.0
            };

            _mockRepository.Setup(repo => repo.Add(novoEspaco))
                .ReturnsAsync(novoEspaco);

            var mockUsuarioRepository = new Mock<IRepository<Usuario>>();
            mockUsuarioRepository.Setup(repo => repo.GetById(1)) 
                .ReturnsAsync(new Usuario("usuario@teste.com", "Usuário Teste"));

            var controller = new EspacoController(_mockRepository.Object, mockUsuarioRepository.Object);

            // Act
            var result = await controller.Post(novoEspaco);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<Espaco>>>(result);
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var response = Assert.IsType<ApiResponse<Espaco>>(createdResult.Value);

            Assert.True(response.Success);
            Assert.Equal("Novo Espaco", response.Data.NomeEspaco);
            Assert.Equal(1, response.Data.UsuarioId);
        }


        [Fact]
        public async Task Post_ReturnsBadRequest_WhenEspacoIsNull()
        {
            // Arrange
            Espaco espaco = null;

            // Act
            var result = await _controller.Post(espaco);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<Espaco>>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var response = Assert.IsType<ApiResponse<Espaco>>(badRequestResult.Value);

            Assert.False(response.Success);
            Assert.Equal("Dados inválidos.", response.Message);
        }

        [Fact]
        public async Task Put_ReturnsOk_WhenEspacoIsUpdated()
        {
            // Arrange
            var idValido = 1;
            var espacoExistente = new Espaco
            {
                EspacoId = idValido,
                NomeEspaco = "Espaco Antigo",
                Endereco = "Rua Antiga, 789, Bairro Antigo",
                FonteEnergia = "Energia Solar",
                OrientacaoSolar = "Sul",
                MediaSolar = 4.5,
                Topografia = "Terreno plano",
                AreaTotal = 3000.0,
                DirecaoVento = "Vento predominante do oeste",
                VelocidadeVento = 12.0
            };

            var espacoAtualizado = new Espaco
            {
                EspacoId = idValido,
                NomeEspaco = "Espaco Atualizado",
                Endereco = "Rua Atualizada, 1010, Bairro Atualizado",
                FonteEnergia = "Energia Solar",
                OrientacaoSolar = "Sul",
                MediaSolar = 5.0,
                Topografia = "Terreno regular",
                AreaTotal = 3500.0,
                DirecaoVento = "Vento predominante do sul",
                VelocidadeVento = 14.0
            };

            _mockRepository.Setup(repo => repo.GetById(idValido)).ReturnsAsync(espacoExistente);
            _mockRepository.Setup(repo => repo.Update(espacoExistente)).ReturnsAsync(espacoAtualizado);

            // Act
            var result = await _controller.Put(idValido, espacoAtualizado);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<Espaco>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var response = Assert.IsType<ApiResponse<Espaco>>(okResult.Value);

            Assert.True(response.Success);
            Assert.Equal("Espaço atualizado com sucesso.", response.Message);
            Assert.Equal("Espaco Atualizado", response.Data.NomeEspaco);
        }

        [Fact]
        public async Task Put_ReturnsNotFound_WhenEspacoDoesNotExist()
        {
            // Arrange
            var idInvalido = 999;
            var espacoInvalido = new Espaco
            {
                EspacoId = idInvalido,
                NomeEspaco = "Espaco Não Existente",
                Endereco = "Rua Não Existente, 999, Bairro Não Existente",
                FonteEnergia = "Energia Solar",
                OrientacaoSolar = "Sul",
                MediaSolar = 4.5,
                Topografia = "Terreno plano",
                AreaTotal = 5000.0,
                DirecaoVento = "Vento predominante do norte",
                VelocidadeVento = 15.0
            };

            _mockRepository.Setup(repo => repo.GetById(idInvalido)).ReturnsAsync((Espaco)null);

            // Act
            var result = await _controller.Put(idInvalido, espacoInvalido);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<Espaco>>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            var response = Assert.IsType<ApiResponse<Espaco>>(notFoundResult.Value);

            Assert.False(response.Success);
            Assert.Equal("Espaço não encontrado.", response.Message);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenEspacoIsDeleted()
        {
            // Arrange
            var idValido = 1;
            var espacoExistente = new Espaco
            {
                EspacoId = idValido,
                NomeEspaco = "Espaco a Ser Deletado",
                Endereco = "Rua Fictícia, 123, Bairro Fictício",
                FonteEnergia = "Energia Solar",
                OrientacaoSolar = "Sul",
                MediaSolar = 4.5,
                Topografia = "Terreno plano",
                AreaTotal = 5000.0,
                DirecaoVento = "Vento predominante do norte",
                VelocidadeVento = 15.0
            };

            _mockRepository.Setup(repo => repo.GetById(idValido)).ReturnsAsync(espacoExistente);
            _mockRepository.Setup(repo => repo.Delete(espacoExistente)).Verifiable();

            // Act
            var result = await _controller.Delete(idValido);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<object>>>(result);
            var noContentResult = Assert.IsType<NoContentResult>(actionResult.Result);

            _mockRepository.Verify(repo => repo.Delete(espacoExistente), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenEspacoDoesNotExist()
        {
            // Arrange
            var idInvalido = 999;
            _mockRepository.Setup(repo => repo.GetById(idInvalido)).ReturnsAsync((Espaco)null);

            // Act
            var result = await _controller.Delete(idInvalido);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<object>>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            var response = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);

            Assert.False(response.Success);
            Assert.Equal("Espaço não encontrado.", response.Message);
        }
    }
}
