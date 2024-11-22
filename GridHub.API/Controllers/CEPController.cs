using ERP_InsightWise.Service.CEP;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ERP_InsightWise.API.Controllers
{
    /// <summary>
    /// Controller responsável por lidar com operações relacionadas a CEP.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class CEPController : ControllerBase
    {
        private readonly ICEPService _cepService;

        /// <summary>
        /// Construtor do CEPController.
        /// </summary>
        /// <param name="cepService">Serviço para manipulação e busca de informações de CEP.</param>
        public CEPController(ICEPService cepService)
        {
            _cepService = cepService;
        }

        /// <summary>
        /// Obtém as informações de endereço com base no CEP informado.
        /// </summary>
        /// <param name="cep">CEP para o qual as informações de endereço devem ser recuperadas.</param>
        /// <returns>Retorna um objeto contendo as informações de endereço correspondentes ao CEP.</returns>
        /// <response code="200">Se a busca pelo CEP for bem-sucedida.</response>
        /// <response code="404">Se o CEP não for encontrado.</response>
        /// <response code="500">Se ocorrer um erro no servidor.</response>
        [HttpGet]
        [ProducesResponseType(typeof(AddressResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public Task<AddressResponse> GetAddressResponse(string cep)
        {
            return _cepService.GetAddressbyCEP(cep);
        }
    }
}
