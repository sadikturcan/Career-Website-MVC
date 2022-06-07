using IntProgFinalProject.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Remoting.Contexts;
using System.IO;
using System.Diagnostics;

namespace IntProgFinalProject.Controllers
{
    public class UserController : Controller
    {
        ProjeOdeviEntities db = new ProjeOdeviEntities();
        Context c = new Context();
        // GET: User
        [Authorize]
        public ActionResult Index()
        {
            TBLUSERLOGIN u;
            u= Session["User"] as TBLUSERLOGIN;
            ViewBag.kisiselkullanici = u.Name;
            return View();
        }

        // Kullanıcı Profil Sayfası
        [Authorize]
        public ActionResult Profil()
        {

            TBLUSERLOGIN u;
            u = Session["User"] as TBLUSERLOGIN;
            ViewBag.kisiselkullaniciadi = u.Name;
            ViewBag.kisiselkullanicisoyadi = u.Surname;
            ViewBag.kisiselkullaniciemail = u.Email;
            var file = GetFile(u.Name);
            var image = GetImage(u.Name);
            ViewBag.filename = file.ToString();
            if(image==null)
            {
                ViewBag.imagepath = "";
            }
            else
            {
                ViewBag.imagepath = image.ToString();
            }
          

            return View();
            
        }
        public string GetFile(string username)
        {
            var dir = new System.IO.DirectoryInfo(Server.MapPath("~/Resume"));
            System.IO.FileInfo[] filenames = dir.GetFiles("*.*");

            //List<string> items = new List<string>();
            string item = "";
            foreach (var file in filenames)
            {
                if (file.Name.Contains(username))
                { item = file.Name; }

            }

            return item;
        }
        [HttpPost]
        public ActionResult ImageUpload(HttpPostedFileBase file)
        {
            TBLUSERLOGIN u;
            u = Session["User"] as TBLUSERLOGIN;

            if (file != null && file.ContentLength > 0)
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Image"), u.Name + Path.GetFileName(file.FileName));
                    file.SaveAs(path);

                }
                catch (Exception e)
                {

                }


            return RedirectToAction("Profil");
        }
        public string GetImage(string username)
        {
            var dir = new System.IO.DirectoryInfo(Server.MapPath("~/Image"));
            System.IO.FileInfo[] filenames = dir.GetFiles("*.*");

            //List<string> items = new List<string>();
            string item = "";
            foreach (var file in filenames)
            {
                if (file.Name.Contains(username))
                { item = file.Name; }

            }

            return item;
        }

        // Ilanları Listele
        [Authorize]
        public ActionResult Ilanlar(string hdr=null,string ctgr=null)
        {
            var ilanlar = from d in db.TBLJOBOPENINGS select d;
            if(!string.IsNullOrEmpty(hdr)&&!string.IsNullOrEmpty(ctgr))
            {
                ilanlar = ilanlar.Where(x => x.Header.Contains(hdr)&&x.Category.Contains(ctgr));
              
                
            }
            if (string.IsNullOrEmpty(hdr) && !string.IsNullOrEmpty(ctgr))
            {
                ilanlar = ilanlar.Where(x =>x.Category.Contains(ctgr));


            }
            if (!string.IsNullOrEmpty(hdr) && string.IsNullOrEmpty(ctgr))
            {
                ilanlar = ilanlar.Where(x => x.Header.Contains(hdr));


            }
           
            return View(ilanlar.ToList());
            //var ilanlar = db.TBLJOBOPENINGS.ToList();
            //return View(ilanlar);
        }
        [Authorize]
        public ActionResult Ilandetay(int id)
        {
            var detay = db.TBLJOBOPENINGS.Where(x => x.ID == id).ToList();
            TBLUSERLOGIN u;
            u = Session["user"] as TBLUSERLOGIN;
            var user = db.TBLUSERS.Where(x => x.Name.Contains(u.Name.ToString())).ToList();
            ViewBag.userid = user[0].ID;

            return View(detay);
        }
        [Authorize]
        //Basvurduklarımı listele
        public ActionResult Basvurularim()
        {
            TBLUSERLOGIN u;
            u = Session["user"] as TBLUSERLOGIN;
            var username = u.Name;
            var basvurular = db.TBLAPPLICATIONS.Where(x=>x.TBLUSERS.Name.Contains(username.ToString())).ToList();


           

            return View(basvurular);
        }
        [Authorize]
        // Favorilediklerimi listele
        public ActionResult Favorilerim()
        {
            return View();
        }
        [Authorize]
        //ilan başvurma link
        [HttpPost]
        public ActionResult IlanBasvur(TBLAPPLICATIONS application)
        {
            db.TBLAPPLICATIONS.Add(application);
            db.SaveChanges();
            return RedirectToAction("Ilanlar");


            
            
            
            
        }
        
        //Özgeçmiş Ekleme

        [HttpPost]
        public ActionResult OzgecmisEkle(HttpPostedFileBase file)
        {
            TBLUSERLOGIN u;
            u = Session["User"] as TBLUSERLOGIN;
            //if (Request.Files.Count > 0)
            //{
            //    string filename = Path.GetFileName(Request.Files[0].FileName);
            //    string extension = Path.GetExtension(Request.Files[0].FileName);
            //    string filepath = "~/Resume/" + filename + extension;
            //    Request.Files[0].SaveAs(Server.MapPath(filepath));
            //    p.Resume = "/Resume/" + filename + extension;

            //}
            string path = Server.MapPath("~/App_Data/Resume");
            string filename = Path.GetFileName(file.FileName);
            string fullpath = Path.Combine(path, filename);
            file.SaveAs(fullpath);
            
            

          
            //var user = db.TBLUSERS.Find(u.Name);
            //user.Resume = p.Resume;
            //db.SaveChanges();
            return RedirectToAction("Profil");
        }

        public ActionResult OzgecmisEkle()
        {
            return View();
        }

        public ActionResult KullaniciGetir()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            TBLUSERLOGIN u;
            u = Session["User"] as TBLUSERLOGIN;

            if (file!=null && file.ContentLength>0)
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Resume"),u.Name + Path.GetFileName(file.FileName));
                    file.SaveAs(path);

                }
                catch (Exception e)
                {

                }

            
            return RedirectToAction("Profil");
        }

        public FileResult Download(string filename)
        {
            var FileVirtualPath = "~/Resume/" + filename;
            return File(FileVirtualPath, "application/force- download", Path.GetFileName(FileVirtualPath));

        }
        public List<string> GetFiles()
        {
            var dir = new System.IO.DirectoryInfo(Server.MapPath("~/Resume"));
            System.IO.FileInfo[] filenames = dir.GetFiles("*.*");

            List<string> items = new List<string>();
            foreach(var file in filenames)
            {
                items.Add(file.Name);
            }

            return items;
        }
        




    }
}