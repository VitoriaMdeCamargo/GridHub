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
    /// Controlador para gerenciar as operações CRUD das microgrids.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MicrogridController : ControllerBase
    {
        private readonly IRepository<Microgrid> _microgridRepository;
        private readonly IRepository<Usuario> _usuarioRepository;
        private readonly IRepository<Espaco> _espacoRepository;

        public MicrogridController(IRepository<Microgrid> microgridRepository, IRepository<Usuario> usuarioRepository, IRepository<Espaco> espacoRepository)
        {
            _microgridRepository = microgridRepository ?? throw new ArgumentNullException(nameof(microgridRepository));
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            _espacoRepository = espacoRepository ?? throw new ArgumentNullException(nameof(espacoRepository));
        }

        /// <summary>
        /// Obtém uma microgrid específica pelo ID.
        /// </summary>
        /// <param name="id">ID da microgrid.</param>
        /// <returns>Microgrid solicitada.</returns>
        /// <response code="200">Retorna a microgrid solicitada.</response>
        /// <response code="404">Microgrid não encontrada.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Microgrid>>> Get(int id)
        {
            var microgrid = await Task.Run(() => _microgridRepository.GetById(id));

            if (microgrid == null)
            {
                return NotFound(ApiResponse<Microgrid>.ErrorResponse("Microgrid não encontrada."));
            }

            return Ok(ApiResponse<Microgrid>.SuccessResponse(microgrid));
        }

        /// <summary>
        /// Obtém todas as microgrids cadastradas.
        /// </summary>
        /// <returns>Lista de microgrids.</returns>
        /// <response code="200">Retorna a lista de microgrids.</response>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Microgrid>>>> GetAll()
        {
            var microgrids = await Task.Run(() => _microgridRepository.GetAll());

            var microgridsList = microgrids.ToList();

            if (microgridsList == null || microgridsList.Count == 0)
            {
                return NotFound(ApiResponse<List<Microgrid>>.ErrorResponse("Nenhuma microgrid encontrada."));
            }

            return Ok(ApiResponse<List<Microgrid>>.SuccessResponse(microgridsList));
        }

        /// <summary>
        /// Adiciona uma nova microgrid ao sistema.
        /// </summary>
        /// <param name="microgrid">Objeto contendo os dados da microgrid a ser criada.</param>
        /// <returns>Retorna um objeto de resposta contendo o status da operação.</returns>
        /// <exception cref="ArgumentNullException">Lançada se o parâmetro 'microgrid' for nulo.</exception>
        /// <response code="201">Retorna a microgrid criada.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Microgrid>>> Post([FromBody] Microgrid microgrid)
        {
            if (microgrid == null)
            {
                return BadRequest(ApiResponse<Microgrid>.ErrorResponse("Dados inválidos."));
            }

            var usuario = await Task.Run(() => _usuarioRepository.GetById(microgrid.UsuarioId));
            if (usuario == null)
            {
                return BadRequest(ApiResponse<Microgrid>.ErrorResponse("Usuário não encontrado."));
            }

            var espaco = await Task.Run(() => _espacoRepository.GetById(microgrid.EspacoId));
            if (espaco == null)
            {
                return BadRequest(ApiResponse<Microgrid>.ErrorResponse("Espaço não encontrado."));
            }

            Microgrid novaMicrogrid = new Microgrid
            {
                UsuarioId = microgrid.UsuarioId,
                EspacoId = microgrid.EspacoId,
                NomeMicrogrid = microgrid.NomeMicrogrid,
                FotoMicrogrid = microgrid.FotoMicrogrid ?? "foto_microgrid_padrao.jpg",
                RadiacaoSolarNecessaria = microgrid.RadiacaoSolarNecessaria,
                TopografiaNecessaria = microgrid.TopografiaNecessaria,
                AreaTotalNecessaria = microgrid.AreaTotalNecessaria,
                VelocidadeVentoNecessaria = microgrid.VelocidadeVentoNecessaria,
                FonteEnergia = microgrid.FonteEnergia,
                MetaFinanciamento = microgrid.MetaFinanciamento
            };

            await Task.Run(() => _microgridRepository.Add(novaMicrogrid));

            return CreatedAtAction(nameof(Get), new { id = novaMicrogrid.MicrogridId }, ApiResponse<Microgrid>.SuccessResponse(novaMicrogrid, "Microgrid criada com sucesso."));
        }

        /// <summary>
        /// Atualiza uma microgrid existente.
        /// </summary>
        /// <param name="id">ID da microgrid a ser atualizada.</param>
        /// <param name="microgrid">Objeto microgrid com os novos dados.</param>
        /// <returns>Microgrid atualizada.</returns>
        /// <response code="200">Retorna a microgrid atualizada.</response>
        /// <response code="400">Dados inválidos ou ID não corresponde à microgrid.</response>
        /// <response code="404">Microgrid não encontrada.</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Microgrid>>> Put(int id, [FromBody] Microgrid microgrid)
        {
            if (microgrid == null || id != microgrid.MicrogridId)
            {
                return BadRequest(ApiResponse<Microgrid>.ErrorResponse("Dados inválidos ou ID não corresponde à microgrid."));
            }

            var microgridExistente = await Task.Run(() => _microgridRepository.GetById(id));
            if (microgridExistente == null)
            {
                return NotFound(ApiResponse<Microgrid>.ErrorResponse("Microgrid não encontrada."));
            }

            microgridExistente.NomeMicrogrid = microgrid.NomeMicrogrid;
            microgridExistente.FotoMicrogrid = microgrid.FotoMicrogrid;
            microgridExistente.RadiacaoSolarNecessaria = microgrid.RadiacaoSolarNecessaria;
            microgridExistente.TopografiaNecessaria = microgrid.TopografiaNecessaria;
            microgridExistente.AreaTotalNecessaria = microgrid.AreaTotalNecessaria;
            microgridExistente.VelocidadeVentoNecessaria = microgrid.VelocidadeVentoNecessaria;
            microgridExistente.FonteEnergia = microgrid.FonteEnergia;
            microgridExistente.MetaFinanciamento = microgrid.MetaFinanciamento;

            await Task.Run(() => _microgridRepository.Update(microgridExistente));

            return Ok(ApiResponse<Microgrid>.SuccessResponse(microgridExistente, "Microgrid atualizada com sucesso."));
        }

        /// <summary>
        /// Exclui uma microgrid específica.
        /// </summary>
        /// <param name="id">ID da microgrid a ser excluída.</param>
        /// <returns>Resposta de sucesso ou erro.</returns>
        /// <response code="204">Microgrid excluída com sucesso.</response>
        /// <response code="404">Microgrid não encontrada.</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var microgrid = await Task.Run(() => _microgridRepository.GetById(id));
            if (microgrid == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Microgrid não encontrada."));
            }

            await Task.Run(() => _microgridRepository.Delete(microgrid));

            return NoContent();
        }
    }
}
