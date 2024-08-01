using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pizzeria.Models;
using Pizzeria.Services;
using Pizzeria.ViewModels;

public class OrdineController : Controller
{
    private readonly DataContext _context;

    public OrdineController(DataContext context)
    {
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
            if (ordine.Items == null)
            {
                ordine.Items = new List<OrderItem>();
            }

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

        var viewModel = new TotaleOrdineViewModel
        {
            Ordine = ordine,
            Totale = totale
        };

        return View(viewModel); // Passa il ViewModel alla vista
    }


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

        if (ordine.Items == null)
        {
            return BadRequest("L'ordine non contiene articoli.");
        }

        var ordineItem = ordine.Items.FirstOrDefault(item => item.Articoli.Id == articoloId);
        if (ordineItem == null)
        {
            return NotFound("Articolo non trovato nell'ordine.");
        }

        ordineItem.Quantita = quantita;
        await _context.SaveChangesAsync();

        return RedirectToAction("TotaleOrdine", new { ordineId = ordineId });
    }




    [HttpPost]
    public async Task<IActionResult> RimuoviArticolo(int ordineId, int articoloId)
    {
        var ordine = await _context.Ordini
            .Include(o => o.Items)
            .ThenInclude(i => i.Articoli)
            .FirstOrDefaultAsync(o => o.Id == ordineId);

        if (ordine != null && ordine.Items != null)
        {
            var ordineItem = ordine.Items.FirstOrDefault(item => item.Articoli.Id == articoloId);
            if (ordineItem != null)
            {
                ordine.Items.Remove(ordineItem);
                await _context.SaveChangesAsync();
            }
        }

        return RedirectToAction("TotaleOrdine", new { ordineId = ordineId });
    }
}
