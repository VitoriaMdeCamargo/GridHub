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
    /// Controlador para gerenciar as operações CRUD dos espaços.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EspacoController : ControllerBase
    {
        private readonly IRepository<Espaco> _espacoRepository;
        private readonly IRepository<Usuario> _usuarioRepository;

        public EspacoController(IRepository<Espaco> espacoRepository, IRepository<Usuario> usuarioRepository)
        {
            _espacoRepository = espacoRepository ?? throw new ArgumentNullException(nameof(espacoRepository));
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        /// <summary>
        /// Obtém um espaço específico pelo ID.
        /// </summary>
        /// <param name="id">ID do espaço.</param>
        /// <returns>Espaço solicitado.</returns>
        /// <response code="200">Retorna o espaço solicitado.</response>
        /// <response code="404">Espaço não encontrado.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Espaco>>> Get(int id)
        {
            var espaco = await Task.Run(() => _espacoRepository.GetById(id));

            if (espaco == null)
            {
                return NotFound(ApiResponse<Espaco>.ErrorResponse("Espaço não encontrado."));
            }

            return Ok(ApiResponse<Espaco>.SuccessResponse(espaco));
        }

        /// <summary>
        /// Obtém todos os espaços cadastrados.
        /// </summary>
        /// <returns>Lista de espaços.</returns>
        /// <response code="200">Retorna a lista de espaços.</response>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Espaco>>>> GetAll()
        {
            var espacos = await Task.Run(() => _espacoRepository.GetAll());

            var espacosList = espacos.ToList();

            if (espacosList == null || espacosList.Count == 0)
            {
                return NotFound(ApiResponse<List<Espaco>>.ErrorResponse("Nenhum espaço encontrado."));
            }

            return Ok(ApiResponse<List<Espaco>>.SuccessResponse(espacosList));
        }

        /// <summary>
        /// Adiciona um novo espaço ao sistema.
        /// </summary>
        /// <param name="espaco">Objeto contendo os dados do espaço a ser criado.</param>
        /// <returns>Retorna um objeto de resposta contendo o status da operação.</returns>
        /// <exception cref="ArgumentNullException">Lançada se o parâmetro 'espaco' for nulo.</exception>
        /// <response code="201">Retorna o espaço criado.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Espaco>>> Post([FromBody] Espaco espaco)
        {
            if (espaco == null)
            {
                return BadRequest(ApiResponse<Espaco>.ErrorResponse("Dados inválidos."));
            }

            var usuario = await Task.Run(() => _usuarioRepository.GetById(espaco.UsuarioId));
            if (usuario == null)
            {
                return BadRequest(ApiResponse<Espaco>.ErrorResponse("Usuário não encontrado."));
            }

            Espaco novoEspaco = new Espaco
            {
                UsuarioId = espaco.UsuarioId,
                Endereco = espaco.Endereco,
                NomeEspaco = espaco.NomeEspaco,
                FotoEspaco = espaco.FotoEspaco ?? "foto_espaco_padrao.jpg", 
                FonteEnergia = espaco.FonteEnergia,
                OrientacaoSolar = espaco.OrientacaoSolar,
                MediaSolar = espaco.MediaSolar,
                Topografia = espaco.Topografia,
                AreaTotal = espaco.AreaTotal,
                DirecaoVento = espaco.DirecaoVento ?? "Vento predominante do norte", 
                VelocidadeVento = espaco.VelocidadeVento
            };

            await Task.Run(() => _espacoRepository.Add(novoEspaco));

            return CreatedAtAction(nameof(Get), new { id = novoEspaco.EspacoId }, ApiResponse<Espaco>.SuccessResponse(novoEspaco, "Espaço criado com sucesso."));
        }

        /// <summary>
        /// Atualiza um espaço existente.
        /// </summary>
        /// <param name="id">ID do espaço a ser atualizado.</param>
        /// <param name="espaco">Objeto espaço com os novos dados.</param>
        /// <returns>Espaço atualizado.</returns>
        /// <response code="200">Retorna o espaço atualizado.</response>
        /// <response code="400">Dados inválidos ou ID não corresponde ao espaço.</response>
        /// <response code="404">Espaço não encontrado.</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Espaco>>> Put(int id, [FromBody] Espaco espaco)
        {
            if (espaco == null || id != espaco.EspacoId)
            {
                return BadRequest(ApiResponse<Espaco>.ErrorResponse("Dados inválidos ou ID não corresponde ao espaço."));
            }

            var espacoExistente = await Task.Run(() => _espacoRepository.GetById(id));
            if (espacoExistente == null)
            {
                return NotFound(ApiResponse<Espaco>.ErrorResponse("Espaço não encontrado."));
            }

            espacoExistente.Endereco = espaco.Endereco;
            espacoExistente.NomeEspaco = espaco.NomeEspaco;
            espacoExistente.FotoEspaco = espaco.FotoEspaco;
            espacoExistente.FonteEnergia = espaco.FonteEnergia;
            espacoExistente.OrientacaoSolar = espaco.OrientacaoSolar;
            espacoExistente.MediaSolar = espaco.MediaSolar;
            espacoExistente.Topografia = espaco.Topografia;
            espacoExistente.AreaTotal = espaco.AreaTotal;
            espacoExistente.DirecaoVento = espaco.DirecaoVento;
            espacoExistente.VelocidadeVento = espaco.VelocidadeVento;

            await Task.Run(() => _espacoRepository.Update(espacoExistente));

            return Ok(ApiResponse<Espaco>.SuccessResponse(espacoExistente, "Espaço atualizado com sucesso."));
        }

        /// <summary>
        /// Exclui um espaço específico.
        /// </summary>
        /// <param name="id">ID do espaço a ser excluído.</param>
        /// <returns>Resposta de sucesso ou erro.</returns>
        /// <response code="204">Espaço excluído com sucesso.</response>
        /// <response code="404">Espaço não encontrado.</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var espaco = await Task.Run(() => _espacoRepository.GetById(id));
            if (espaco == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Espaço não encontrado."));
            }

            await Task.Run(() => _espacoRepository.Delete(espaco));

            return NoContent();
        }
    }
}
