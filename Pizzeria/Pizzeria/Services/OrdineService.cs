using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pizzeria.Models;

namespace Pizzeria.Services
{
    public class OrdineService : IOrdineService
    {
        private readonly DataContext _context;

        public OrdineService(DataContext context)
        {
            _context = context;
        }

        public async Task AddProdotto(int ordineId, int articoloId, int quantita)
        {
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
        }

        public async Task<Ordine> GetOrdineById(int ordineId)
        {
            return await _context.Ordini
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Articoli)
                .FirstOrDefaultAsync(o => o.Id == ordineId);
        }

     



        public async Task AggiornaQuantita(int ordineId, int articoloId, int quantita)
        {
            var ordine = await _context.Ordini.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == ordineId);
            if (ordine != null)
            {
                var ordineItem = ordine.Items.FirstOrDefault(item => item.Articoli.Id == articoloId);
                if (ordineItem != null)
                {
                    ordineItem.Quantita = quantita;
                    await _context.SaveChangesAsync();
                }
            }
        }


        public async Task<IEnumerable<Ordine>> GetAll()
        {
            return await _context.Ordini
                                 .Include(o => o.User)
                                 .Include(o => o.Items)
                                     .ThenInclude(i => i.Articoli)
                                 .ToListAsync();
        }



        public async Task<bool> UpdateOrdineEvaso(int ordineId, bool ordineEvaso)
        {
            var ordine = await _context.Ordini.FindAsync(ordineId);
            if (ordine == null)
            {
                return false;
            }

            ordine.OrdineEvaso = ordineEvaso;
            await _context.SaveChangesAsync();
            return true;
        }



    }
}
