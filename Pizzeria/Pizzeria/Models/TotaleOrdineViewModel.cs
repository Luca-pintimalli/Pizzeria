using Pizzeria.Models;

namespace Pizzeria.ViewModels
{
    public class TotaleOrdineViewModel
    {
        public Ordine Ordine { get; set; }
        public decimal Totale { get; set; }



        public string Indirizzo { get; set; }
        public string? Note { get; set; }

    }
}