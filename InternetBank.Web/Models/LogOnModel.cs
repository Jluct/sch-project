using InternetBank.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace InternetBank.Models
{
    public class LogOnModel
    {
        [Required(ErrorMessageResourceType = typeof(UserResources), ErrorMessageResourceName = "LoginRequired")]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserResources), ErrorMessageResourceName = "PassRequired")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
