using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pizzeria.Models;
using Pizzeria.Services;
using Pizzeria.ViewModels;

public class OrdineController : Controller
{
    private readonly DataContext _context;
    private readonly IOrdineService _ordineService;

    public OrdineController(DataContext context, IOrdineService ordineService)
    {
        // Inizializzo il contesto dati e il servizio degli ordini tramite dependency injection
        _context = context;
        _ordineService = ordineService;
    }

    // Metodo per calcolare il totale di un ordine specifico
    [HttpGet]
    public async Task<IActionResult> TotaleOrdine(int ordineId)
    {
        var ordine = await _context.Ordini
            .Include(o => o.Items)
            .ThenInclude(oi => oi.Articoli)
            .FirstOrDefaultAsync(o => o.Id == ordineId);

        if (ordine == null)
        {
            return NotFound("Ordine non trovato");
        }

        var totale = ordine.Items.Sum(item => item.Articoli.PrezzoVendita * item.Quantita);
        var viewModel = new TotaleOrdineViewModel
        {
            Ordine = ordine,
            Totale = totale
        };

        return View(viewModel);
    }

    // Metodo per aggiungere un prodotto a un ordine
    [HttpPost]
    public async Task<IActionResult> AddProdotto(int articoloId, int quantita)
    {
        var ordineId = 1; // Assumo che l'ID dell'ordine sia predefinito a 1 per semplicità
        var ordine = await _context.Ordini.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == ordineId);
        var articolo = await _context.Articoli.FindAsync(articoloId);

        if (ordine != null && articolo != null)
        {
            var orderItem = new OrderItem
            {
                Ordini = ordine,
                Articoli = articolo,
                Quantita = quantita
            };

            ordine.Items.Add(orderItem);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Home"); // Reindirizza alla vista Index del controller Home
    }

    // Metodo per aggiornare la quantità di un prodotto in un ordine
    [HttpPost]
    public async Task<IActionResult> AggiornaQuantita(int ordineId, int articoloId, int quantita)
    {
        var ordine = await _context.Ordini
            .Include(o => o.Items)
            .ThenInclude(oi => oi.Articoli)
            .FirstOrDefaultAsync(o => o.Id == ordineId);

        if (ordine == null)
        {
            return NotFound("Ordine non trovato");
        }

        var ordineItem = ordine.Items.FirstOrDefault(item => item.Articoli?.Id == articoloId);

        if (ordineItem != null)
        {
            if (quantita <= 0)
            {
                return BadRequest("La quantità deve essere maggiore di zero");
            }

            ordineItem.Quantita = quantita;
            _context.OrderItems.Update(ordineItem);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("TotaleOrdine", new { ordineId = ordine.Id });
    }

    // Metodo per rimuovere un articolo da un ordine
    [HttpPost]
    public async Task<IActionResult> RimuoviArticolo(int ordineId, int articoloId, int quantita)
    {
        var ordine = await _context.Ordini
            .Include(o => o.Items)
            .ThenInclude(oi => oi.Articoli)
            .FirstOrDefaultAsync(o => o.Id == ordineId);

        if (ordine == null)
        {
            return NotFound("Ordine non trovato");
        }

        var ordineItem = ordine.Items.FirstOrDefault(item => item.Articoli?.Id == articoloId);

        if (ordineItem != null)
        {
            if (ordineItem.Quantita <= quantita)
            {
                // Se la quantità dell'articolo nel carrello è minore o uguale a quella da rimuovere, rimuoviamo l'articolo
                ordine.Items.Remove(ordineItem);
            }
            else
            {
                // Altrimenti, riduciamo la quantità
                ordineItem.Quantita -= quantita;
                _context.OrderItems.Update(ordineItem);
            }

            await _context.SaveChangesAsync();
        }

        return RedirectToAction("TotaleOrdine", new { ordineId });
    }

    // Metodo per creare un nuovo ordine
    [HttpPost]
    public async Task<IActionResult> CreaOrdine(string indirizzo, string note)
    {
        var userId = 2; // Assumo un utente con ID 2 per semplicità

        var nuovoOrdine = new Ordine
        {
            DataOrdine = DateTime.Now,
            User = await _context.Users.FindAsync(userId),
            OrdineEvaso = false,
            Indirizzo = indirizzo,
            Note = note,
            Items = new List<OrderItem>() // Inizializza la lista degli articoli
        };

        _context.Ordini.Add(nuovoOrdine);
        await _context.SaveChangesAsync();

        // Rimuovi gli articoli dal carrello corrente se necessario
        // Puoi implementare la logica per mantenere o rimuovere il carrello precedente

        return RedirectToAction("TotaleOrdine", new { ordineId = nuovoOrdine.Id });
    }

    // Metodo per visualizzare la lista degli ordini
    public async Task<IActionResult> ListaOrdini()
    {
        var ordini = await _context.Ordini
            .Include(o => o.User)
            .Include(o => o.Items)
            .ThenInclude(i => i.Articoli) // Assicurati di includere gli articoli
            .ToListAsync();

        if (ordini == null || !ordini.Any())
        {
            return View("ListaOrdini", new List<Ordine>());
        }

        return View("ListaOrdini", ordini);
    }

    // Metodo per aggiornare lo stato di evasione di un ordine
    [HttpPost]
    [Route("Ordine/UpdateOrdineEvaso")]
    public async Task<IActionResult> UpdateOrdineEvaso([FromBody] UpdateOrdineEvasoRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { success = false, message = "Dati non validi" });
        }

        bool result = await _ordineService.UpdateOrdineEvaso(request.OrdineId, request.OrdineEvaso);

        if (result)
        {
            return Ok(new { success = true });
        }
        else
        {
            return NotFound(new { success = false, message = "Ordine non trovato" });
        }
    }

    // Metodo per visualizzare gli ordini evasi
    public IActionResult OrdiniEvasi()
    {
        var ordiniEvasi = _ordineService.GetAll().Result.Where(o => o.OrdineEvaso);
        return View(ordiniEvasi);
    }
}
