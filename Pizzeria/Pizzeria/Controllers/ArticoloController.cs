using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pizzeria.Models;
using Pizzeria.Services;

namespace Pizzeria.Controllers
{
	public class ArticoloController : Controller
	{
		private readonly ILogger<ArticoloController> _logger;
		private readonly DataContext _dataContex;



		public ArticoloController(DataContext dataContext, ILogger<ArticoloController> logger)
		{
			_logger = logger;
			_dataContex = dataContext;

        }


		public IActionResult NuovoArticolo()
		{
			return View();
		}




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NuovoArticolo(Articolo articolo, IFormFile fotoFile)
        {
            if (fotoFile != null && fotoFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await fotoFile.CopyToAsync(memoryStream);
                    articolo.Foto = memoryStream.ToArray();
                }
            }

            if (ModelState.IsValid)
            {
                _dataContex.Add(articolo);
                await _dataContex.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(articolo);
        }

        public IActionResult GetPhoto(int id)
        {
            var articolo = _dataContex.Articoli.Find(id);
            if (articolo == null || articolo.Foto == null)
            {
                return NotFound();
            }

            return File(articolo.Foto, "image/jpg"); // Usa il tipo MIME appropriato
        }
























    }
}

