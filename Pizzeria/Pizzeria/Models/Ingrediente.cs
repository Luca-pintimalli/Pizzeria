using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pizzeria.Models
{
	public class Ingrediente
	{
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

		[Required]
		[StringLength(50)]
		public required string Nome { get; set; }


		public List<Articolo> Articoli { get; set; } = [];
	}
}

