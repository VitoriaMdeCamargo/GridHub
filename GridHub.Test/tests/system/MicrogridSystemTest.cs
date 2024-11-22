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
    public class MicrogridSystemTest : IClassFixture<WebApplicationFactory<GridHub.API.Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<GridHub.API.Program> _factory;

        public MicrogridSystemTest(WebApplicationFactory<GridHub.API.Program> factory)
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
        public async Task Microgrid_CRUD_ShouldWorkAsExpected()
        {
            // Criar um usuário inicial para vincular à microgrid
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

            // Criar um espaço para a microgrid (necessário para a microgrid)
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

            var espacoResponse = await _client.PostAsJsonAsync("/api/espaco", novoEspaco);
            var espacoCriado = (await espacoResponse.Content.ReadFromJsonAsync<ApiResponse<Espaco>>())?.Data;

            Assert.NotNull(espacoCriado);

            // 1. Criar uma nova microgrid (Create)
            var novaMicrogrid = new Microgrid
            {
                NomeMicrogrid = "Microgrid Solar",
                UsuarioId = usuarioCriado.UsuarioId,
                EspacoId = espacoCriado.EspacoId,
                FotoMicrogrid = "foto_microgrid.jpg",
                RadiacaoSolarNecessaria = 5.0,
                TopografiaNecessaria = "Plano",
                AreaTotalNecessaria = 250.0,
                VelocidadeVentoNecessaria = 3.0,
                FonteEnergia = "Solar",
                MetaFinanciamento = 100000.0
            };

            var createResponse = await _client.PostAsJsonAsync("/api/microgrid", novaMicrogrid);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var createdMicrogridResponse = await createResponse.Content.ReadFromJsonAsync<ApiResponse<Microgrid>>();
            var createdMicrogrid = createdMicrogridResponse?.Data;

            Assert.NotNull(createdMicrogrid);
            Assert.Equal("Microgrid Solar", createdMicrogrid.NomeMicrogrid);

            // 2. Obter a microgrid criada (Read)
            var getResponse = await _client.GetAsync($"/api/microgrid/{createdMicrogrid.MicrogridId}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var fetchedMicrogridResponse = await getResponse.Content.ReadFromJsonAsync<ApiResponse<Microgrid>>();
            var fetchedMicrogrid = fetchedMicrogridResponse?.Data;

            Assert.NotNull(fetchedMicrogrid);
            Assert.Equal(createdMicrogrid.NomeMicrogrid, fetchedMicrogrid.NomeMicrogrid);

            // 3. Atualizar a microgrid (Update)
            fetchedMicrogrid.NomeMicrogrid = "Microgrid Solar Atualizada";
            fetchedMicrogrid.RadiacaoSolarNecessaria = 6.0;

            var updateResponse = await _client.PutAsJsonAsync($"/api/microgrid/{fetchedMicrogrid.MicrogridId}", fetchedMicrogrid);
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

            var verifyUpdateResponse = await _client.GetAsync($"/api/microgrid/{fetchedMicrogrid.MicrogridId}");
            var updatedMicrogridResponse = await verifyUpdateResponse.Content.ReadFromJsonAsync<ApiResponse<Microgrid>>();
            var updatedMicrogrid = updatedMicrogridResponse?.Data;

            Assert.Equal("Microgrid Solar Atualizada", updatedMicrogrid.NomeMicrogrid);
            Assert.Equal(6.0, updatedMicrogrid.RadiacaoSolarNecessaria);

            // 4. Deletar a microgrid (Delete)
            var deleteResponse = await _client.DeleteAsync($"/api/microgrid/{fetchedMicrogrid.MicrogridId}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Verificar se a microgrid foi realmente deletada
            var verifyDeleteResponse = await _client.GetAsync($"/api/microgrid/{fetchedMicrogrid.MicrogridId}");
            Assert.Equal(HttpStatusCode.NotFound, verifyDeleteResponse.StatusCode);
        }
    }
}
