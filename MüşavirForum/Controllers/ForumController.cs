using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MüşavirForum.Context;
using MüşavirForum.Models;
using System.Data.Entity;
using System.Collections;
using System.Text;
using System.Globalization;

namespace MüşavirForum.Controllers
{
    public class ForumController : Controller
    {

        DatabaseContext db = new DatabaseContext();


        // GET: Forum
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
            }


       
            //Sıralama , görüntüleme , konu
            var ct = db.Categories.ToList();
            var tp = db.Topics.ToList();


            ViewBag.TopicNumber = tp.Count();
            ViewData["Categories"] = ct.Where(n => n.Status == 1 );
            int[] counts = new int[5000];
            int k = 0;   foreach (Category ct2 in ct)
            {

                int Cats = ct2.CategoryId;
                counts[k] = tp.Where(s=>s.CategoryId == Cats).Count();
                k++; 
            }
            ViewBag.Category_number = counts;/**/

            ViewData["Users"] = db.Users.ToList().OrderByDescending(number => number.Point).Take(10).ToList();

            return View();
        }

        [HttpGet]
        public ActionResult Kategori(String Id , String sort)
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
            }


           if (Id == "") { return RedirectToAction("Index", "Forum"); }

            var ct = db.Categories.ToList().Where(n => n.Seo == Id);

           if (ct.Count() == 0) { return RedirectToAction("Index", "Forum"); }


            foreach (Category ct2 in ct)
            {

                var Cats = ct2.CategoryId;

                var tp = db.Topics.ToList();
                if (sort == "date") { tp = db.Topics.ToList().OrderByDescending(number => number.TopicId).ToList(); }

                


                ViewBag.Topics = tp.Where(n => n.Status == 1 && n.CategoryId == Cats);


     
            }

               String [] names = new String[50000];
                int [] yorum = new int[50000];
            int k = 0;

                foreach (Topic ts in ViewBag.Topics)
                {
                names [k] = Convert.ToString(ts.User.Name);
                yorum [k] = db.Contents.ToList().Where(n => n.TopicId == ts.TopicId).Count();

                k++;
                }

            ViewBag.name = Id;
            ViewBag.usernames = names;
            ViewBag.yorum_Adet = yorum;
            ViewData["Topics"] = ViewBag.Topics;
            return View();

        }

        public ActionResult Detay(String Id)
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
            }


            if (Id == "") { return RedirectToAction("Index", "Forum"); }

            var konu = db.Topics.ToList().Where(n => n.Seo == Id && n.Status == 1);

            if (konu.Count() == 0) { return RedirectToAction("Index", "Forum"); }



            int count = 0;
            int k = 0; foreach (Topic tpc in konu)
            {

                int tpcid = tpc.TopicId;
                var yorumlar = db.Contents.ToList().OrderByDescending(number => number.Point).ToList(); 
                count = yorumlar.Where(s => s.TopicId == tpcid).Count();
                ViewBag.Contents = yorumlar.Where(s => s.TopicId == tpcid);
                k++;
            }

            int[] cnts = new int[500000];
            Content[] cntic = new Content[500000];

            int r = 0; foreach (Content cnt in ViewBag.Contents)
            {

                IEnumerable<Point> po = db.Points.ToList().Where(n => n.ContentId == cnt.ContentId);
                cnts[r] = po.Count();
                cntic[r] = cnt;
                cntic[r].Point = po.Count();
                r++;
            }



            ViewBag.cnts = cnts;
            ViewBag.cntic = cntic;

            db.Topics.ToList();
            ViewBag.Topic_number = count;/**/
            ViewBag.Topic_seo = Id;/**/
            ViewData["Contents"] = ViewBag.Contents;
            ViewData["Topics"] = konu;

            return View();
        }


        public ActionResult KonuEkle()
        {
            if (Session["Sess_status"] == null)
            {
                ViewBag.Sess_sta = Session["Sess_status"];
                ViewBag.Kullanici = "";
                return RedirectToAction("Index", "Forum");
            }
            else
            {
                ViewBag.Sess_sta = Session["Sess_status"];
                ViewBag.Kullanici = Session["Kullanici"];
            }

            ViewData["Categories"] =  db.Categories.ToList();

            return View();
        }


        [HttpPost]
        public ActionResult KonuEkle(Topic Topics)
        {

            if (Session["Sess_status"] == null)
            {
                ViewBag.Sess_sta = Session["Sess_status"];
                ViewBag.Kullanici = "";
                return RedirectToAction("Index", "Forum");
            }
            else
            {
                ViewBag.Sess_sta = Session["Sess_status"];
                ViewBag.Kullanici = Session["Kullanici"];
            }

            ViewData["Categories"] = db.Categories.ToList();

            if (!ModelState.IsValid)
            { //checking model state

                ViewBag.successColor = "red";
                ViewBag.successMessage = "Kayıt Başarısız";
                return View();
            }


            if (Convert.ToInt32(Request.Form.Get("CategoryId")) == 0)
            {
                ViewBag.successColor = "red";
                ViewBag.successMessage = "Lütfen Kategori Seçiniz!";
                return View();
            }





            db.Topics.Add(new Topic
            {
                CategoryId = Convert.ToInt32(Request.Form.Get("CategoryId")),
                Content = Convert.ToString(Request.Form.Get("Content")),
                Content_Description = Convert.ToString(Request.Form.Get("Content_Description")),
                UserId = Convert.ToInt32(Session["UserId"]),
                Status = Convert.ToInt32(1),
                Date = Convert.ToString(DateTime.Now),
                Seo = FriendlyURLTitle(Convert.ToString(Request.Form.Get("Content")))
  
    });

            db.SaveChanges();
            ViewBag.successColor = "green";
            ViewBag.successMessage = "Kayıt Başarılı";
            return View();

        }






        [HttpPost]
        public ActionResult Detay(Content Contents)
        {
     
            if (Session["Sess_status"] == null)
            {
                ViewBag.Sess_sta = Session["Sess_status"];
                ViewBag.Kullanici = "";
                return RedirectToAction("Index", "Forum");
            }
            else
            {
                ViewBag.Sess_sta = Session["Sess_status"];
                ViewBag.Kullanici = Session["Kullanici"];
            }

      

            db.Contents.Add(new Content
            {
                UserId = Convert.ToInt32(Session["UserId"]),
                TopicId = Convert.ToInt32(Request.Form.Get("topicId")),
                ContentDetail = Convert.ToString(Request.Form.Get("ContentDetail")),
                Date = Convert.ToString(DateTime.Now),
                Status = Convert.ToInt32(1)
            });

            db.SaveChanges();
            ViewBag.successColor = "green";
            ViewBag.successMessage = "Kayıt Başarılı";

            return RedirectToAction("Detay/"+ Request.Form.Get("seo") , "Forum");




        }

        [HttpPost]
        public ActionResult YorumAl()
        {
                 if (Session["Sess_status"] == null)
                 {
                     ViewBag.Sess_sta = Session["Sess_status"];
                     ViewBag.Kullanici = "";
                     return RedirectToAction("Index", "Forum");
                 }
                 else
                 {
                     ViewBag.Sess_sta = Session["Sess_status"];
                     ViewBag.Kullanici = Session["Kullanici"];
                 }

            var data = db.Points.ToList().Where(z => z.UserId == Convert.ToInt32(Session["UserId"]) && z.ContentId == Convert.ToInt32(Request.Form.Get("contId")));

            if (data != null)
            {
            

                foreach (Point p in data)
                {
                    return Content("error");
                    db.Points.Remove(p);
                    db.SaveChanges();
                   return Content("Puan Silindi");
                }

            }


        
            db.Points.Add(new Point
            {
                UserId = Convert.ToInt32(Session["UserId"]),
                ContentId = Convert.ToInt32(Request.Form.Get("contId")),
                Date = Convert.ToString(DateTime.Now),
                PointVal = Convert.ToInt32(1)
            });
            db.SaveChanges();


            int contid = Convert.ToInt32(Request.Form.Get("contId"));
            int adet = db.Points.ToList().Where(f => f.ContentId == contid ).Count();
            Content sss = db.Contents.Where(k => k.ContentId == contid).FirstOrDefault();
            sss.Point = adet;
            db.SaveChanges();

            int usId = sss.UserId;
            int tot = 0;
            var User_conts = db.Contents.ToList().Where(k => k.UserId == Convert.ToInt32(Session["UserId"]));
            foreach (Content cc in User_conts)
            {
                tot = tot + cc.Point;
            }

            int uid = Convert.ToInt32(Session["UserId"]);
            User uss = db.Users.Where(k => k.Id == uid).FirstOrDefault();
            uss.Point = tot;
            db.SaveChanges();


            return Content(Convert.ToString(adet));

        }







        [HttpPost]
        public ActionResult Ara()
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
            }
            
           
            String se = Request.Form.Get("se");



            if (se.Length > 3) {

                String[] names = new String[50000];
                int[] yorum = new int[50000];

                var tp = db.Topics.Where(s => s.Content.Contains(se) || s.Content_Description.Contains(se)).ToList();
         
         
            ViewBag.Topics = tp;
            ViewBag.adet = tp.Count();

            int k = 0;

            foreach (Topic ts in ViewBag.Topics)
            {
                names[k] = Convert.ToString(ts.User.Name);
                yorum[k] = db.Contents.ToList().Where(n => n.TopicId == ts.TopicId).Count();

                k++;
            }
         

            ViewBag.name = Request.Form.Get("se");
            ViewBag.usernames = names;
            ViewBag.yorum_Adet = yorum;
            ViewData["Topics"] = ViewBag.Topics;

            }
            else
            {

                String[] names = new String[50000];
                int[] yorum = new int[50000];

            
                var tp = db.Topics.Where(s => s.TopicId < 1 ).ToList();
                ViewBag.Topics = tp;
                ViewBag.adet = 0;

                int k = 0;

                foreach (Topic ts in ViewBag.Topics)
                {
                    names[k] = Convert.ToString(ts.User.Name);
                    yorum[k] = db.Contents.ToList().Where(n => n.TopicId == ts.TopicId).Count();

                    k++;
                }


                ViewBag.name = "Lütfen en az 3 hanei bir arama terimi kullanınız.";
                ViewBag.usernames = names;
                ViewBag.yorum_Adet = yorum;
                ViewData["Topics"] = ViewBag.Topics;

            }


            return View();

        }


        public static string FriendlyURLTitle(string pTitle)
        {
            pTitle = pTitle.Replace(" ", "-");
            pTitle = pTitle.Replace(".", "-");
            pTitle = pTitle.Replace("ı", "i");
            pTitle = pTitle.Replace("İ", "I");

            pTitle = String.Join("", pTitle.Normalize(NormalizationForm.FormD) // türkçe karakterleri ingilizceye çevir.
                    .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));

            pTitle = HttpUtility.UrlEncode(pTitle);
            return System.Text.RegularExpressions.Regex.Replace(pTitle, @"\%[0-9A-Fa-f]{2}", "");
        }


    }
}