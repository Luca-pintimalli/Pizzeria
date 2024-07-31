using System;
using Microsoft.AspNetCore.Mvc;
using Pizzeria.Models;
using Pizzeria.Services;

namespace Pizzeria.Controllers
{
	public class IngredienteController : Controller
	{
		private readonly ILogger<IngredienteController> _logger;
		private readonly DataContext _dataContex;


		public IngredienteController(DataContext dataContext, ILogger<IngredienteController> logger)
		{
			_logger = logger;
			_dataContex = dataContext;
		}


        public IActionResult Ingredienti()
        {
            var ingredientiOrdinati = _dataContex.Ingredienti.OrderBy(i => i.Nome).ToList();
            return View(ingredientiOrdinati);
        }



        public IActionResult NuovoIngrediente()
		{
			return View();

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
        public IActionResult NuovoIngrediente(Ingrediente ingrediente)
        {
            if (ModelState.IsValid)
            {
                _dataContex.Ingredienti.Add(ingrediente);
                _dataContex.SaveChanges();
                return RedirectToAction("NuovoArticolo", "Articolo");
            }
            return View("Index");

        }


        [HttpGet]

        public IActionResult Edit(int id)
		{
            
			var ingrediente = _dataContex.Ingredienti.Single(a => a.Id == id);
			return View(ingrediente);
		}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Ingrediente ingrediente)
		{
            if (ModelState.IsValid)
            {
                var a = _dataContex.Ingredienti.Single(a => a.Id == ingrediente.Id);
                a.Nome = ingrediente.Nome;
                _dataContex.SaveChanges();
                return RedirectToAction("NuovoArticolo", "Articolo");
            }
            return View(ingrediente);
        }



		public IActionResult Delete(int id)
		{
            var ingrediente = _dataContex.Ingredienti.Single(a => a.Id == id);
            return View(ingrediente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Delete(Ingrediente ingrediente)
        {
            var a = _dataContex.Ingredienti.Single(a => a.Id == ingrediente.Id);
                _dataContex.Ingredienti.Remove(a);
            _dataContex.SaveChanges();

            return RedirectToAction("NuovoArticolo", "Articolo");


        }

    }
}

