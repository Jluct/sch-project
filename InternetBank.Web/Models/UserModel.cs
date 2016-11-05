using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace InternetBank.Models
{
    [DataContract]
    public class UserModel
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Login { get; set; }
        [DataMember]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Password { get; set; }
        [DataMember]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string FIO { get; set; }
        [DataMember]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PassportNumber { get; set; }
        [DataMember]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PassportData { get; set; }
        [DataMember]
        public bool IsBlocked { get; set; }
        [DataMember]
        public bool IsActivated { get; set; }
        [DataMember]
        public bool IsAdmin { get; set; }
        [DataMember]
        public int CityId { get; set; }
    }
}