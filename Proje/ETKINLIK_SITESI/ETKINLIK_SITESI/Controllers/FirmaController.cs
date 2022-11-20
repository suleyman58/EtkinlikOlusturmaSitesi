using ETKINLIK_SITESI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using System;
using System.Security.Claims;
using System.Text;

namespace ETKINLIK_SITESI.Controllers
{
    public class FirmaController : Controller
    {
        DbEtkinlikContext context = new DbEtkinlikContext();

        public IActionResult FirIndex()
        {
           
            var bytes = default(byte[]);
            HttpContext.Session.TryGetValue("firid", out bytes);
            var content = Encoding.UTF8.GetString(bytes);
            int SFirmaId = Convert.ToInt32(content);

            var sorgu = context.Etkinliklers.Where(a => a.Onay == true && a.Biletli == true && a.EtkinlikTarihi >= DateTime.Today).ToList();

            return View(sorgu);

        }


        public IActionResult FirFirmaKayit(int id)
        {
            var sorgu = context.Etkinliklers.Where(a=>a.EtkinlikId==id).FirstOrDefault();
            return View(sorgu);

        }

        [HttpPost]
        public IActionResult FirFirmaKayit(FirmaEtkinlik firmaetk,int id)
        {



            var bytes = default(byte[]);
            HttpContext.Session.TryGetValue("firid", out bytes);
            var content = Encoding.UTF8.GetString(bytes);
            int SFirmaId = Convert.ToInt32(content);

            firmaetk.EtkinlikId = id;
            firmaetk.FirmaId = SFirmaId;


            var sorguf = context.FirmaEtkinliks.Where(a => a.EtkinlikId == id && a.FirmaId == SFirmaId).FirstOrDefault();
            if (sorguf == null)
            {
                context.FirmaEtkinliks.Add(firmaetk);
                context.SaveChanges();
                TempData["AlertMessage"] = "Başarılı kayıt olundu ";
                return RedirectToAction("FirIndex");

            }
            else
            {

                TempData["AlertMessage"] = "Bu etkinlige daha onceden kayit oldunuz";
                return RedirectToAction("FirIndex");

            }
      
        }

        public IActionResult FirGiris()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FirGiris(string Mail, string Sifre, int firid)
        {
            string sifrekod = Sifreleme.Sifreleme.MD5Olustur(Sifre);
            Sifre = sifrekod;



            var firma1 = context.Firmalars.FirstOrDefault(ff => ff.Mail == Mail && ff.Sifre == Sifre);
            if (firma1 != null)
            {
                firid = firma1.FirmaId;
                var bytes = Encoding.UTF8.GetBytes(firid.ToString());
                HttpContext.Session.Set("firid", bytes);


                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Email,Mail)

                 };
                var useridentity = new ClaimsIdentity(claims, "GirişIndex");
                ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);

                await HttpContext.SignInAsync(principal);

                return RedirectToAction("FirIndex", "Firma");

            }
            else
            {
                return RedirectToAction("FirGiris", "Firma");
            }
        }

        public async Task<IActionResult> Cikis()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Varsayilan");
        }

        public IActionResult Firprofil()
        {
            return View();

        }

        public IActionResult Firetkinliklerim()
        {
            var bytes = default(byte[]);
            HttpContext.Session.TryGetValue("firid", out bytes);
            var content = Encoding.UTF8.GetString(bytes);
            int SFirmaId = Convert.ToInt32(content);


            var sorgu = from e in context.Etkinliklers
                        join fe in context.FirmaEtkinliks
                        on e.EtkinlikId equals fe.EtkinlikId
                        where fe.FirmaId == SFirmaId && e.EtkinlikTarihi >= DateTime.Today
                        select e;
            return View(sorgu);
            

        }

    }
}
