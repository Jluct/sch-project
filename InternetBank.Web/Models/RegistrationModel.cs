using InternetBank.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InternetBank.Models
{
    public class RegistrationModel : LogOnModel
    {
        [Required(ErrorMessageResourceType = typeof(UserResources), ErrorMessageResourceName = "PassportNumberRequired")]
        [Display(Name = "PassportNumber")]
        public string PassportNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(UserResources), ErrorMessageResourceName = "PassRequired")]
        [DataType(DataType.Password)]
        [Display(Name = "RepeatPassword")]
        [Compare("Password", ErrorMessageResourceType = typeof(UserResources), ErrorMessageResourceName = "PasswordNotConfirmed")]
        public string RepeatPassword { get; set; }
    }
}