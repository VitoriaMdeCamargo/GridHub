using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GridHub.Database.Models
{
    public class Usuario
    {
        public Usuario(string email, string senha)
        {
            Email = email;
            DefinirSenha(senha);
        }

        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email fornecido não é válido.")]
        [DefaultValue("exemplo@email.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha é obrigatória.")]
        [DefaultValue("")]
        public string Senha { get; private set; }

        [Required(ErrorMessage = "Nome é obrigatório.")]
        [DefaultValue("João")]
        public string Nome { get; set; }

        [Required]
        [DefaultValue("000000000")]
        public string Telefone { get; set; }

        [Required]
        [DefaultValue("foto_padrao.png")]
        public string FotoPerfil { get; set; }

        [DefaultValue(typeof(DateTime), "2024-01-01")]
        public DateTime DataCriacao { get; set; }

        public void DefinirSenha(string senha)
        {
            Senha = BCrypt.Net.BCrypt.HashPassword(senha, 13);
        }

        public bool VerificarSenha(string senha)
        {
            return BCrypt.Net.BCrypt.Verify(senha, Senha);
        }
    }
}
