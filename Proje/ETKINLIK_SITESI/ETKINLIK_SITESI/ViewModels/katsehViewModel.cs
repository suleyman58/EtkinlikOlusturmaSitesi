using System.Collections.Generic;
using ETKINLIK_SITESI.Models;

namespace ETKINLIK_SITESI.ViewModels
{
    public class katsehViewModel
    {
        public IEnumerable<EtkinlikKategori>  Wkategori { get; set; }
        public IEnumerable<Sehirler>  Wsehir { get; set; }
    }
}
