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
    public class EspacoSystemTest : IClassFixture<WebApplicationFactory<GridHub.API.Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<GridHub.API.Program> _factory;

        public EspacoSystemTest(WebApplicationFactory<GridHub.API.Program> factory)
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
        public async Task Espaco_CRUD_ShouldWorkAsExpected()
        {
            // Criar um usuário inicial para vincular ao espaço
            var novoUsuario = new Usuario("Carlos Silva", "carlos.silva@example.com")
            {
                Nome = "Carlos Silva",
                Email = "carlos.silva@example.com",
                Telefone = "999999999",
                FotoPerfil = "foto_usuario.jpg",
                DataCriacao = DateTime.Now
            };

            novoUsuario.DefinirSenha("senha123");
            var usuarioResponse = await _client.PostAsJsonAsync("/api/usuario", novoUsuario);
            var usuarioCriado = (await usuarioResponse.Content.ReadFromJsonAsync<ApiResponse<Usuario>>())?.Data;

            Assert.NotNull(usuarioCriado);

            // 1. Criar um novo espaço (Create)
            var novoEspaco = new Espaco
            {
                NomeEspaco = "Espaço Solar",
                Endereco = "Rua das Flores, 123",
                UsuarioId = usuarioCriado.UsuarioId,
                FotoEspaco = "foto_espaco.jpg",
                FonteEnergia = "Solar",
                OrientacaoSolar = "Norte",
                MediaSolar = 4.5,
                Topografia = "Plano",
                AreaTotal = 250.0,
                DirecaoVento = "Sudeste",
                VelocidadeVento = 2.3
            };

            var createResponse = await _client.PostAsJsonAsync("/api/espaco", novoEspaco);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var createdEspacoResponse = await createResponse.Content.ReadFromJsonAsync<ApiResponse<Espaco>>();
            var createdEspaco = createdEspacoResponse?.Data;

            Assert.NotNull(createdEspaco);
            Assert.Equal("Espaço Solar", createdEspaco.NomeEspaco);

            // 2. Obter o espaço criado (Read)
            var getResponse = await _client.GetAsync($"/api/espaco/{createdEspaco.EspacoId}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var fetchedEspacoResponse = await getResponse.Content.ReadFromJsonAsync<ApiResponse<Espaco>>();
            var fetchedEspaco = fetchedEspacoResponse?.Data;

            Assert.NotNull(fetchedEspaco);
            Assert.Equal(createdEspaco.NomeEspaco, fetchedEspaco.NomeEspaco);

            // 3. Atualizar o espaço (Update)
            fetchedEspaco.NomeEspaco = "Espaço Solar Atualizado";
            fetchedEspaco.OrientacaoSolar = "Sul";

            var updateResponse = await _client.PutAsJsonAsync($"/api/espaco/{fetchedEspaco.EspacoId}", fetchedEspaco);
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

            var verifyUpdateResponse = await _client.GetAsync($"/api/espaco/{fetchedEspaco.EspacoId}");
            var updatedEspacoResponse = await verifyUpdateResponse.Content.ReadFromJsonAsync<ApiResponse<Espaco>>();
            var updatedEspaco = updatedEspacoResponse?.Data;

            Assert.Equal("Espaço Solar Atualizado", updatedEspaco.NomeEspaco);
            Assert.Equal("Sul", updatedEspaco.OrientacaoSolar);

            // 4. Deletar o espaço (Delete)
            var deleteResponse = await _client.DeleteAsync($"/api/espaco/{fetchedEspaco.EspacoId}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Verificar se o espaço foi realmente deletado
            var verifyDeleteResponse = await _client.GetAsync($"/api/espaco/{fetchedEspaco.EspacoId}");
            Assert.Equal(HttpStatusCode.NotFound, verifyDeleteResponse.StatusCode);
        }
    }
}
