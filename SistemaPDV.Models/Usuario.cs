using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaPDV.Models.Entities
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nome { get; set; }

        [Required, MaxLength(50)]
        public string Login { get; set; }

        [Required, MaxLength(255)]
        public string SenhaHash { get; set; }

        [Required, MaxLength(20)]
        public string Tipo { get; set; } // "Administrador" ou "Vendedor"

        public bool Ativo { get; set; } = true;

        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}