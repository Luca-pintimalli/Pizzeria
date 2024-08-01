using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pizzeria.Models
{
	public class OrderItem
	{
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        public required Articolo Articoli { get; set; }


        public required Ordine Ordini { get; set; }

        public int Quantita { get; set; }





    }
}

