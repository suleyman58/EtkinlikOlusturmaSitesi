using System;
using System.Collections.Generic;

namespace ETKINLIK_SITESI
{
    public partial class Uyeler
    {
        public int UyeId { get; set; }
        public string? UyeAdi { get; set; }
        public string? UyeSoyAdi { get; set; }
        public string? Mail { get; set; }
        public string? Sifre { get; set; }
        public string? Rol { get; set; }
        public string? SifreTekrar { get; set; }
    }
}
