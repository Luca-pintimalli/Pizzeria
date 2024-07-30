using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pizzeria.Models;
using Pizzeria.Services;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pizzeria.Controllers
{
    public class ArticoloController : Controller
    {
        private readonly DataContext _dataContext;

        public ArticoloController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IActionResult NuovoArticolo()
        {
            ViewBag.Ingredienti = _dataContext.Ingredienti.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NuovoArticolo(Articolo articolo, int[] selectedIngredienti, IFormFile fotoFile)
        {
            // Verifica se il modello è valido
            if (!ModelState.IsValid)
            {
                ViewBag.Ingredienti = _dataContext.Ingredienti.ToList();
                return View(articolo);
            }

            // Verifica se il file è stato caricato
            if (fotoFile != null && fotoFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await fotoFile.CopyToAsync(memoryStream);
                    articolo.Foto = memoryStream.ToArray();
                }
            }
            else
            {
                // Se il file è obbligatorio e non è stato caricato, mostra un errore
                ModelState.AddModelError("Foto", "La foto è obbligatoria.");
                ViewBag.Ingredienti = _dataContext.Ingredienti.ToList();
                return View(articolo);
            }

            // Assegna gli ingredienti selezionati
            articolo.Ingredienti = _dataContext.Ingredienti.Where(i => selectedIngredienti.Contains(i.Id)).ToList();

            // Aggiungi l'articolo al contesto e salva
            _dataContext.Articoli.Add(articolo);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction("Index"); // Supponendo che tu abbia una vista Index per visualizzare gli articoli
        }
    


    public IActionResult Edit(int id)
        {
            var articolo = _dataContext.Articoli
                .Include(a => a.Ingredienti)
                .SingleOrDefault(a => a.Id == id);

            if (articolo == null)
            {
                return NotFound();
            }

            ViewBag.Ingredienti = _dataContext.Ingredienti.ToList();
            return View(articolo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Articolo articolo, int[] selectedIngredienti, IFormFile fotoFile)
        {
            if (ModelState.IsValid)
            {
                var articoloToUpdate = _dataContext.Articoli
                    .Include(a => a.Ingredienti)
                    .Single(a => a.Id == articolo.Id);

                articoloToUpdate.Nome = articolo.Nome;
                articoloToUpdate.PrezzoVendita = articolo.PrezzoVendita;
                articoloToUpdate.TempoDiConsegna = articolo.TempoDiConsegna;
                articoloToUpdate.Ingredienti = _dataContext.Ingredienti.Where(i => selectedIngredienti.Contains(i.Id)).ToList();

                if (fotoFile != null && fotoFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await fotoFile.CopyToAsync(memoryStream);
                        articoloToUpdate.Foto = memoryStream.ToArray();
                    }
                }

                await _dataContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Ingredienti = _dataContext.Ingredienti.ToList();
            return View(articolo);
        }
    }
}
