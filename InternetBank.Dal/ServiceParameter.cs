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
    
    public partial class ServiceParameter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ValueType { get; set; }
        public string DefaultValue { get; set; }
        public int ServiceId { get; set; }
        public Nullable<int> AccountId { get; set; }
    
        public virtual Service Service { get; set; }
        public virtual Account Account { get; set; }
    }
}