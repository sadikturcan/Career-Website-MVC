using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IntProgFinalProject.Models.Entity;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;

namespace IntProgFinalProject.Controllers
{
    public class CompanyController : Controller
    {
        // GET: Company
        [Authorize]
        public ActionResult Index()
        {
            TBLUSERLOGIN u;
            u = Session["company"] as TBLUSERLOGIN;
            ViewBag.sirketkullanici = u.Name;
            return View();
        }

        ProjeOdeviEntities db = new ProjeOdeviEntities();
        [Authorize]
        public ActionResult Ilanlarım()
        {

            TBLUSERLOGIN u;
            u = Session["company"] as TBLUSERLOGIN;
            var compname = u.Name;
            var comp = db.TBLCOMPANIES.Where(x => x.Name.Contains(compname.ToString())).ToList();
            ViewBag.compid = comp[0].ID;
            ViewBag.compsector = comp[0].Sector;
            var ilanlar = db.TBLJOBOPENINGS.Where(x => x.TBLCOMPANIES.Name.Contains(compname.ToString())).ToList();
            
          
            return View(ilanlar);
        }
        [Authorize]
        public ActionResult Profil()
        {
            TBLUSERLOGIN u;
            u = Session["company"] as TBLUSERLOGIN;
            ViewBag.sirketadi = u.Name;
           
            ViewBag.sirketmail = u.Email;
            return View();
        }
        //ILANI GETİRME
        [Authorize]
        public ActionResult Ilangetir(int id)
        {
            var kisilistesi = db.TBLAPPLICATIONS.Where(x => x.JobID == id).ToList();
            if (kisilistesi.Count!=0) { 
            //ViewBag.kisiad = kisilistesi[0].TBLUSERS.Name;
            //ViewBag.kisisoyad = kisilistesi[0].TBLUSERS.Surname;
            //ViewBag.kisiemail = kisilistesi[0].TBLUSERS.Email;
            ViewBag.kisiler = kisilistesi;
            }
            var ilan = db.TBLJOBOPENINGS.Where(x => x.ID == id).ToList();
            ViewBag.JOBID=ilan[0].ID;
            //var files = GetFiles();
            //foreach(var file in files)
            //{
            //    if (!file.Contains(kisilistesi[0].TBLUSERS.Name))
            //    {
            //        files.Remove(file);
            //    }
            //}
            List<string> indirilecek = new List<string>();
            
            //if (files.Count > 0)
            //{

            //    ViewBag.basvurucvler = files;

            //}
            var kisisayisi = kisilistesi.Count;
            for(int i=0;i<kisisayisi;i++)
            {
                indirilecek.Add(GetFile(kisilistesi[i].TBLUSERS.Name.ToString()));
            }
            if (indirilecek.Count > 0) {
                ViewBag.basvurucvler = indirilecek;
            }
            


            return View(ilan);
        }
        public FileResult Indır(string filename)
        {
            var FileVirtualPath = "~/Resume/" + filename;
            return File(FileVirtualPath, "application/force- download", Path.GetFileName(FileVirtualPath));

        }
        public List<string> GetFiles(string username)
        {
            var dir = new System.IO.DirectoryInfo(Server.MapPath("~/Resume"));
            System.IO.FileInfo[] filenames = dir.GetFiles("*.*");

            List<string> items = new List<string>();
           
            foreach (var file in filenames)
            {
                while(file.Name.Contains(username))
                { items.Add(file.Name); }
                
            }

            return items;
        }
        public string GetFile(string username)
        {
            var dir = new System.IO.DirectoryInfo(Server.MapPath("~/Resume"));
            System.IO.FileInfo[] filenames = dir.GetFiles("*.*");

            //List<string> items = new List<string>();
            string item ="";
            foreach (var file in filenames)
            {
                if(file.Name.Contains(username))
                { item = file.Name; }

            }

            return item;
        }

        //ILAN EKLEME BOLUMU
        [HttpPost]
        [Authorize]
        public ActionResult Ilanekle(TBLJOBOPENINGS p1)
        {
            
            db.TBLJOBOPENINGS.Add(p1);
            db.SaveChanges();
            return View();
        }
        [HttpGet]
        [Authorize]
        public ActionResult Ilanekle()
        {
            
            return View();
        }
        [Authorize]
        public ActionResult Ilanguncellegetir(int id)
        {
            var guncellenecekilan = db.TBLJOBOPENINGS.Where(x => x.ID == id).ToList();
            if (guncellenecekilan == null)
            {
                return HttpNotFound();
            }
            
            return View("Ilanguncellegetir",guncellenecekilan);

        }

        
        //ILAN GUNCELLEME BOLUMU
        [Authorize]
        [HttpPost]
        public ActionResult Ilanduzenle(List<TBLJOBOPENINGS> p1)
        {
            var ilan = db.TBLJOBOPENINGS.Find(p1[0].ID);
            ilan.Header = p1[0].Header;
            
            ilan.About = p1[0].About;
            ilan.Category = p1[0].Category;
           
            db.SaveChanges();
            return RedirectToAction("Ilanlarım");
            
           

        }
        [Authorize]
        public ActionResult Basvuranlar(int id)
        {
            
            var kisilistesi = db.TBLAPPLICATIONS.Where(x => x.JobID == id).ToList();
            ViewBag.kisiad = kisilistesi[0].TBLUSERS.Name;
            ViewBag.kisisoyad = kisilistesi[0].TBLUSERS.Surname;
            ViewBag.kisiemail = kisilistesi[0].TBLUSERS.Email;
            //ViewBag.kisi = kisilistesi[0].TBLUSERS.Name;
          
            return View(kisilistesi);


        }
       
        [Authorize]
        [HttpGet]
        public ActionResult BasvuruDurum(int id)
        {
            var guncellenecekapp = db.TBLAPPLICATIONS.Where(x => x.ApplicantID == id).ToList();
            if(guncellenecekapp==null)
            {
                return HttpNotFound();
            }
            ViewBag.id = id;
            return View("BasvuruDurum", guncellenecekapp);


        }

        public ActionResult IlanSil(int id)
        {
            var ilan = db.TBLJOBOPENINGS.Find(id);
            db.TBLJOBOPENINGS.Remove(ilan);
            db.SaveChanges();
            return RedirectToAction("Ilanlarım");
        }

        [HttpPost]
        public ActionResult BasvuruDurumKaydet(List<TBLAPPLICATIONS> p1)
        {
            var guncellenecek = db.TBLAPPLICATIONS.Find(p1[0].ApplicationID);
            if(guncellenecek==null)
            {
                return HttpNotFound();
            }
            else
            {
                guncellenecek.Applicationstatus = p1[0].Applicationstatus;


            }
            db.SaveChanges();
            return RedirectToAction("Ilanlarım", "Company");

        }




    }
}

