using Job_Seeker.DAL;
using Job_Seeker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Job_Seeker.Controllers
{
    public class AccountController : Controller
    {
        private JobSystemEntities db = new JobSystemEntities();
       // List<Models.Register> listRegister = null;
        // GET: Account
        public ActionResult Index()
        {
            //listRegister = new List<Models.Register>();
            var list = db.Registers.ToList()[1];
            //foreach(var item in list)
            //{
                Models.Register register = new Models.Register();
            register.Userid = list.Userid;
            register.Name = list.Name;
            register.Email = list.Email;
            register.Username = list.Username;
            register.Password = list.Password;
            register.ConfirmPassword = list.ConfirmPassword;

            //    listRegister.Add(register);
            //}
            return View(register);
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Models.Login login)
        {
            var UserDetails = db.Registers.FirstOrDefault(x => x.Username == login.Username && x.Password == login.Password);//.Where(x => x.Username == login.Username && x.Password == login.Password);
            if (UserDetails != null)
            {
                Session["userName"] = UserDetails.Username;
                return RedirectToAction("Dashboard");
            }
            ViewBag.Message = "Username or password is not valid";
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Models.Register register)
        {
            DAL.Register dalRegister = new DAL.Register();
            dalRegister.Name = register.Name;
            dalRegister.Email = register.Email;
            dalRegister.Username = register.Username;
            dalRegister.Password = register.Password;
            dalRegister.ConfirmPassword = register.ConfirmPassword;
            db.Registers.Add(dalRegister);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Dashboard()
        {
            if(Session.Count > 0)
            {
                return View();
            }
            return RedirectToAction("Login");
        }
        public ActionResult Logout()
        {
            if (Session != null)
            {
                Session.RemoveAll();//.Remove("Username");
            }
          

            return RedirectToAction("Login");
        }
    }
}