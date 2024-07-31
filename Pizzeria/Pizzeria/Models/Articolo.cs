using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pizzeria.Models
{
	public class Articolo 
    {
		[Key ,DatabaseGenerated(DatabaseGeneratedOption.Identity) ]
		public int Id { get; set; }

		[StringLength(50)]
		public required string Nome { get; set; }

        [Required]
        public  byte[]? Foto { get; set; }

		[Range (0, 100)]
		public decimal PrezzoVendita {get;set;}

		public int TempoDiConsegna { get; set; }

        public List<Ingrediente> Ingredienti { get; set; } = new();

    }
}

