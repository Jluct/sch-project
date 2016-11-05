//------------------------------------------------------------------------------
// <auto-generated>
//    Этот код был создан из шаблона.
//
//    Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//    Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InternetBank.Dal
{
    using System;
    using System.Collections.Generic;
    
    public partial class Card
    {
        public Card()
        {
            this.Operations = new HashSet<Operation>();
        }
    
        public int Id { get; set; }
        public string Number { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime ExpireDate { get; set; }
        public int AccountId { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsAttached { get; set; }
        public bool IsExpired { get; set; }
        public int CardTypeId { get; set; }
    
        public virtual Account Account { get; set; }
        public virtual ICollection<Operation> Operations { get; set; }
        public virtual CardType CardType { get; set; }
    }
}