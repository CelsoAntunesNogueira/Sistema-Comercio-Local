using System.ComponentModel.DataAnnotations;

namespace SistemaPDV.Models.Entities
{
    public class Produto
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Nome { get; set; }

        [MaxLength(50)]
        public string CodigoBarras { get; set; }

        [Required]
        public decimal Preco { get; set; }

        [Required]
        public int EstoqueAtual { get; set; }

        public int EstoqueMinimo { get; set; } = 0;

        public bool Ativo { get; set; } = true;

        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}