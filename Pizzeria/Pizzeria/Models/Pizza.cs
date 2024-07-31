using System;
using System.ComponentModel.DataAnnotations;

namespace Pizzeria.Models
{
	public class Pizza 
	{
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string Nome { get; set; }

        [Required]
        public required byte[] Foto { get; set; }

        [Range(0, 100)]
        public decimal PrezzoVendita { get; set; }

        public int TempoDiConsegna { get; set; }

        public List<Ingrediente> Ingredienti { get; set; } = new();
    }
}

