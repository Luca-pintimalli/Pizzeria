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

        // Metodo per visualizzare la vista di creazione di un nuovo articolo
        public IActionResult NuovoArticolo()
        {
            // Carica la lista degli ingredienti ordinati per nome e li passa alla vista tramite ViewBag
            ViewBag.Ingredienti = _dataContext.Ingredienti.OrderBy(i => i.Nome).ToList();
            return View();
        }

        [HttpPost]
        // Metodo per gestire la creazione di un nuovo articolo
        public async Task<IActionResult> NuovoArticolo(int[] selectedIngredienti, FileUpload model)
        {
            // Verifica se il file caricato è null o vuoto e aggiunge un errore al ModelState se lo è
            if (model.UploadFile == null || model.UploadFile.Length == 0)
            {
                ModelState.AddModelError("UploadFile", "La foto è obbligatoria.");
            }

            // Se il ModelState è valido
            if (ModelState.IsValid)
            {
                // Crea un nuovo oggetto Articolo
                var articolo = new Articolo
                {
                    Nome = model.Nome,
                    PrezzoVendita = model.PrezzoVendita,
                    TempoDiConsegna = model.TempoDiConsegna,
                    // Assegna gli ingredienti selezionati al nuovo articolo
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

                // Aggiunge il nuovo articolo al contesto dati e salva le modifiche
                _dataContext.Articoli.Add(articolo);
                await _dataContext.SaveChangesAsync();

                // Reindirizza alla vista di creazione di un nuovo articolo
                return RedirectToAction("NuovoArticolo");
            }

            // Se ci sono errori, ricarica la lista degli ingredienti e ritorna la vista con il modello corrente
            ViewBag.Ingredienti = _dataContext.Ingredienti.ToList();
            return View(model);
        }

        // Metodo per visualizzare la vista di modifica di un articolo esistente
        public IActionResult Edit(int id)
        {
            // Trova l'articolo per id, inclusi gli ingredienti associati
            var articolo = _dataContext.Articoli
                .Include(a => a.Ingredienti)
                .SingleOrDefault(a => a.Id == id);

            if (articolo == null)
            {
                return NotFound();
            }

            // Carica la lista degli ingredienti e crea un modello per la vista di modifica
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
        // Metodo per gestire la modifica di un articolo esistente
        public async Task<IActionResult> Edit(int id, FileUpload model, int[] selectedIngredienti)
        {
            if (ModelState.IsValid)
            {
                // Trova l'articolo da aggiornare per id, inclusi gli ingredienti associati
                var articoloToUpdate = _dataContext.Articoli
                    .Include(a => a.Ingredienti)
                    .Single(a => a.Id == id);

                if (articoloToUpdate == null)
                {
                    return NotFound();
                }

                // Aggiorna le proprietà dell'articolo
                articoloToUpdate.Nome = model.Nome;
                articoloToUpdate.PrezzoVendita = model.PrezzoVendita;
                articoloToUpdate.TempoDiConsegna = model.TempoDiConsegna;
                articoloToUpdate.Ingredienti = _dataContext.Ingredienti.Where(i => selectedIngredienti.Contains(i.Id)).ToList();

                // Gestione del file caricato
                if (model.UploadFile != null && model.UploadFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.UploadFile.CopyToAsync(memoryStream);
                        articoloToUpdate.Foto = memoryStream.ToArray();
                    }
                }

                // Salva le modifiche al contesto dati
                await _dataContext.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            // Se ci sono errori, ricarica la lista degli ingredienti e ritorna la vista con il modello corrente
            ViewBag.Ingredienti = _dataContext.Ingredienti.ToList();
            return View(model);
        }

        // Metodo per visualizzare la vista di cancellazione di un articolo esistente
        public IActionResult Delete(int id)
        {
            // Trova l'articolo per id, inclusi gli ingredienti associati
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
        // Metodo per gestire la cancellazione di un articolo
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Trova l'articolo per id, inclusi gli ingredienti associati
            var articolo = _dataContext.Articoli
                .Include(a => a.Ingredienti)
                .SingleOrDefault(a => a.Id == id);

            if (articolo == null)
            {
                return NotFound();
            }

            // Rimuove l'articolo dal contesto dati e salva le modifiche
            _dataContext.Articoli.Remove(articolo);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction("NuovoArticolo", "Articolo");
        }
    }
}
