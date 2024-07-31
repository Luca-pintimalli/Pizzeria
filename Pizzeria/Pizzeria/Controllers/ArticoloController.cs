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
        public async Task<IActionResult> NuovoArticolo(int[] selectedIngredienti, FileUpload model)
        {
            if (model.UploadFile == null || model.UploadFile.Length == 0)
            {
                ModelState.AddModelError("UploadFile", "La foto è obbligatoria.");
            }

            if (ModelState.IsValid)
            {
                var articolo = new Articolo
                {
                    Nome = model.Nome,
                    PrezzoVendita = model.PrezzoVendita,
                    TempoDiConsegna = model.TempoDiConsegna,
                    Ingredienti = _dataContext.Ingredienti.Where(i => selectedIngredienti.Contains(i.Id)).ToList()
                };

                // Gestione del file caricato
                if (model.UploadFile != null && model.UploadFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.UploadFile.CopyToAsync(memoryStream);
                        articolo.Foto = memoryStream.ToArray();
                    }
                }
                else
                {
                    // Aggiungi un valore predefinito o gestisci l'errore se necessario
                    // articolo.Foto = GetDefaultImage(); // Metodo per ottenere un'immagine predefinita se necessario
                }

                _dataContext.Articoli.Add(articolo);
                await _dataContext.SaveChangesAsync();

                return RedirectToAction("NuovoArticolo");
            }

            // Recarica la lista degli ingredienti in caso di errore
            ViewBag.Ingredienti = _dataContext.Ingredienti.ToList();
            return View(model);
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
            var model = new FileUpload
            {
                Id = articolo.Id,
                Nome = articolo.Nome,
                PrezzoVendita = articolo.PrezzoVendita,
                TempoDiConsegna = articolo.TempoDiConsegna
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FileUpload model, int[] selectedIngredienti)
        {
            if (ModelState.IsValid)
            {
                var articoloToUpdate = _dataContext.Articoli
                    .Include(a => a.Ingredienti)
                    .Single(a => a.Id == id);

                if (articoloToUpdate == null)
                {
                    return NotFound();
                }

                articoloToUpdate.Nome = model.Nome;
                articoloToUpdate.PrezzoVendita = model.PrezzoVendita;
                articoloToUpdate.TempoDiConsegna = model.TempoDiConsegna;
                articoloToUpdate.Ingredienti = _dataContext.Ingredienti.Where(i => selectedIngredienti.Contains(i.Id)).ToList();

                if (model.UploadFile != null && model.UploadFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.UploadFile.CopyToAsync(memoryStream);
                        articoloToUpdate.Foto = memoryStream.ToArray();
                    }
                }

                await _dataContext.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Ingredienti = _dataContext.Ingredienti.ToList();
            return View(model);
        }

        public IActionResult Delete(int id)
        {
            var articolo = _dataContext.Articoli
                .Include(a => a.Ingredienti)
                .SingleOrDefault(a => a.Id == id);

            if (articolo == null)
            {
                return NotFound();
            }

            return View(articolo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var articolo = _dataContext.Articoli
                .Include(a => a.Ingredienti)
                .SingleOrDefault(a => a.Id == id);

            if (articolo == null)
            {
                return NotFound();
            }

            _dataContext.Articoli.Remove(articolo);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction("NuovoArticolo", "Articolo");
        }
   
}
}
    