using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GridHub.Database.Models
{
    public class Relatorio
    {
        public int RelatorioId { get; set; }

        [Required]
        public int MicrogridId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "A energia gerada deve ser um valor positivo.")]
        [DefaultValue(0.0)]
        public double EnergiaGerada { get; set; }

        [Range(-50, 100, ErrorMessage = "A temperatura do painel solar deve estar entre -50 e 100°C.")]
        [DefaultValue(25.0)]
        public double TempPainelSolar { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "O lucro gerado deve ser um valor positivo.")]
        [DefaultValue(0.0)]
        public double LucroGerado { get; set; }
    }
}
