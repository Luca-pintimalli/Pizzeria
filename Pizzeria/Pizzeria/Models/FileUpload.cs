using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Pizzeria.Models
{
    public class FileUpload
    {
        public IFormFile UploadFile { get; set; }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string Nome { get; set; }

        [Range(0, 100)]
        public decimal PrezzoVendita { get; set; }

        public int TempoDiConsegna { get; set; }
    }
}
