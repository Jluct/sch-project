using InternetBank.Dal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace InternetBank.Models
{
    [DataContract]
    public class OperationMenuItemModel
    {
        [DataMember]
        public int Id { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public bool IsService { get; set; }
        [DataMember]
        public int? ParentId { get; set; }
        [DataMember]
        public int? LocalityTypeId { get; set; }
        [DataMember]
        public int? LocalityId { get; set; }
        [DataMember]
        public int? LocalityInternalId { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DataMember]
        public string ScriptName { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DataMember]
        public string TemplateName { get; set; }
        [DataMember]
        public List<OperationMenuItemModel> Children;
        
        public OperationMenuItemModel()
        {
            Children = new List<OperationMenuItemModel>();
        }

        public OperationMenuItemModel(ServiceCategory category)
            : this()
        {
            Id = category.Id;
            Name = category.Name;
            IsService = false;
            ParentId = category.ServiceCategoryId;
            LocalityTypeId = category.Locality == null ? (int?)null : category.Locality.LocalityType;
            LocalityId = category.Locality == null ? (int?)null : category.Locality.LocalityId;
            LocalityInternalId = category.Locality == null ? (int?)null : category.Locality.Id;
            Children = category.ServiceCategories.Select(categ => new OperationMenuItemModel(categ)).ToList();
            Children.AddRange(category.Services.Select(service => new OperationMenuItemModel(service)));
        }

        public OperationMenuItemModel(Service service)
            :this()
        {
            Id = service.Id;
            Name = service.Name;
            IsService = true;
            ParentId = service.ServiceCategoryId;
            LocalityTypeId = service.Locality == null ? (int?)null : service.Locality.LocalityType;
            LocalityId = service.Locality == null ? (int?)null : service.Locality.LocalityId;
            LocalityInternalId = service.Locality == null ? (int?)null : service.Locality.Id;
            ScriptName = service.ScriptName;
            TemplateName = service.TemplateName;
        }
    }
}