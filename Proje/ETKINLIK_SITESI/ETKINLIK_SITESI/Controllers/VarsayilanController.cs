using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Linq;
using ETKINLIK_SITESI.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.Serialization;
using Microsoft.CodeAnalysis.Differencing;
using System.Text;
using ETKINLIK_SITESI.Sifreleme;

namespace ETKINLIK_SITESI.Controllers
{
    public class VarsayilanController : Controller
    {
        DbEtkinlikContext context = new DbEtkinlikContext();

        public IActionResult Index()
        {

            var query = from e in context.Etkinliklers
                        where e.Onay == true
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

        public IActionResult Hakkimizda()
        {
            return View();
        }

        public IActionResult Giris()
        {



            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Giris(string Mail, string Sifre, int id)
        {

            string sifrekod = Sifreleme.Sifreleme.MD5Olustur(Sifre);
            Sifre = sifrekod;

           

            var uyeler = context.Uyelers.FirstOrDefault(uye => uye.Mail == Mail && uye.Sifre == Sifre);

           

            if (uyeler != null && uyeler.Rol == "admin")
            {
                id = uyeler.UyeId;
                var bytes = Encoding.UTF8.GetBytes(id.ToString());
                HttpContext.Session.Set("id", bytes);


                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Email,Mail),
                    new Claim(ClaimTypes.Role,uyeler.Rol)
                 };
                var useridentity = new ClaimsIdentity(claims, "GirişIndex");
                ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);
                await HttpContext.SignInAsync(principal);

                return RedirectToAction("Index", "Admin");
            }

            else if (uyeler != null && uyeler.Rol == "Kullanıcı")
            {
                id = uyeler.UyeId;
                var bytes = Encoding.UTF8.GetBytes(id.ToString());
                HttpContext.Session.Set("id", bytes);

                List<Claim> claims = new List<Claim>() {
                    new Claim(ClaimTypes.Email,Mail),
                    new Claim(ClaimTypes.Role,uyeler.Rol)

                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                AuthenticationProperties properties = new AuthenticationProperties()
                {

                    AllowRefresh = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity), properties);

                return RedirectToAction("profil", new RouteValueDictionary(new { Controller = "Kullanici", Action = "profil", Id = id }));
            }
            else
            {
               
                return RedirectToAction("Giris", "Varsayilan");
            }



        }

        public IActionResult Uyeol()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Uyeol(Uyeler u)
        {

            if (ModelState.IsValid)
            {
                string sifrekod = Sifreleme.Sifreleme.MD5Olustur(u.Sifre);
                u.Sifre = sifrekod;
                u.SifreTekrar = sifrekod;

                var kontrol = context.Uyelers.Where(k=>k.Mail == u.Mail).FirstOrDefault();
                if (kontrol == null)
                {
                    context.Uyelers.Add(u);
                    context.SaveChanges();
                    return View();
                    ViewBag.Message = "Kayıt Başarılı ";
                }
                else
                {
                    
                    ViewBag.Message = "Bu mail adresi daha onceden kayit edilmistir";
                    return View();
                }
               
            }
            else
            {
                BadRequest(ModelState);
            }
            return View();
            
        }

        public IActionResult FirmaKayıt()
        {
            return View();

        }
        [HttpPost]
        public IActionResult FirmaKayıt(Firmalar f)
        {
            if (ModelState.IsValid)
            {
                var kontrol = context.Firmalars.Where(k => k.Mail == f.Mail).FirstOrDefault();
                if (kontrol == null)
                {
                    string sifrekod = Sifreleme.Sifreleme.MD5Olustur(f.Sifre);
                    f.Sifre = sifrekod;
                    f.SifreT = sifrekod;


                    context.Firmalars.Add(f);
                    context.SaveChanges();
                    return View();
                    TempData["AlertMessage"] = "Başarılı kayıt olundu ";
                    return RedirectToAction("FirmaGiris");
                }
                else
                {

                    TempData["AlertMessage"] = "Kayıt başarısız ";
                    return RedirectToAction("FirmaGiris");
                }

            }
            else
            {
                BadRequest(ModelState);
            }
            return View();


        }

        public IActionResult FirmaGiris()
        {
            return View();

        }
    }
}



