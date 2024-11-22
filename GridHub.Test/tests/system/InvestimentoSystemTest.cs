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
    public class InvestimentoSystemTest : IClassFixture<WebApplicationFactory<GridHub.API.Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<GridHub.API.Program> _factory;

        public InvestimentoSystemTest(WebApplicationFactory<GridHub.API.Program> factory)
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
        public async Task Investimento_CRUD_ShouldWorkAsExpected()
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
                EspacoId = espacoCriado.EspacoId,  // A relação entre microgrid e espaço
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

            // 1. Criar um novo investimento (Create)
            var novoInvestimento = new Investimento
            {
                UsuarioId = usuarioCriado.UsuarioId,
                MicrogridId = microgridCriada.MicrogridId,
                DescricaoProposta = "Investimento em microgrid solar"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/investimento", novoInvestimento);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var createdInvestimentoResponse = await createResponse.Content.ReadFromJsonAsync<ApiResponse<Investimento>>();
            var createdInvestimento = createdInvestimentoResponse?.Data;

            Assert.NotNull(createdInvestimento);
            Assert.Equal("Investimento em microgrid solar", createdInvestimento.DescricaoProposta);

            // 2. Obter o investimento criado (Read)
            var getResponse = await _client.GetAsync($"/api/investimento/{createdInvestimento.InvestimentoId}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var fetchedInvestimentoResponse = await getResponse.Content.ReadFromJsonAsync<ApiResponse<Investimento>>();
            var fetchedInvestimento = fetchedInvestimentoResponse?.Data;

            Assert.NotNull(fetchedInvestimento);
            Assert.Equal(createdInvestimento.DescricaoProposta, fetchedInvestimento.DescricaoProposta);

            // 3. Atualizar o investimento (Update)
            fetchedInvestimento.DescricaoProposta = "Investimento atualizado na microgrid solar";

            var updateResponse = await _client.PutAsJsonAsync($"/api/investimento/{fetchedInvestimento.InvestimentoId}", fetchedInvestimento);
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

            var verifyUpdateResponse = await _client.GetAsync($"/api/investimento/{fetchedInvestimento.InvestimentoId}");
            var updatedInvestimentoResponse = await verifyUpdateResponse.Content.ReadFromJsonAsync<ApiResponse<Investimento>>();
            var updatedInvestimento = updatedInvestimentoResponse?.Data;

            Assert.Equal("Investimento atualizado na microgrid solar", updatedInvestimento.DescricaoProposta);

            // 4. Deletar o investimento (Delete)
            var deleteResponse = await _client.DeleteAsync($"/api/investimento/{fetchedInvestimento.InvestimentoId}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Verificar se o investimento foi realmente deletado
            var verifyDeleteResponse = await _client.GetAsync($"/api/investimento/{fetchedInvestimento.InvestimentoId}");
            Assert.Equal(HttpStatusCode.NotFound, verifyDeleteResponse.StatusCode);
        }
    }
}
