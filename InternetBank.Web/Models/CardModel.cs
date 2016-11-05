using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace InternetBank.Models
{
    [DataContract]
    public class CardModel
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Number { get; set; }
        [DataMember]
        public long CreateDate { get; set; }
        [DataMember]
        public long ExpireDate { get; set; }
        [DataMember]
        public int AccountId { get; set; }
        [DataMember]
        public int CardTypeId { get; set; }
        [DataMember]
        public bool IsBlocked { get; set; }
        [DataMember]
        public bool IsAttached { get; set; }
        [DataMember]
        public bool IsExpired { get; set; }
    }
}