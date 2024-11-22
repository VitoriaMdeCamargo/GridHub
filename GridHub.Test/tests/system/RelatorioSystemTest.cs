using System;
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

namespace GridHub.API.Tests
{
    public class RelatorioSystemTest : IClassFixture<WebApplicationFactory<GridHub.API.Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<GridHub.API.Program> _factory;

        public RelatorioSystemTest(WebApplicationFactory<GridHub.API.Program> factory)
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
        public async Task Relatorio_CRUD_ShouldWorkAsExpected()
        {
            // Criar um usuário inicial
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

            // Criar um espaço
            var novoEspaco = new Espaco
            {
                UsuarioId = usuarioCriado.UsuarioId,
                Endereco = "Rua Fictícia, 123, Bairro Fictício",
                NomeEspaco = "Espaço Solar Premium",
                FotoEspaco = "foto_espaco_padrao.jpg",
                FonteEnergia = "Energia Solar",
                OrientacaoSolar = "Sul",
                MediaSolar = 4.5,
                Topografia = "Terreno plano e regular",
                AreaTotal = 5000.0,
                DirecaoVento = "Vento predominante do norte",
                VelocidadeVento = 15.0
            };

            var espacoResponse = await _client.PostAsJsonAsync("/api/espaco", novoEspaco);
            var espacoCriado = (await espacoResponse.Content.ReadFromJsonAsync<ApiResponse<Espaco>>())?.Data;

            Assert.NotNull(espacoCriado);

            // Criar uma microgrid
            var novaMicrogrid = new Microgrid
            {
                UsuarioId = usuarioCriado.UsuarioId,
                EspacoId = espacoCriado.EspacoId,
                NomeMicrogrid = "Microgrid Solar",
                FotoMicrogrid = "foto_microgrid.jpg",
                RadiacaoSolarNecessaria = 5.0,
                TopografiaNecessaria = "Terreno plano",
                AreaTotalNecessaria = 3000.0,
                VelocidadeVentoNecessaria = 10.0,
                FonteEnergia = "Energia Solar",
                MetaFinanciamento = 50000.0
            };

            var microgridResponse = await _client.PostAsJsonAsync("/api/microgrid", novaMicrogrid);
            var microgridCriada = (await microgridResponse.Content.ReadFromJsonAsync<ApiResponse<Microgrid>>())?.Data;

            Assert.NotNull(microgridCriada);

            // 1. Criar um novo relatório (Create)
            var novoRelatorio = new Relatorio
            {
                MicrogridId = microgridCriada.MicrogridId,
                EnergiaGerada = 1000.0,
                TempPainelSolar = 25.0,
                LucroGerado = 500.0
            };

            var createResponse = await _client.PostAsJsonAsync("/api/relatorio", novoRelatorio);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var createdRelatorioResponse = await createResponse.Content.ReadFromJsonAsync<ApiResponse<Relatorio>>();
            var createdRelatorio = createdRelatorioResponse?.Data;

            Assert.NotNull(createdRelatorio);
            Assert.Equal(1000.0, createdRelatorio.EnergiaGerada);
            Assert.Equal(25.0, createdRelatorio.TempPainelSolar);
            Assert.Equal(500.0, createdRelatorio.LucroGerado);

            // 2. Obter o relatório criado (Read)
            var getResponse = await _client.GetAsync($"/api/relatorio/{createdRelatorio.RelatorioId}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var fetchedRelatorioResponse = await getResponse.Content.ReadFromJsonAsync<ApiResponse<Relatorio>>();
            var fetchedRelatorio = fetchedRelatorioResponse?.Data;

            Assert.NotNull(fetchedRelatorio);
            Assert.Equal(createdRelatorio.EnergiaGerada, fetchedRelatorio.EnergiaGerada);
            Assert.Equal(createdRelatorio.TempPainelSolar, fetchedRelatorio.TempPainelSolar);
            Assert.Equal(createdRelatorio.LucroGerado, fetchedRelatorio.LucroGerado);

            // 3. Atualizar o relatório (Update)
            fetchedRelatorio.EnergiaGerada = 1200.0;
            fetchedRelatorio.TempPainelSolar = 30.0;
            fetchedRelatorio.LucroGerado = 600.0;

            var updateResponse = await _client.PutAsJsonAsync($"/api/relatorio/{fetchedRelatorio.RelatorioId}", fetchedRelatorio);
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

            var verifyUpdateResponse = await _client.GetAsync($"/api/relatorio/{fetchedRelatorio.RelatorioId}");
            var updatedRelatorioResponse = await verifyUpdateResponse.Content.ReadFromJsonAsync<ApiResponse<Relatorio>>();
            var updatedRelatorio = updatedRelatorioResponse?.Data;

            Assert.Equal(1200.0, updatedRelatorio.EnergiaGerada);
            Assert.Equal(30.0, updatedRelatorio.TempPainelSolar);
            Assert.Equal(600.0, updatedRelatorio.LucroGerado);

            // 4. Deletar o relatório (Delete)
            var deleteResponse = await _client.DeleteAsync($"/api/relatorio/{fetchedRelatorio.RelatorioId}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Verificar se o relatório foi realmente deletado
            var verifyDeleteResponse = await _client.GetAsync($"/api/relatorio/{fetchedRelatorio.RelatorioId}");
            Assert.Equal(HttpStatusCode.NotFound, verifyDeleteResponse.StatusCode);
        }
    }
}
