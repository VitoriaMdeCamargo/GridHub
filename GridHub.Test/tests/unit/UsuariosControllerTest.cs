using GridHub.API.Configuration;
using GridHub.API.Controllers;
using GridHub.Database.Models;
using GridHub.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tests.unit
{
    public class UsuarioControllerTests
    {
        private readonly Mock<IRepository<Usuario>> _mockRepository;
        private readonly UsuarioController _controller;

        public UsuarioControllerTests()
        {
            _mockRepository = new Mock<IRepository<Usuario>>();
            _controller = new UsuarioController(_mockRepository.Object);
        }
        [Fact]
        public async Task Get_ReturnsUsuario_WhenUsuarioExists()
        {
            // Arrange
            var usuarioId = 1;
            var usuario = new Usuario("Test User", "test@example.com")
            {
                UsuarioId = usuarioId,
                Nome = "Test User"
            };

            _mockRepository.Setup(repo => repo.GetById(usuarioId))
                .ReturnsAsync(usuario);

            // Act
            var result = await _controller.Get(usuarioId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<Usuario>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var response = Assert.IsType<ApiResponse<Usuario>>(okResult.Value);

            Assert.True(response.Success);
            Assert.Equal("Test User", response.Data.Nome);
        }


        [Fact]
        public async Task Get_ReturnsNotFound_WhenUsuarioDoesNotExist()
        {
            // Arrange
            var usuarioId = 1;
            _mockRepository.Setup(repo => repo.GetById(usuarioId)).ReturnsAsync((Usuario)null);

            // Act
            var result = await _controller.Get(usuarioId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<Usuario>>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            var response = Assert.IsType<ApiResponse<Usuario>>(notFoundResult.Value);

            Assert.False(response.Success);
            Assert.Equal("Usuário não encontrado.", response.Message);
        }

        [Fact]
        public async Task Post_CreatesUsuario_WithValidData()
        {
            // Arrange
            var novoUsuario = new Usuario("test@example.com", "senha123");
            novoUsuario.DefinirSenha("senha123");

            _mockRepository.Setup(repo => repo.Add(novoUsuario)) 
                .ReturnsAsync(novoUsuario); 

            // Act
            var result = await _controller.Post(novoUsuario);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<Usuario>>>(result);
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var response = Assert.IsType<ApiResponse<Usuario>>(createdResult.Value);

            Assert.True(response.Success);
            Assert.Equal("test@example.com", response.Data.Email);
        }



        [Fact]
        public async Task Post_ReturnsBadRequest_WhenUsuarioIsNull()
        {
            // Arrange
            Usuario usuario = null;

            // Act
            var result = await _controller.Post(usuario);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<Usuario>>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var response = Assert.IsType<ApiResponse<Usuario>>(badRequestResult.Value);

            Assert.False(response.Success);
            Assert.Equal("Dados inválidos.", response.Message);
        }

        [Fact]
        public async Task Put_ReturnsOk_WhenUsuarioIsUpdated()
        {
            // Arrange
            var idValido = 1;
            var usuarioExistente = new Usuario("oldemail@example.com", "oldpassword123")
            {
                UsuarioId = idValido,
                Nome = "Old Name",
                Telefone = "123456789"
            };

            var usuarioAtualizado = new Usuario("newemail@example.com", "newpassword123")
            {
                UsuarioId = idValido,
                Nome = "New Name",
                Telefone = "987654321"
            };

            _mockRepository.Setup(repo => repo.GetById(idValido)).ReturnsAsync(usuarioExistente);

            _mockRepository.Setup(repo => repo.Update(usuarioExistente)).ReturnsAsync(usuarioExistente);

            // Act
            var result = await _controller.Put(idValido, usuarioAtualizado);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<Usuario>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);

            var response = Assert.IsType<ApiResponse<Usuario>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Usuário atualizado com sucesso.", response.Message);
            Assert.Equal("New Name", response.Data.Nome);
            Assert.Equal("newemail@example.com", response.Data.Email);
        }



        [Fact]
        public async Task Put_ReturnsBadRequest_WhenUsuarioIsNullOrIdMismatch()
        {
            // Arrange
            var idInvalido = 999;
            var usuarioInvalido = new Usuario("test@example.com", "senha123");
            usuarioInvalido.UsuarioId = 1000; 

            // Act
            var result = await _controller.Put(idInvalido, usuarioInvalido);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<Usuario>>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);

            var response = Assert.IsType<ApiResponse<Usuario>>(badRequestResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Dados inválidos ou ID não corresponde ao usuário.", response.Message);
        }

        [Fact]
        public async Task Put_ReturnsNotFound_WhenUsuarioDoesNotExist()
        {
            // Arrange
            var idInvalido = 999; 
            var usuarioInvalido = new Usuario("test@example.com", "senha123");
            usuarioInvalido.UsuarioId = idInvalido;

            _mockRepository.Setup(repo => repo.GetById(idInvalido)).ReturnsAsync((Usuario)null);

            // Act
            var result = await _controller.Put(idInvalido, usuarioInvalido);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<Usuario>>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);

            var response = Assert.IsType<ApiResponse<Usuario>>(notFoundResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Usuário não encontrado.", response.Message);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenUsuarioIsDeleted()
        {
            // Arrange
            var idValido = 1; 
            var usuarioExistente = new Usuario("test@example.com", "senha123")
            {
                UsuarioId = idValido
            };

            _mockRepository.Setup(repo => repo.GetById(idValido)).ReturnsAsync(usuarioExistente);

            _mockRepository.Setup(repo => repo.Delete(usuarioExistente)).Verifiable();

            // Act
            var result = await _controller.Delete(idValido);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<object>>>(result);
            var noContentResult = Assert.IsType<NoContentResult>(actionResult.Result);

            _mockRepository.Verify(repo => repo.Delete(usuarioExistente), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenUsuarioDoesNotExist()
        {
            // Arrange
            var idInvalido = 999; 

            _mockRepository.Setup(repo => repo.GetById(idInvalido)).ReturnsAsync((Usuario)null);

            // Act
            var result = await _controller.Delete(idInvalido);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<object>>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);

            var response = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Usuário não encontrado.", response.Message);
        }

    }
}
