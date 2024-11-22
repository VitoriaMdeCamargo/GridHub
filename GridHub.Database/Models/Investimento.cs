using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GridHub.Database.Models
{
    public class Investimento
    {
        public int InvestimentoId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int MicrogridId { get; set; }

        [Required]
        [StringLength(500)]
        [DefaultValue("Proposta de investimento para implementação de microgrid")]
        public string DescricaoProposta { get; set; }
    }
}
