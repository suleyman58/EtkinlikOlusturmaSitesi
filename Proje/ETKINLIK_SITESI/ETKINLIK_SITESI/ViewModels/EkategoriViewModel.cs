using Microsoft.AspNetCore.Mvc.Rendering;

namespace ETKINLIK_SITESI.ViewModels
{
    public class EkategoriViewModel
    {
        public int KategoriId { get; set; }
        public string KategoriAd { get; set; }
        public List<SelectListItem> kategoriler { get; set; }

    }
}
