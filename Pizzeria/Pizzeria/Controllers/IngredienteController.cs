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
            return View(_dataContex.Ingredienti);

        }




        public IActionResult NuovoIngrediente()
		{
			return View();

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
        public IActionResult NuovoIngrediente(Ingrediente ingrediente)
        {
            _dataContex.Ingredienti.Add(ingrediente);
            _dataContex.SaveChanges();
            return RedirectToAction("NuovoArticolo", "Articolo");

        }


        [HttpGet]

        public IActionResult Edit(int id)
		{
			var ingrediente = _dataContex.Ingredienti.Single(a => a.Id == id);
			return View(ingrediente);
		}


        [HttpPost]

        public IActionResult Edit(Ingrediente ingrediente)
		{
			var a = _dataContex.Ingredienti.Single(a => a.Id == ingrediente.Id);
			a.Nome = ingrediente.Nome;
			_dataContex.SaveChanges();
            return RedirectToAction("NuovoArticolo", "Articolo");
        }



		public IActionResult Delete(int id)
		{
            var ingrediente = _dataContex.Ingredienti.Single(a => a.Id == id);
            return View(ingrediente);
        }

        [HttpPost]
        public IActionResult Delete(Ingrediente ingrediente)
        {
            var a = _dataContex.Ingredienti.Single(a => a.Id == ingrediente.Id);
                _dataContex.Ingredienti.Remove(a);
            _dataContex.SaveChanges();

            return RedirectToAction("NuovoArticolo", "Articolo");


        }

    }
}

