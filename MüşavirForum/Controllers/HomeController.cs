using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MüşavirForum.Context;
using MüşavirForum.Models;
using System.Data.Entity;
using System.Web.Helpers;
using System.Net.Mail;
using System.Net;

namespace MüşavirForum.Controllers
{
    public class HomeController : Controller
    {
        DatabaseContext db = new DatabaseContext();


      //  [OutputCache(Duration = 60)]
        public ActionResult Index()
        {

            if (Session["Sess_status"] == null)
            {
                ViewBag.Sess_sta = Session["Sess_status"];
                ViewBag.Kullanici = "";
            }
            else
            {
                ViewBag.Sess_sta = Session["Sess_status"];
                ViewBag.Kullanici = Session["Kullanici"];
                return RedirectToAction("Index", "Forum");
            }
            var u = db.Users.ToList();
            return View();

        }



        public ActionResult Kayit()
        {

            if (Session["Sess_status"] == null)
            {
                ViewBag.Sess_sta = Session["Sess_status"];
                ViewBag.Kullanici = "";
            }
            else
            {
                ViewBag.Sess_sta = Session["Sess_status"];
                ViewBag.Kullanici = Session["Kullanici"];
                return RedirectToAction("Index", "Home");
            }


            return View();
        }


        [HttpPost]
        public ActionResult Kayit(User Users)
        {

            if (!ModelState.IsValid)
            { //checking model state

                ViewBag.successColor = "red";
                ViewBag.successMessage = "Kayıt Başarısız";
                return View();
            }
           
            if ((Request.Form.Get("Name") == "") || (Request.Form.Get("Username") == "") || (Request.Form.Get("Email") == "")
                || (Request.Form.Get("Password") == "") || (Request.Form.Get("Re_pass") == ""))
            {
                ViewBag.successColor = "red";
                ViewBag.successMessage = "Lütfen boş alan bırakmayın ! !";
                return View();
            }

            if (Request.Form.Get("Password")!= Request.Form.Get("Re_pass")) {
                ViewBag.successColor = "red";
                ViewBag.successMessage = "Şifreler aynı olmalıdır !";
                return View();
            }
            if (Request.Form.Get("Password").Length < 6)
            {
                ViewBag.successColor = "red";
                ViewBag.successMessage = "Şifre En az 6 haneli olmalıdır !";
                return View();
            }



            var u = db.Users.ToList();
            var ret = u.FirstOrDefault(x => x.Username == Users.Username );

            if (ret != null)
            {
                ViewBag.successColor = "red";
                ViewBag.successMessage = "Kullanıcı Adı Çakışma Hatası!";
                return View();
            }

            ret = u.FirstOrDefault(x => x.Email == Users.Email);

            if (ret != null)
            {
                ViewBag.successColor = "red";
                ViewBag.successMessage = "E-Mail Adresi Çakışma Hatası!";
                return View();
            }



            Random r = new Random();
            int token = r.Next();
            string HashDegeri = Crypto.SHA256(Request.Form.Get("Password"));
            //eşit mi    bool EsitMi = Crypto.VerifyHashedPassword(HashDegeri, Sifre);


            db.Users.Add(new User
            {
                Name = Convert.ToString(Request.Form.Get("Name")),
                Username = Convert.ToString(Request.Form.Get("Username")),
                Email = Convert.ToString(Request.Form.Get("Email")),
                Password = Convert.ToString(HashDegeri),
                Token = Convert.ToString(token),
                Status = Convert.ToInt32(1)
            });

            db.SaveChanges();
            ViewBag.successColor = "green";
            ViewBag.successMessage = "Kayıt Başarılı";
            return View();

        }




        public ActionResult Giris()
        {

            if (Session["Sess_status"] == null)
            {
                ViewBag.Sess_sta = Session["Sess_status"];
                ViewBag.Kullanici = "";
            }
            else
            {
                ViewBag.Sess_sta = Session["Sess_status"];
                ViewBag.Kullanici = Session["Kullanici"];
                return RedirectToAction("Index", "Home");
            }



            return View();
        }




        [HttpPost]
        public ActionResult Giris(User Users)
        {

            if (!ModelState.IsValid)
            { //checking model state

                ViewBag.successColor = "red";
                ViewBag.successMessage = "Giriş Başarısız";
                return View();
            }



            var u = db.Users.ToList();
            string HashDegeri = Crypto.SHA256(Request.Form.Get("Password"));

            var ret = u.FirstOrDefault(x => x.Username == Users.Username  && x.Password == HashDegeri);
       

            if (ret == null)
            {

                Session["Kullanici"] = "";
                Session["Sess_status"] = null;
                Session["UserId"] = "";

                ViewBag.successColor = "red";
                ViewBag.successMessage = "Giriş Başarısız";
                return View();


            }
      
                Session["Kullanici"] = Request.Form.Get("Username");
                Session["Sess_status"] = 1;
                Session["UserId"] = ret.Id;

            ViewBag.successColor = "green";
            ViewBag.successMessage = "Giriş Başarılı";
            //  return View();
            return RedirectToAction("Index", "Home");


        }


        public ActionResult Cikis()
        {
            Session.RemoveAll();
            return RedirectToAction("Index", "Home");
        }



        public ActionResult PasswordReset()
        {

            Session["Kullanici"] = "";
            Session["Sess_status"] = null;
            Session["UserId"] = "";

            return View();
        }

        [HttpPost]
        public ActionResult PasswordReset(User user)
        {

            var u = db.Users.ToList();
            var ret = u.FirstOrDefault(x => x.Email == Request.Form.Get("Email"));

            if (ret == null)
            {

                Session["Kullanici"] = "";
                Session["Sess_status"] = null;
                Session["UserId"] = "";

                ViewBag.successColor = "red";
                ViewBag.successMessage = "E-Mail Adresi Bulunamadı";
           


            }
            else {



                Session["Kullanici"] = "";
                Session["Sess_status"] = null;
                Session["UserId"] = "";

                try
                {
                    String Token = ret.Token;
                    string url = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Content("~")+ "Home/Sifirla/"+ Token;

                    var senderEmail = new MailAddress("", "MF Şifre Sıfırlama");
                        var receiverEmail = new MailAddress(Request.Form.Get("Email"), "Receiver");
                        var password = "";
                        var sub = "Şifre Sıfırlama İsteği";
                        var body = "Şifenizi sıfırlamak için <a href='"+ url + "'>tıklayın</a>";
                        var smtp = new SmtpClient
                        {
                            Host = "smtp.yandex.com.tr",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(senderEmail.Address, password)
                        };
                        using (var mess = new MailMessage(senderEmail, receiverEmail)
                        {
                            Subject = "Şifre Sıfırlama İsteği",
                            Body = body,
                            IsBodyHtml = true
                })
                        {
                            smtp.Send(mess);
                        }
  

                    ViewBag.successColor = "green";
                    ViewBag.successMessage = "E-Mail gönderimi başarılı";

                }
                catch (Exception)
                {
                    ViewBag.successColor = "red";
                    ViewBag.successMessage = "E-Mail gönderimi başarısız";
                }
            }


            return View();
        }


        public ActionResult Sifirla(String Id)
        {

            Session["Kullanici"] = "";
            Session["Sess_status"] = null;
            Session["UserId"] = "";

            var u = db.Users.ToList();
            User ret = u.FirstOrDefault(x => x.Token == Id);

            if (ret == null)
            {

                return RedirectToAction("Index", "Home");
            }
            else
            {

                Session["Token"] = Id;
                return View();

            }
   
        }




        [HttpPost]
        public ActionResult Sifirla(User user)
        {



            if ((Request.Form.Get("Password") == "") || (Request.Form.Get("Re_pass") == ""))
            {
                ViewBag.successColor = "red";
                ViewBag.successMessage = "Lütfen boş alan bırakmayın ! !";
                return View();
            }

            if (Request.Form.Get("Password") != Request.Form.Get("Re_pass"))
            {
                ViewBag.successColor = "red";
                ViewBag.successMessage = "Şifreler aynı olmalıdır !";
                return View();
            }
            if (Request.Form.Get("Password").Length < 6)
            {
                ViewBag.successColor = "red";
                ViewBag.successMessage = "Şifre En az 6 haneli olmalıdır !";
                return View();
            }

            var Token = Session["Token"];
            var u = db.Users.ToList();
            var ret = u.FirstOrDefault(x => x.Token == Token);

            if (ret != null)
            {
                ViewBag.successColor = "red";
                ViewBag.successMessage = "Token Hatası!";
                return View();
            }


            string HashDegeri = Crypto.SHA256(Request.Form.Get("Password"));
            //eşit mi    bool EsitMi = Crypto.VerifyHashedPassword(HashDegeri, Sifre);



            User sss = db.Users.Where(k => k.Token == Token).FirstOrDefault();
            sss.Password = HashDegeri;
            db.SaveChanges();

            ViewBag.successColor = "green";
            ViewBag.successMessage = "Şifre Güncelleme Başarılı";
            return View();


        }




        public ActionResult Profil()
        {
            return View();
        }



        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }



        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }




   




    }
}

