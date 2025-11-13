using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPDV.Models.Entities
{
    public class Venda
    {
        [Key]
        public int Id { get; set; }

        public DateTime DataVenda { get; set; } = DateTime.Now;

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        public int? ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; }

        [Required]
        public decimal ValorTotal { get; set; }

        public virtual ICollection<ItemVenda> Itens { get; set; }
    }
}