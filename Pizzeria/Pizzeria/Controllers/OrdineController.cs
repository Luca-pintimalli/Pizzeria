using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pizzeria.Models;
using Pizzeria.Services;
using Pizzeria.ViewModels;
using System.Linq;
using System.Threading.Tasks;

public class OrdineController : Controller
{
    private readonly DataContext _context;

    public OrdineController(DataContext context)
    {
        _context = context;
    }

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
            Totale = totale,
            Indirizzo = ordine.Indirizzo,
            Note = ordine.Note
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AggiornaOrdine(int ordineId, string indirizzo, string note)
    {
        var ordine = await _context.Ordini.FindAsync(ordineId);
        if (ordine == null)
        {
            return NotFound("Ordine non trovato");
        }

        ordine.Indirizzo = indirizzo;
        ordine.Note = note;

        _context.Ordini.Update(ordine);
        await _context.SaveChangesAsync();

        return RedirectToAction("TotaleOrdine", new { ordineId = ordine.Id });
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

        var ordineItem = ordine.Items.FirstOrDefault(item => item.Articoli.Id == articoloId);

        if (ordineItem != null)
        {
            ordineItem.Quantita = quantita;
            _context.OrderItems.Update(ordineItem);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("TotaleOrdine", new { ordineId = ordine.Id });
    }

    [HttpPost]
    public async Task<IActionResult> RimuoviArticolo(int ordineId, int articoloId)
    {
        var ordine = await _context.Ordini
            .Include(o => o.Items)
            .ThenInclude(oi => oi.Articoli)
            .FirstOrDefaultAsync(o => o.Id == ordineId);

        if (ordine == null)
        {
            return NotFound("Ordine non trovato");
        }

        var ordineItem = ordine.Items.FirstOrDefault(item => item.Articoli.Id == articoloId);

        if (ordineItem != null)
        {
            _context.OrderItems.Remove(ordineItem);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("TotaleOrdine", new { ordineId = ordine.Id });
    }
}
