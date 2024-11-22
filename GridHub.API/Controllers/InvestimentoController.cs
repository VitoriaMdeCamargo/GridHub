using GridHub.Database.Models;
using GridHub.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GridHub.API.Configuration;

namespace GridHub.API.Controllers
{
    /// <summary>
    /// Controlador para gerenciar as operações CRUD dos investimentos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class InvestimentoController : ControllerBase
    {
        private readonly IRepository<Investimento> _investimentoRepository;
        private readonly IRepository<Usuario> _usuarioRepository;
        private readonly IRepository<Microgrid> _microgridRepository;

        public InvestimentoController(IRepository<Investimento> investimentoRepository,
                                      IRepository<Usuario> usuarioRepository,
                                      IRepository<Microgrid> microgridRepository)
        {
            _investimentoRepository = investimentoRepository ?? throw new ArgumentNullException(nameof(investimentoRepository));
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            _microgridRepository = microgridRepository ?? throw new ArgumentNullException(nameof(microgridRepository));
        }

        /// <summary>
        /// Obtém um investimento específico pelo ID.
        /// </summary>
        /// <param name="id">ID do investimento.</param>
        /// <returns>Investimento solicitado.</returns>
        /// <response code="200">Retorna o investimento solicitado.</response>
        /// <response code="404">Investimento não encontrado.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Investimento>>> Get(int id)
        {
            var investimento = await Task.Run(() => _investimentoRepository.GetById(id));

            if (investimento == null)
            {
                return NotFound(ApiResponse<Investimento>.ErrorResponse("Investimento não encontrado."));
            }

            return Ok(ApiResponse<Investimento>.SuccessResponse(investimento));
        }

        /// <summary>
        /// Obtém todos os investimentos cadastrados.
        /// </summary>
        /// <returns>Lista de investimentos.</returns>
        /// <response code="200">Retorna a lista de investimentos.</response>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Investimento>>>> GetAll()
        {
            var investimentos = await Task.Run(() => _investimentoRepository.GetAll());

            var investimentosList = investimentos.ToList();

            if (investimentosList == null || investimentosList.Count == 0)
            {
                return NotFound(ApiResponse<List<Investimento>>.ErrorResponse("Nenhum investimento encontrado."));
            }

            return Ok(ApiResponse<List<Investimento>>.SuccessResponse(investimentosList));
        }

        /// <summary>
        /// Adiciona um novo investimento ao sistema.
        /// </summary>
        /// <param name="investimento">Objeto contendo os dados do investimento a ser criado.</param>
        /// <returns>Retorna um objeto de resposta contendo o status da operação.</returns>
        /// <exception cref="ArgumentNullException">Lançada se o parâmetro 'investimento' for nulo.</exception>
        /// <response code="201">Retorna o investimento criado.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Investimento>>> Post([FromBody] Investimento investimento)
        {
            if (investimento == null)
            {
                return BadRequest(ApiResponse<Investimento>.ErrorResponse("Dados inválidos."));
            }

            var usuario = await Task.Run(() => _usuarioRepository.GetById(investimento.UsuarioId));
            if (usuario == null)
            {
                return BadRequest(ApiResponse<Investimento>.ErrorResponse("Usuário não encontrado."));
            }

            var microgrid = await Task.Run(() => _microgridRepository.GetById(investimento.MicrogridId));
            if (microgrid == null)
            {
                return BadRequest(ApiResponse<Investimento>.ErrorResponse("Microgrid não encontrada."));
            }

            Investimento novoInvestimento = new Investimento
            {
                UsuarioId = investimento.UsuarioId,
                MicrogridId = investimento.MicrogridId,
                DescricaoProposta = investimento.DescricaoProposta
            };

            await Task.Run(() => _investimentoRepository.Add(novoInvestimento));

            return CreatedAtAction(nameof(Get), new { id = novoInvestimento.InvestimentoId }, ApiResponse<Investimento>.SuccessResponse(novoInvestimento, "Investimento criado com sucesso."));
        }

        /// <summary>
        /// Atualiza um investimento existente.
        /// </summary>
        /// <param name="id">ID do investimento a ser atualizado.</param>
        /// <param name="investimento">Objeto investimento com os novos dados.</param>
        /// <returns>Investimento atualizado.</returns>
        /// <response code="200">Retorna o investimento atualizado.</response>
        /// <response code="400">Dados inválidos ou ID não corresponde ao investimento.</response>
        /// <response code="404">Investimento não encontrado.</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Investimento>>> Put(int id, [FromBody] Investimento investimento)
        {
            if (investimento == null || id != investimento.InvestimentoId)
            {
                return BadRequest(ApiResponse<Investimento>.ErrorResponse("Dados inválidos ou ID não corresponde ao investimento."));
            }

            var investimentoExistente = await Task.Run(() => _investimentoRepository.GetById(id));
            if (investimentoExistente == null)
            {
                return NotFound(ApiResponse<Investimento>.ErrorResponse("Investimento não encontrado."));
            }

            investimentoExistente.DescricaoProposta = investimento.DescricaoProposta;

            await Task.Run(() => _investimentoRepository.Update(investimentoExistente));

            return Ok(ApiResponse<Investimento>.SuccessResponse(investimentoExistente, "Investimento atualizado com sucesso."));
        }

        /// <summary>
        /// Exclui um investimento específico.
        /// </summary>
        /// <param name="id">ID do investimento a ser excluído.</param>
        /// <returns>Resposta de sucesso ou erro.</returns>
        /// <response code="204">Investimento excluído com sucesso.</response>
        /// <response code="404">Investimento não encontrado.</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var investimento = await Task.Run(() => _investimentoRepository.GetById(id));
            if (investimento == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Investimento não encontrado."));
            }

            await Task.Run(() => _investimentoRepository.Delete(investimento));

            return NoContent();
        }
    }
}
