using System;
using System.Collections.Generic;

namespace ETKINLIK_SITESI
{
    public partial class Bildirimler
    {
        public int EtkinlikId { get; set; }
        public string? Bildirim { get; set; }
        public int? OlusturanId { get; set; }
    }
}
