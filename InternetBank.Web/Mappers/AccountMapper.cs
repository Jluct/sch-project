using Common;
using InternetBank.Dal;
using InternetBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InternetBank.Mappers
{
    public class AccountMapper : Mapper<Account, AccountModel>
    {
        DateTime unix = new DateTime(1970, 1, 1);

        public override Account MapToEntity(AccountModel model)
        {
            var entity = base.MapToEntity(model);
            entity.CloseDate = unix.AddSeconds(model.CloseDate);
            entity.OpenDate = unix.AddSeconds(model.OpenDate);
            return entity;
        }

        public override Account MapToEntity(AccountModel model, ref Account entity)
        {
            entity = base.MapToEntity(model, ref entity);
            entity.CloseDate = unix.AddSeconds(model.CloseDate);
            entity.OpenDate = unix.AddSeconds(model.OpenDate);
            return entity;
        }

        public override ICollection<Account> MapToEntityList(ICollection<AccountModel> models)
        {
            var result = new List<Account>();
            foreach (var model in models)
            {
                var entity = MapToEntity(model);
                entity.CloseDate = unix.AddSeconds(model.CloseDate);
                entity.OpenDate = unix.AddSeconds(model.OpenDate);
                result.Add(entity);
            }
            return result;
        }

        public override AccountModel MapToModel(Account entity)
        {
            var model = base.MapToModel(entity);
            return model;
        }

        public override IList<AccountModel> MapToModelList(ICollection<Account> entities)
        {
            var result = new List<AccountModel>();
            foreach (var entity in entities)
            {
                var model = MapToModel(entity);
                model.CloseDate = (long)(entity.CloseDate - unix).TotalSeconds;
                model.OpenDate = (long)(entity.OpenDate - unix).TotalSeconds;
                result.Add(model);
            }
            return result;
        }
    }
}