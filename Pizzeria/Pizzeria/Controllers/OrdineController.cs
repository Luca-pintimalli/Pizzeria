﻿using Microsoft.AspNetCore.Mvc;
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

        var ordineItem = ordine.Items
            .FirstOrDefault(item => item.Articoli?.Id == articoloId);

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


    [HttpPost]
    public async Task<IActionResult> CreaOrdine(string indirizzo, string note)
    {
        var userId = 2; // Assumi un utente con ID 2 per semplicità

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

}

