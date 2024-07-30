using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Pizzeria.Models
{
	public class User
	{
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required]
        [StringLength(20)]
        public required string Nome { get; set; }


        [Required]
        [EmailAddress]
        public required string Email { get; set; }


        [Required]
        [StringLength(20)]
        public required string Password { get; set; }


        public List<Role> Roles { get; set; } = [];


        public List<Ordine> Ordini { get; set; } = [];


    }
}

