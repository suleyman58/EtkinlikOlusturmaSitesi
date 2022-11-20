using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using ETKINLIK_SITESI.ViewModels;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;
using ETKINLIK_SITESI.Models;

namespace ETKINLIK_SITESI.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        DbEtkinlikContext context = new DbEtkinlikContext();
        
        public IActionResult Index()
        {
            

            var query = from e in context.Etkinliklers
                        where e.Onay == true && e.EtkinlikTarihi >= DateTime.Today
                        select new
                        {

                            e.EtkinlikId,
                            e.EtkinlikAdi,
                            e.EtkinlikTarihi,
                            e.SonBasvuru,
                            e.Aciklama,
                            e.Sehir,
                            e.Adres,
                            e.Kontenjan,
                            e.OluşturanId,
                            e.Ucret
                        };

            return View(query);
        }
        
        public IActionResult EtkinlikTalep()
        {
            
            var uyeler = context.Etkinliklers.Where(a=>a.Onay == false && a.EtkinlikTarihi >= DateTime.Today).ToList();
            return View(uyeler);
        }
        
        public IActionResult EtkinlikOnay(int id)
        {


            var sorgu = context.Etkinliklers.Find(id);
            sorgu.Onay=true;
            context.SaveChanges();
            Bildirimler bldrm = new Bildirimler();
            bldrm.EtkinlikId = id;
            bldrm.OlusturanId = sorgu.OluşturanId;
            bldrm.Bildirim = "Etkinliğiniz onaylanmıştır.";
            context.Bildirimlers.Add(bldrm);
            context.SaveChanges();
            return RedirectToAction("EtkinlikTalep", "Admin");

        }
        
        public IActionResult EtkinlikSil(int id)
        {
            var sorgu = context.Etkinliklers.Find(id);
            context.Etkinliklers.Remove(sorgu);
            context.SaveChanges();

            Bildirimler bldrm = new Bildirimler();
            bldrm.EtkinlikId = id;
            bldrm.OlusturanId = sorgu.OluşturanId;
            bldrm.Bildirim = "Etkinliğiniz onaylanmamıştır.";
            context.Bildirimlers.Add(bldrm);
            context.SaveChanges();

            return RedirectToAction("EtkinlikTalep", "Admin");

        }
       
        public IActionResult Duzenleme()
        {
            katsehViewModel vm = new katsehViewModel();
            vm.Wsehir = context.Sehirlers.ToList();
            vm.Wkategori = context.EtkinlikKategoris.ToList();
            return View(vm);
        }

        [HttpPost]
        public IActionResult KategoriSil(int id)
        {
            try
            {
                var sorgu = context.EtkinlikKategoris.Find(id);
                context.EtkinlikKategoris.Remove(sorgu);
                context.SaveChanges();
                return Json(new { status = "success", message = "Başarıyla Silindi" });
            }
            catch (Exception)
            {
                return Json(new { status = "error", message = "İşlem Başarısız" });
            }
           
        }

        [HttpPost]
        public IActionResult KategoriEkle(EtkinlikKategori et)
        {
            context.EtkinlikKategoris.Add(et);
            context.SaveChanges();
            return RedirectToAction("Duzenleme", "Admin");

        }

        public IActionResult SehirSil(int id)
        {

            var sorgu = context.Sehirlers.Find(id);
            context.Sehirlers.Remove(sorgu);
            context.SaveChanges();
            return RedirectToAction("Duzenleme", "Admin");
        }
        [HttpPost]
        public IActionResult SehirEkle(Sehirler s)
        {
            context.Sehirlers.Add(s);
            context.SaveChanges();
            return RedirectToAction("Duzenleme", "Admin");


        }

        public async Task<IActionResult> Cikis()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Varsayilan");
        }

    }
}
