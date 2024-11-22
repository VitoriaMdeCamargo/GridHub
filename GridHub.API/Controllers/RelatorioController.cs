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
    /// Controlador para gerenciar as operações CRUD dos relatórios.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RelatorioController : ControllerBase
    {
        private readonly IRepository<Relatorio> _relatorioRepository;
        private readonly IRepository<Microgrid> _microgridRepository;

        public RelatorioController(IRepository<Relatorio> relatorioRepository, IRepository<Microgrid> microgridRepository)
        {
            _relatorioRepository = relatorioRepository ?? throw new ArgumentNullException(nameof(relatorioRepository));
            _microgridRepository = microgridRepository ?? throw new ArgumentNullException(nameof(microgridRepository));
        }

        /// <summary>
        /// Obtém um relatório específico pelo ID.
        /// </summary>
        /// <param name="id">ID do relatório.</param>
        /// <returns>Relatório solicitado.</returns>
        /// <response code="200">Retorna o relatório solicitado.</response>
        /// <response code="404">Relatório não encontrado.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Relatorio>>> Get(int id)
        {
            var relatorio = await Task.Run(() => _relatorioRepository.GetById(id));

            if (relatorio == null)
            {
                return NotFound(ApiResponse<Relatorio>.ErrorResponse("Relatório não encontrado."));
            }

            return Ok(ApiResponse<Relatorio>.SuccessResponse(relatorio));
        }

        /// <summary>
        /// Adiciona um novo relatório ao sistema.
        /// </summary>
        /// <param name="relatorio">Objeto contendo os dados do relatório a ser criado.</param>
        /// <returns>Retorna um objeto de resposta contendo o status da operação.</returns>
        /// <exception cref="ArgumentNullException">Lançada se o parâmetro 'relatorio' for nulo.</exception>
        /// <response code="201">Retorna o relatório criado.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Relatorio>>> Post([FromBody] Relatorio relatorio)
        {
            if (relatorio == null)
            {
                return BadRequest(ApiResponse<Relatorio>.ErrorResponse("Dados inválidos."));
            }

            var microgrid = await Task.Run(() => _microgridRepository.GetById(relatorio.MicrogridId));
            if (microgrid == null)
            {
                return BadRequest(ApiResponse<Relatorio>.ErrorResponse("Microgrid não encontrada."));
            }

            Relatorio novoRelatorio = new Relatorio
            {
                MicrogridId = relatorio.MicrogridId,
                EnergiaGerada = relatorio.EnergiaGerada,
                TempPainelSolar = relatorio.TempPainelSolar,
                LucroGerado = relatorio.LucroGerado
            };

            await Task.Run(() => _relatorioRepository.Add(novoRelatorio));

            return CreatedAtAction(nameof(Get), new { id = novoRelatorio.RelatorioId }, ApiResponse<Relatorio>.SuccessResponse(novoRelatorio, "Relatório criado com sucesso."));
        }

        /// <summary>
        /// Atualiza um relatório existente.
        /// </summary>
        /// <param name="id">ID do relatório a ser atualizado.</param>
        /// <param name="relatorio">Objeto relatório com os novos dados.</param>
        /// <returns>Relatório atualizado.</returns>
        /// <response code="200">Retorna o relatório atualizado.</response>
        /// <response code="400">Dados inválidos ou ID não corresponde ao relatório.</response>
        /// <response code="404">Relatório não encontrado.</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Relatorio>>> Put(int id, [FromBody] Relatorio relatorio)
        {
            if (relatorio == null || id != relatorio.RelatorioId)
            {
                return BadRequest(ApiResponse<Relatorio>.ErrorResponse("Dados inválidos ou ID não corresponde ao relatório."));
            }

            var relatorioExistente = await Task.Run(() => _relatorioRepository.GetById(id));
            if (relatorioExistente == null)
            {
                return NotFound(ApiResponse<Relatorio>.ErrorResponse("Relatório não encontrado."));
            }

            relatorioExistente.EnergiaGerada = relatorio.EnergiaGerada;
            relatorioExistente.TempPainelSolar = relatorio.TempPainelSolar;
            relatorioExistente.LucroGerado = relatorio.LucroGerado;

            await Task.Run(() => _relatorioRepository.Update(relatorioExistente));

            return Ok(ApiResponse<Relatorio>.SuccessResponse(relatorioExistente, "Relatório atualizado com sucesso."));
        }

        /// <summary>
        /// Exclui um relatório específico.
        /// </summary>
        /// <param name="id">ID do relatório a ser excluído.</param>
        /// <returns>Resposta de sucesso ou erro.</returns>
        /// <response code="204">Relatório excluído com sucesso.</response>
        /// <response code="404">Relatório não encontrado.</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var relatorio = await Task.Run(() => _relatorioRepository.GetById(id));
            if (relatorio == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Relatório não encontrado."));
            }

            await Task.Run(() => _relatorioRepository.Delete(relatorio));

            return NoContent();
        }
    }
}
