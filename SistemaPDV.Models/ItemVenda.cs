using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPDV.Models.Entities
{
    public class ItemVenda
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VendaId { get; set; }

        [ForeignKey("VendaId")]
        public Venda Venda { get; set; }

        [Required]
        public int ProdutoId { get; set; }

        [ForeignKey("ProdutoId")]
        public Produto Produto { get; set; }

        [Required]
        public int Quantidade { get; set; }

        [Required]
        public decimal PrecoUnitario { get; set; }

        [NotMapped]
        public decimal Subtotal => Quantidade * PrecoUnitario;
    }
}