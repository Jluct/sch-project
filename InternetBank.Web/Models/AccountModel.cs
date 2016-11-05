using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace InternetBank.Models
{
    [DataContract]
    public class AccountModel
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Number { get; set; }
        [DataMember]
        public long OpenDate { get; set; }
        [DataMember]
        public long CloseDate { get; set; }
        [DataMember]
        public decimal Balance { get; set; }
        [DataMember]
        public decimal MinBalance { get; set; }
        [DataMember]
        public int Currency { get; set; }
        [DataMember]
        public bool IsOpened { get; set; }
    }
}