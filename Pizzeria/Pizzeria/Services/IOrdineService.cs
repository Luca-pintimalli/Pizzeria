using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pizzeria.Models;

namespace Pizzeria.Services
{
    public interface IOrdineService
    {
        Task AddProdotto(int ordineId, int articoloId, int quantita);
        Task<Ordine> GetOrdineById(int ordineId);
        Task<IEnumerable<Ordine>> GetAll();
        Task <bool> UpdateOrdineEvaso(int ordineId, bool ordineEvaso);



    }
}