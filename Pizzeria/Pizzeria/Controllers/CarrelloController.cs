using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pizzeria.Models;
using Pizzeria.Services;
using Pizzeria.ViewModels;

public class CarrelloController : Controller
{
    private readonly DataContext _context;

    public CarrelloController(DataContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Assumi che l'ID dell'ordine sia 1 per semplicità
        var ordineId = 1;

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
}
