using FluentValidation;
using ETKINLIK_SITESI.Models;
using System.Text.RegularExpressions;
using FluentValidation.AspNetCore;

namespace ETKINLIK_SITESI.Validators
{
    public class Validator : AbstractValidator<Uyeler>
    {
        public Validator()
        {

            RuleFor(a => a.UyeAdi).NotNull().WithMessage("Bu Alan Boş Bırakılamaz").MinimumLength(3).WithMessage("Ad 3 karakterden az olamaz").MaximumLength(35).WithMessage("Daha kısa bir ad yazın");
            RuleFor(a => a.UyeSoyAdi).NotNull().WithMessage("Bu Alan Boş Bırakılamaz").MinimumLength(2).WithMessage("Soyad 2 karakterden az olamaz").MaximumLength(35).WithMessage("Daha kısa bir Soyad yazın");
            RuleFor(a => a.Mail).NotNull().WithMessage("Bu Alan Boş Bırakılamaz").MaximumLength(35).WithMessage("Daha kısa bir mail yazın").EmailAddress().WithMessage("Bir mail adresi yazın");
            RuleFor(a => a.Sifre).NotNull().WithMessage("Bu Alan Boş Bırakılamaz")
                .MinimumLength(8).WithMessage("Şifre 8 karakterden az olamaz")
                .Matches("^(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{8,}$").WithMessage("Şifre Doğru Formatta Girilmedi. Örn: 1Ss*aA12")
                .Equal(x => x.SifreTekrar).WithMessage("Şifreler işleşmiyor");

        }


    }
    public class EtkinlikValidator : AbstractValidator<Etkinlikler>
    {
        public EtkinlikValidator()
        {
            RuleFor(a => a.EtkinlikTarihi).NotNull().WithMessage("Bu Alan Boş Bırakılamaz");
            RuleFor(a => a.SonBasvuru).NotNull().WithMessage("Bu Alan Boş Bırakılamaz");
            RuleFor(a => a.Adres).NotNull().WithMessage("Bu Alan Boş Bırakılamaz").MinimumLength(10).WithMessage("10 karakterden az olamaz");
            RuleFor(a => a.Kontenjan).NotNull().WithMessage("Bu Alan Boş Bırakılamaz").GreaterThan(0).WithMessage("Kontenjan 0 dan fazla olmalıdır");
            RuleFor(a => a.Ucret).NotNull().WithMessage("Bu Alan Boş Bırakılamaz").GreaterThan(-1).WithMessage("Ucret 0 dan az  olmaz");
            RuleFor(a => a.Biletli).NotNull().WithMessage("Bu Alan Boş Bırakılamaz");
            RuleFor(a => a.Aciklama).NotNull().WithMessage("Bu Alan Boş Bırakılamaz").MinimumLength(10).WithMessage("10 karakterden az olamaz");
           


        }
    }
    public class FirmaValidator : AbstractValidator<Firmalar>
    {
        public FirmaValidator()
        {
            RuleFor(a => a.FirmaAdı).NotNull().WithMessage("Bu Alan Boş Bırakılamaz").MinimumLength(3).WithMessage("Ad 3 karakterden az olamaz").MaximumLength(49).WithMessage("Daha kısa bir ad yazın");
            RuleFor(a => a.WebSite).NotNull().WithMessage("Bu Alan Boş Bırakılamaz").MinimumLength(10).WithMessage("Websitesi 10 karakterden az olamaz").MaximumLength(49).WithMessage("Daha kısa bir Soyad yazın");
            RuleFor(a => a.Mail).NotNull().WithMessage("Bu Alan Boş Bırakılamaz").MaximumLength(49).WithMessage("Daha kısa bir mail yazın").EmailAddress().WithMessage("Bir mail adresi yazın");
            RuleFor(a => a.Sifre).MinimumLength(8).WithMessage("Şifre 8 karakterden az olamaz").Equal(x => x.SifreT).WithMessage("Şifreler işleşmiyor").NotNull().WithMessage("Bu Alan Boş Bırakılamaz");
        }
    }



}
        