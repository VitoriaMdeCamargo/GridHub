using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GridHub.Database.Models
{
    public class Espaco
    {
        public int EspacoId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(255)]
        [DefaultValue("Rua Fictícia, 123, Bairro Fictício")]
        public string Endereco { get; set; }

        [Required]
        [StringLength(100)]
        [DefaultValue("Espaço Solar Premium")]
        public string NomeEspaco { get; set; }

        [StringLength(500)]
        [DefaultValue("foto_espaco_padrao.jpg")]
        public string FotoEspaco { get; set; }

        [Required]
        [StringLength(50)]
        [DefaultValue("Energia Solar")]
        public string FonteEnergia { get; set; }

        [Required]
        [StringLength(50)]
        [DefaultValue("Sul")]
        public string OrientacaoSolar { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "A média solar deve ser um valor positivo.")]
        [DefaultValue(4.5)]
        public double MediaSolar { get; set; }

        [Required]
        [StringLength(100)]
        [DefaultValue("Terreno plano e regular")]
        public string Topografia { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "A área total deve ser um valor positivo.")]
        [DefaultValue(5000.0)]
        public double AreaTotal { get; set; }

        [StringLength(50)]
        [DefaultValue("Vento predominante do norte")]
        public string DirecaoVento { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "A velocidade do vento deve ser um valor positivo.")]
        [DefaultValue(15.0)]
        public double VelocidadeVento { get; set; }
    }
}
