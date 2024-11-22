using Microsoft.AspNetCore.Mvc;
using GridHub_ML;

namespace MyEnergyPredictionApi.Controllers
{
    /// <summary>
    /// Previsão de geração de energia com base no clima.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MyEnergyController : ControllerBase
    {
        /// <summary>
        /// Realiza uma previsão de geração de energia gerada com base nos dados de clima fornecidos.
        /// </summary>
        /// <param name="input">Os dados de entrada necessários para a previsão, incluindo temperatura, hora do dia, cobertura de nuvens, velocidade do vento e energia gerada.</param>
        /// <returns>Retorna a saída do modelo de previsão ou mensagens de erro se os dados forem inválidos.</returns>
        [HttpPost("predict")]
        public ActionResult<ValuePrediction.ModelOutput> Predict([FromBody] ValuePrediction.ModelInput input)
        {
            // Verifica se os dados de entrada são nulos
            if (input == null)
            {
                return BadRequest("Invalid input data.");
            }

            // Valida os campos para garantir que não contenham valores inválidos
            if (input.Temperature < 0 || input.HourOfDay < 0 || input.CloudCoverage < 0 || input.WindSpeed < 0 || input.EnergyGenerated < 0)
            {
                return BadRequest("Input data contains invalid values.");
            }

            // Realiza a previsão usando o modelo ML.NET
            var result = ValuePrediction.Predict(input);

            // Retorna o resultado da previsão
            return Ok(result);
        }
    }
}
