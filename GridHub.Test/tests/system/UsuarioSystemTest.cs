using System.Net;
using System.Net.Http.Json;
using GridHub.Database.Models;
using GridHub.API.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Xunit;
using GridHub.Database;
using System.Linq;
using System.Threading.Tasks;

namespace test.system
{
    public class UsuarioSystemTest : IClassFixture<WebApplicationFactory<GridHub.API.Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<GridHub.API.Program> _factory;

        public UsuarioSystemTest(WebApplicationFactory<GridHub.API.Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Configurando o banco de dados em memória para os testes de sistema
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<FIAPDBContext>));

                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<FIAPDBContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDB_System");
                    });
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Usuario_CRUD_ShouldWorkAsExpected()
        {
            // 1. Criar um novo usuário (Create)
            var novoUsuario = new Usuario("Maria Oliveira", "maria.oliveira@example.com")
            {
                Nome = "Maria Oliveira",
                Email = "maria.oliveira@example.com",
                Telefone = "987654321",
                FotoPerfil = "foto.jpg",
                DataCriacao = DateTime.Now
            };

            // Chamar método para definir a senha corretamente
            novoUsuario.DefinirSenha("senha123");

            var createResponse = await _client.PostAsJsonAsync("/api/usuario", novoUsuario);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var createdUsuarioResponse = await createResponse.Content.ReadFromJsonAsync<ApiResponse<Usuario>>();
            var createdUsuario = createdUsuarioResponse?.Data;
            Assert.NotNull(createdUsuario);
            Assert.Equal("Maria Oliveira", createdUsuario.Nome);
            Assert.NotEqual(0, createdUsuario.UsuarioId); 

            // 2. Obter o usuário criado (Read)
            var getResponse = await _client.GetAsync($"/api/usuario/{createdUsuario.UsuarioId}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var fetchedUsuarioResponse = await getResponse.Content.ReadFromJsonAsync<ApiResponse<Usuario>>();
            var fetchedUsuario = fetchedUsuarioResponse?.Data;
            Assert.NotNull(fetchedUsuario);
            Assert.Equal(createdUsuario.Nome, fetchedUsuario.Nome);

            // 3. Atualizar o usuário (Update)
            fetchedUsuario.Telefone = "912345678";
            fetchedUsuario.FotoPerfil = "nova_foto.jpg";

            var updateResponse = await _client.PutAsJsonAsync($"/api/usuario/{fetchedUsuario.UsuarioId}", fetchedUsuario);
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

            // Verificar se o usuário foi atualizado
            var verifyUpdateResponse = await _client.GetAsync($"/api/usuario/{fetchedUsuario.UsuarioId}");
            var updatedUsuarioResponse = await verifyUpdateResponse.Content.ReadFromJsonAsync<ApiResponse<Usuario>>();
            var updatedUsuario = updatedUsuarioResponse?.Data;
            Assert.Equal("912345678", updatedUsuario.Telefone);
            Assert.Equal("nova_foto.jpg", updatedUsuario.FotoPerfil);

            // 4. Deletar o usuário (Delete)
            var deleteResponse = await _client.DeleteAsync($"/api/usuario/{fetchedUsuario.UsuarioId}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Verificar se o usuário foi realmente deletado
            var verifyDeleteResponse = await _client.GetAsync($"/api/usuario/{fetchedUsuario.UsuarioId}");
            Assert.Equal(HttpStatusCode.NotFound, verifyDeleteResponse.StatusCode);
        }
    }
}
