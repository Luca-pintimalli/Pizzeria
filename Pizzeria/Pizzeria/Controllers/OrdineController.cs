using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pizzeria.Models;
using Pizzeria.Services;

public class OrdineController : Controller
{
    private readonly IOrdineService _ordineService;
    private readonly DataContext _context;

    public OrdineController(IOrdineService ordineService, DataContext context)
    {
        _ordineService = ordineService;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddProdotto(int articoloId, int quantita)
    {
        var ordineId = 1; // Assumi che l'ID dell'ordine sia predefinito a 1 per semplicità
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

    [HttpGet("TotaleOrdine")]
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
        ViewData["Totale"] = totale;

        return View(ordine); // Assicurati che esista una vista TotaleOrdine.cshtml nella cartella Views/Ordine
    }
}