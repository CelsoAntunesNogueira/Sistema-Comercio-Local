using System.ComponentModel.DataAnnotations;

namespace SistemaPDV.Models.Entities
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Nome { get; set; }

        [MaxLength(14)]
        public string CPF { get; set; }

        [MaxLength(15)]
        public string Telefone { get; set; }

        [MaxLength(200)]
        public string Email { get; set; }

        public bool Ativo { get; set; } = true;

        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}