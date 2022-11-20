using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ETKINLIK_SITESI.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using ETKINLIK_SITESI.ViewModels;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Xml.Linq;
using Microsoft.CodeAnalysis.Scripting;

namespace ETKINLIK_SITESI.Controllers
{
    [Authorize(Roles = "Kullanıcı")]
    public class KullaniciController : Controller
    {
       

        DbEtkinlikContext context = new DbEtkinlikContext();

        public IActionResult Index(int id)
        {
            var bytes = default(byte[]);
            HttpContext.Session.TryGetValue("id", out bytes);
            var content = Encoding.UTF8.GetString(bytes);
            int kulid = Convert.ToInt32(content);


            var query = from e in context.Etkinliklers
                        where e.Onay == true && e.OluşturanId != kulid && e.EtkinlikTarihi >= DateTime.Today
                        select new
                        {

                            e.EtkinlikId,
                            e.EtkinlikAdi,
                            e.EtkinlikTarihi,
                            e.SonBasvuru,
                            e.Aciklama,
                            e.Sehir,
                            e.Adres,
                            e.Biletli,
                            e.Kontenjan,
                            e.Ucret
                        };

            return View(query);
        }

        public IActionResult etkinliklerim()
        {
            var bytes = default(byte[]);
            HttpContext.Session.TryGetValue("id", out bytes);
            var content = Encoding.UTF8.GetString(bytes);
            int kulid = Convert.ToInt32(content);


            var query = from e in context.Etkinliklers
                        where e.OluşturanId == kulid && e.Onay == true
                        select e;


            return View(query);
        }

        public ActionResult profil(int id, string Mail)
        {


            var bytes = default(byte[]);
            HttpContext.Session.TryGetValue("id", out bytes);
            var content = Encoding.UTF8.GetString(bytes);
            int kulid = Convert.ToInt32(content);




            var sorgu = from e in context.Uyelers
                        where e.UyeId == kulid
                        select new
                        {
                            e.UyeId,
                            e.UyeAdi,
                            e.UyeSoyAdi,
                            e.Mail

                        };
            return View(sorgu);
        }

        public async Task<IActionResult> Cikis()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Varsayilan");
        }


        public IActionResult EtkinlikOlustur()
        {


            List<SelectListItem> etk = (from e in context.EtkinlikKategoris
                                        select new SelectListItem()
                                        {
                                            Text = e.KategoriAd,
                                            Value = e.KategoriId.ToString()

                                        }).ToList();
            ViewBag.ktgr = etk;

            List<SelectListItem> shr = (from e in context.Sehirlers
                                        select new SelectListItem()
                                        {
                                            Text = e.Isim,
                                            Value = e.Plaka.ToString()

                                        }).ToList();

            ViewBag.shrl = shr;
            return View();
        }

        [HttpPost]
        public ActionResult EtkinlikOlustur(Etkinlikler e)
        {
            if (ModelState.IsValid)
            {
                var bytes = default(byte[]);
                HttpContext.Session.TryGetValue("id", out bytes);
                var content = Encoding.UTF8.GetString(bytes);
                int kulid = Convert.ToInt32(content);

                e.OluşturanId = kulid;

                var isim = context.Sehirlers.Where(a => a.Plaka.ToString() == e.Sehir).FirstOrDefault();
                e.Sehir = isim.Isim;
                var kategry = context.EtkinlikKategoris.Where(a => a.KategoriId.ToString() == e.EtkinlikAdi).FirstOrDefault();
                e.EtkinlikAdi = kategry.KategoriAd;


                context.Etkinliklers.Add(e);
                context.SaveChanges();
                TempData["AlertMessage"] = "Etkinlik başarılı şekilde oluşturuldu";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["AlertMessage"] = "Etkinlik oluşturulamadı";
                return RedirectToAction("Index");
            }
            
        }


        public IActionResult EtkinlikKayıt(int id)
        {
            EtFirViewModel ef = new EtFirViewModel();


            ef.Wetkinlik = context.Etkinliklers.Where(a => a.EtkinlikId == id).ToList();
            ef.Wfirma = (from e in context.Etkinliklers
                        join fe in context.FirmaEtkinliks
                        on e.EtkinlikId equals fe.EtkinlikId
                        join fi in context.Firmalars
                        on fe.FirmaId equals fi.FirmaId
                        where e.EtkinlikId == id
                        select fi).ToList();

            //ef.Wfirma = context.Firmalars.ToList();

            return View(ef);
        }

        [HttpPost]
        public ActionResult EtkinlikKayıt(Etkinlikler etk, Katilimciliste k, int id)
        {


            k.EtkinlikId = id;

            var bytes = default(byte[]);
            HttpContext.Session.TryGetValue("id", out bytes);
            var content = Encoding.UTF8.GetString(bytes);
            int kulid = Convert.ToInt32(content);

            k.UyeId = kulid;

        

            var sorgu2 = context.Katilimcilistes.Where(a => a.EtkinlikId == id && a.UyeId == kulid).FirstOrDefault();
            var sorgu1 = context.Etkinliklers.Where(a => a.EtkinlikId == id && a.Kontenjan == 0).FirstOrDefault();
            if (sorgu2 == null && sorgu1 == null)
            {
                context.Katilimcilistes.Add(k);
                context.SaveChanges();
                var kont = context.Etkinliklers.Where(a => a.EtkinlikId == id).FirstOrDefault();
                kont.Kontenjan--;
                context.SaveChanges();
                TempData["AlertMessage"] = "Etkinliğe başarılı kayıt olundu";
                return RedirectToAction("Index");
            }
            else
            {

                TempData["AlertMessage"] = "Etkinliğe daha önceden kayıt oldunuz veya etkinlik boş kontenjanı kontrol edin";
                return RedirectToAction("Index");

            }
            return View();


            //return null;
        }



        public IActionResult UyeDuzenle(int id,Uyeler uyes,string Mail)
        {
            Uyeler uye = new Uyeler();
            //uye = context.Uyelers.FirstOrDefault(a => a.UyeId == id);

            uye.UyeAdi = context.Uyelers.Where(a => a.UyeId == id).Select(p =>  p.UyeAdi).FirstOrDefault();
            uye.UyeSoyAdi = context.Uyelers.Where(a => a.UyeId == id).Select(p =>  p.UyeSoyAdi).FirstOrDefault();
            uye.Mail = context.Uyelers.Where(a => a.UyeId == id).Select(p =>  p.Mail).FirstOrDefault();

            return View(uye);
        }

        [HttpPost]
        public IActionResult UyeDuzenle(Uyeler uyeg, int id)
        {


            string sifrekod = Sifreleme.Sifreleme.MD5Olustur(uyeg.Sifre);
            uyeg.Sifre = sifrekod;
            uyeg.SifreTekrar = sifrekod;


            var eski = context.Uyelers.Where(a => a.UyeId == id).FirstOrDefault();
            eski.UyeAdi = uyeg.UyeAdi;
            eski.UyeSoyAdi = uyeg.UyeSoyAdi;
            eski.Sifre = uyeg.Sifre;
            eski.SifreTekrar = uyeg.SifreTekrar;
            context.SaveChanges();
            TempData["AlertMessage"] = " Güncelleme başarılı yapıldı ";
            return RedirectToAction("profil");
            return View();
        }

        public IActionResult EtkinlikDuzenle(int id)
        {

            Etkinlikler sec = context.Etkinliklers.Where(a => a.EtkinlikId == id).FirstOrDefault();

            string basvuru = sec.SonBasvuru.ToString();
            DateTime fark = DateTime.Parse(basvuru).Date;
            DateTime sonduzenleme = fark.AddDays(-5);


            if (sonduzenleme >= DateTime.Today)
            {
                List<SelectListItem> etk = (from e in context.EtkinlikKategoris
                                            select new SelectListItem()
                                            {
                                                Text = e.KategoriAd,
                                                Value = e.KategoriId.ToString()

                                            }).ToList();
                ViewBag.ktgr = etk;

                List<SelectListItem> shr = (from e in context.Sehirlers
                                            select new SelectListItem()
                                            {
                                                Text = e.Isim,
                                                Value = e.Plaka.ToString()

                                            }).ToList();

                ViewBag.shrl = shr;
                return View(sec);
            }
            else
            {
                return RedirectToAction("etkinliklerim", "Kullanici");
            }



        }

        [HttpPost]
        public IActionResult EtkinlikDuzenle(Etkinlikler etkng, int id)
        {


            var eski = context.Etkinliklers.Where(a => a.EtkinlikId == id).FirstOrDefault();
            eski.EtkinlikAdi = etkng.EtkinlikAdi;
            eski.EtkinlikTarihi = etkng.EtkinlikTarihi;
            eski.SonBasvuru = etkng.SonBasvuru;
            eski.Aciklama = etkng.Aciklama;
            eski.Sehir = etkng.Sehir;
            eski.Adres = etkng.Adres;
            eski.Kontenjan = etkng.Kontenjan;
            eski.Ucret = etkng.Ucret;
            context.SaveChanges();
            TempData["AlertMessage"] = "Düzenleme işlemi başarılı ";
            return RedirectToAction("etkinliklerim");

        }

        public IActionResult EtkinlikSil(int id)
        {

            Etkinlikler sec = context.Etkinliklers.Where(a => a.EtkinlikId == id).FirstOrDefault();

            string basvuru = sec.SonBasvuru.ToString();
            DateTime fark = DateTime.Parse(basvuru).Date;
            DateTime sonduzenleme = fark.AddDays(-5);

            if (sonduzenleme >= DateTime.Today)
            {
                var sorgu = context.Etkinliklers.Find(id);
                context.Etkinliklers.Remove(sorgu);
                context.SaveChanges();
                TempData["AlertMessage"] = "Etkinlik silmesi başarılı ";
                return RedirectToAction("etkinliklerim");
            }
            else
            {
                TempData["AlertMessage"] = "Etkinlik silmesi başarısız.Son düzenleme tarihi geçmiş olabilir.";
                return RedirectToAction("etkinliklerim");
            }





        }

        public IActionResult KayıtOlunanEtkinlikler()
        {
            var bytes = default(byte[]);
            HttpContext.Session.TryGetValue("id", out bytes);
            var content = Encoding.UTF8.GetString(bytes);
            int kulid = Convert.ToInt32(content);

            var sorgu = from e in context.Etkinliklers
                        join ke in context.Katilimcilistes
                        on e.EtkinlikId equals ke.EtkinlikId
                        where ke.UyeId == kulid && e.EtkinlikTarihi >= DateTime.Today
                        select e;
            return View(sorgu);

        }

        public IActionResult GecmisEtkinlikler()
        {
            var bytes = default(byte[]);
            HttpContext.Session.TryGetValue("id", out bytes);
            var content = Encoding.UTF8.GetString(bytes);
            int kulid = Convert.ToInt32(content);

            var sorgu = from e in context.Etkinliklers
                        join ke in context.Katilimcilistes
                        on e.EtkinlikId equals ke.EtkinlikId
                        where ke.UyeId == kulid && e.EtkinlikTarihi < DateTime.Today
                        select e;
            return View(sorgu);

        }

        public IActionResult KatılmaIptal(int id)
        {
            var bytes = default(byte[]);
            HttpContext.Session.TryGetValue("id", out bytes);
            var content = Encoding.UTF8.GetString(bytes);
            int kulid = Convert.ToInt32(content);

           
            var sorgu2 = context.Katilimcilistes.Where(a => a.EtkinlikId == id && a.UyeId == kulid).FirstOrDefault();
            if (sorgu2 != null)
            {
                // burayı var uyeler değil de k yı kullanarak yap 
                var sorgu = context.Katilimcilistes.Find(id,kulid);
                context.Katilimcilistes.Remove(sorgu);
                context.SaveChanges();
                var kont = context.Etkinliklers.Where(a => a.EtkinlikId == id).FirstOrDefault();
                kont.Kontenjan++;
                context.SaveChanges();
                TempData["AlertMessage"]= "Etkinlik katılımın iptal edildi";
                return RedirectToAction("etkinliklerim");





            }
            else
            {
            
                TempData["AlertMessage"] = " Hata etkinlik katılımın iptal edilemedi";
                return RedirectToAction("etkinliklerim");

            }



        }

        public IActionResult Bildirimler()
        {
            var bytes = default(byte[]);
            HttpContext.Session.TryGetValue("id", out bytes);
            var content = Encoding.UTF8.GetString(bytes);
            int kulid = Convert.ToInt32(content);

            var sorgu = context.Bildirimlers.Where(a => a.OlusturanId == kulid).ToList();



            return View(sorgu);

        }
            

    }

}
