using System;
using System.Collections.Generic;

namespace ETKINLIK_SITESI
{
    public partial class Etkinlikler
    {
        public int EtkinlikId { get; set; }
        public string? EtkinlikAdi { get; set; }
        public DateTime? EtkinlikTarihi { get; set; }
        public DateTime? SonBasvuru { get; set; }
        public string? Aciklama { get; set; }
        public bool? Onay { get; set; }
        public string? Sehir { get; set; }
        public string? Adres { get; set; }
        public int? Kontenjan { get; set; }
        public bool? Biletli { get; set; }
        public int? OluşturanId { get; set; }
        public decimal? Ucret { get; set; }
    }
}
