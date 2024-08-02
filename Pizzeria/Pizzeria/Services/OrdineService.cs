using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pizzeria.Models;

namespace Pizzeria.Services
{
    // La classe OrdineService implementa l'interfaccia IOrdineService e fornisce metodi per la gestione degli ordini
    public class OrdineService : IOrdineService
    {
        private readonly DataContext _context;

        // Il costruttore accetta un'istanza di DataContext per accedere al database
        public OrdineService(DataContext context)
        {
            _context = context;
        }

        // Metodo per aggiungere un prodotto a un ordine
        public async Task AddProdotto(int ordineId, int articoloId, int quantita)
        {
            // Trova l'ordine e l'articolo corrispondenti dagli ID forniti
            var ordine = await _context.Ordini.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == ordineId);
            var articolo = await _context.Articoli.FindAsync(articoloId);

            // Se l'ordine e l'articolo esistono, crea un nuovo OrderItem e aggiungilo all'ordine
            if (ordine != null && articolo != null)
            {
                var orderItem = new OrderItem
                {
                    Ordini = ordine,
                    Articoli = articolo,
                    Quantita = quantita
                };

                ordine.Items.Add(orderItem);
                await _context.SaveChangesAsync(); // Salva le modifiche nel database
            }
        }

        // Metodo per ottenere un ordine per ID
        public async Task<Ordine> GetOrdineById(int ordineId)
        {
            return await _context.Ordini
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Articoli) // Includi gli articoli associati agli order items
                .FirstOrDefaultAsync(o => o.Id == ordineId);
        }

        // Metodo per aggiornare la quantità di un articolo in un ordine
        public async Task AggiornaQuantita(int ordineId, int articoloId, int quantita)
        {
            var ordine = await _context.Ordini.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == ordineId);
            if (ordine != null)
            {
                var ordineItem = ordine.Items.FirstOrDefault(item => item.Articoli.Id == articoloId);
                if (ordineItem != null)
                {
                    ordineItem.Quantita = quantita;
                    await _context.SaveChangesAsync(); // Salva le modifiche nel database
                }
            }
        }

        // Metodo per ottenere tutti gli ordini
        public async Task<IEnumerable<Ordine>> GetAll()
        {
            return await _context.Ordini
                                 .Include(o => o.User) // Includi l'utente associato all'ordine
                                 .Include(o => o.Items)
                                     .ThenInclude(i => i.Articoli) // Includi gli articoli associati agli order items
                                 .ToListAsync();
        }

        // Metodo per aggiornare lo stato di evasione di un ordine
        public async Task<bool> UpdateOrdineEvaso(int ordineId, bool ordineEvaso)
        {
            var ordine = await _context.Ordini.FindAsync(ordineId);
            if (ordine == null)
            {
                return false; // Se l'ordine non esiste, ritorna false
            }

            ordine.OrdineEvaso = ordineEvaso;
            await _context.SaveChangesAsync(); // Salva le modifiche nel database
            return true; // Ritorna true per indicare che l'operazione ha avuto successo
        }
    }
}
