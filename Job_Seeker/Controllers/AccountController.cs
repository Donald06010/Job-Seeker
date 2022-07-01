using Job_Seeker.DAL;
using Job_Seeker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
                Session["IsSuperAdmin"] = UserDetails.IsSuperAdmin;
                Session["userName"] = UserDetails.Username;
                Session["Email"] = UserDetails.Email;
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
            if (Session.Count > 0)
            {
                if (Convert.ToBoolean(Session["IsSuperAdmin"]) != false)
                {
                    ViewBag.Message = "Edit Jobs";
                }
                else
                {
                    ViewBag.Message = "All jobs";
                }
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
        public ActionResult Userlist()
        {
            if (Session.Count > 0)
            {
                if (Convert.ToBoolean(Session["IsSuperAdmin"]) != false)
                {
                    ViewBag.Message = "Edit Jobs";
                }
                else
                {
                    ViewBag.Message = "All jobs";
                }
                return View();
            }
            return RedirectToAction("Login");
        }
        public ActionResult EditProfile()
        {
            if (Session.Count > 0)
            {
                return View();
            }
            return RedirectToAction("Login");
        }
        public ActionResult Jobs()
        {
            if (Convert.ToBoolean(Session["IsSuperAdmin"]) != false)
            {
                ViewBag.Message = "Edit Jobs";
            }
            else
            {
                ViewBag.Message = "All jobs";
            }
            return View();
        }
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {
            //Verify Email ID
            //Generate Reset password link 
            //Send Email 
            string message = "";
            bool status = false;

            using (JobSystemEntities dc = new JobSystemEntities())
            {
                var account = dc.Registers.Where(a => a.Email == Email).FirstOrDefault();
                if (account.Email != null)
                {
                    //Send email for reset password
                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(account.Email, resetCode, "ResetPassword");
                    account.ResetPasswordCode = resetCode;
                    //This line I have added here to avoid confirm password not match issue , as we had added a confirm password property 
                    //in our model class in part 1
                    dc.Configuration.ValidateOnSaveEnabled = false;
                    dc.SaveChanges();
                    message = "Reset password link has been sent to your email id.";
                }
                else
                {
                    message = "Account not found";
                }
            }
            ViewBag.Message = message;
            return View();
        }
        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/Account/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("nikhilkateliya.sanskaribalak@gmail.com", "Dotnet Awesome");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "eiubeoyepftozasy"; // Replace with actual password

            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created!";
                body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
                    " successfully created. Please click on the below link to verify your account" +
                    " <br/><br/><a href='" + link + "'>" + link + "</a> ";
            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hi,<br/><br/>We got request for reset your account password. Please click on the below link to reset your password" +
                    "<br/><br/><a href=" + link + ">Reset Password link</a>";
            }


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
        public ActionResult ResetPassword(string id)
        {
            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            using (JobSystemEntities dc = new JobSystemEntities())
            {
                var user = dc.Registers.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
                if (user != null)
                {
                    ResetPasswordModel model = new ResetPasswordModel();
                    model.ResetCode = id;
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                using (JobSystemEntities dc = new JobSystemEntities())
                {
                    var user = dc.Registers.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                    if (user != null)
                    {
                        user.Password = model.NewPassword;
                        user.ConfirmPassword = model.ConfirmPassword;
                        user.ResetPasswordCode = "";
                        dc.Configuration.ValidateOnSaveEnabled = false;
                        dc.SaveChanges();
                        message = "New password updated successfully";
                    }
                }
            }
            else
            {
                message = "Something invalid";
            }
            ViewBag.Message = message;
            return View(model);
        }
    }
}