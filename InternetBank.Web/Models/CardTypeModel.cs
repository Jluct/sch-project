﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace InternetBank.Models
{
    [DataContract]
    public class CardTypeModel
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Name { get; set; }
        [DataMember]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string IconName { get; set; }
    }
}