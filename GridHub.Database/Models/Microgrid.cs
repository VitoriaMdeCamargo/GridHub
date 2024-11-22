using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GridHub.Database.Models
{
    public class Microgrid
    {
        [Key]
        public int MicrogridId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int EspacoId { get; set; }

        [Required]
        [StringLength(100)]
        [DefaultValue("Microgrid Padrão")]
        public string NomeMicrogrid { get; set; }

        [StringLength(500)]
        [DefaultValue("foto_microgrid_padrao.jpg")]
        public string FotoMicrogrid { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "A radiação solar necessária deve ser um valor positivo.")]
        [DefaultValue(4.5)]
        public double RadiacaoSolarNecessaria { get; set; }

        [Required]
        [StringLength(100)]
        [DefaultValue("Terreno plano")]
        public string TopografiaNecessaria { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "A área total necessária deve ser um valor positivo.")]
        [DefaultValue(3000.0)]
        public double AreaTotalNecessaria { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "A velocidade do vento necessária deve ser um valor positivo.")]
        [DefaultValue(10.0)]
        public double VelocidadeVentoNecessaria { get; set; }

        [Required]
        [StringLength(50)]
        [DefaultValue("Energia Solar")]
        public string FonteEnergia { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "A meta de financiamento deve ser um valor positivo.")]
        [DefaultValue(50000.0)]
        public double MetaFinanciamento { get; set; }
    }
}
