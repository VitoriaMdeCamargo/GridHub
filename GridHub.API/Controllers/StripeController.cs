using GridHub.Service.Payment;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace GridHub.API.Controllers
{
    /// <summary>
    /// Controller responsável por interagir com a API do Stripe para processar pagamentos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StripeController : ControllerBase
    {
        /// <summary>
        /// Cria uma intenção de pagamento no Stripe com base nos detalhes fornecidos.
        /// </summary>
        /// <param name="paymentRequest">Os detalhes da solicitação de pagamento, incluindo o valor.</param>
        /// <returns>Retorna um objeto contendo o clientSecret para completar o pagamento ou uma mensagem de erro.</returns>
        [HttpPost("create-payment-intent")]
        public IActionResult CreatePaymentIntent([FromBody] PaymentRequest paymentRequest)
        {
            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = paymentRequest.Amount,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" },
                };
                var service = new PaymentIntentService();
                PaymentIntent intent = service.Create(options);

                return Ok(new { clientSecret = intent.ClientSecret });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    /// <summary>
    /// Modelo que representa a solicitação de pagamento.
    /// </summary>
    public class PaymentRequest
    {
        /// <summary>
        /// Valor do pagamento a ser processado, em centavos.
        /// </summary>
        public long Amount { get; set; }
    }
}
