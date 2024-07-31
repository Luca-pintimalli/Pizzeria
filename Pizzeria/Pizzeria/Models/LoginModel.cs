﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Pizzeria.Models
{
	public class LoginModel
	{
        [Required]
        [EmailAddress]
        [StringLength(80)]
        public required string Email { get; set; }
        /// <summary>
        /// Password.
        /// </summary>
        [Required]
        [StringLength(20)]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}

