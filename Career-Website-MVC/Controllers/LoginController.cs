using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using IntProgFinalProject.Models.Entity;
using System.Diagnostics;
using System.Web.Security;

namespace IntProgFinalProject.Controllers
{
    public class LoginController : Controller
    {
        ProjeOdeviEntities db = new ProjeOdeviEntities();
        // GET: Login
        public ActionResult GirisYap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GirisYap(TBLUSERLOGIN t)
        {
            var userdata = db.TBLUSERLOGIN.FirstOrDefault(x => x.Email == t.Email && x.Password == t.Password);
            
            if(userdata!=null)
            {

               
                if (userdata.Role=="Company")
                {
                    Debug.WriteLine(" company");
                    FormsAuthentication.SetAuthCookie(userdata.Name, false);
                    Session["company"] = userdata;
                    return RedirectToAction("Index", "Company");
                }
                if (userdata.Role == "User")
                {
                    Debug.WriteLine(" user");
                    FormsAuthentication.SetAuthCookie(userdata.Name, false);
                    Session["user"] = userdata;
                   
                    return RedirectToAction("Index", "User");
                }
                return RedirectToAction("Profil", "User");
            }
            else 
            {
                Debug.WriteLine("data bos");
                return View(); 
            }
            
        }


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CikisYap()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("GirisYap","Login");
        }


        [HttpGet]
        public ActionResult Register()
        {

            return View();
        }




        [HttpPost]
        public ActionResult Register(TBLUSERLOGIN u)
        {
            db.TBLUSERLOGIN.Add(u);
            db.SaveChanges();
            return View("GirisYap");
            
        }

    }
}