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
    
    public partial class Operation
    {
        public int Id { get; set; }
        public System.DateTime Date { get; set; }
        public string Data { get; set; }
        public decimal Sum { get; set; }
        public int CardId { get; set; }
        public int ServiceId { get; set; }
    
        public virtual Card Card { get; set; }
        public virtual Service Service { get; set; }
    }
}
