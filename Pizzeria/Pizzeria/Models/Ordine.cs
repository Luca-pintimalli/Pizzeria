using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pizzeria.Models
{
	public class Ordine
	{
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        public DateTime DataOrdine { get; set; }


        public required User User { get; set; }

        public bool OrdineEvaso { get; set; }

        [Required]
        [StringLength(80)]
        public required string Indirizzo { get; set; }

        [StringLength(255)]
        public string? Note { get; set; }


        public List<OrderItem> Items { get; set; } = [];

    }
}

