using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Job_Seeker.Models
{
    public class Register
    {
        public int Userid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string ResetPasswordCode { get; set;}
    }
}