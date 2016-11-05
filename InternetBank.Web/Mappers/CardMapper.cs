using Common;
using InternetBank.Dal;
using InternetBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InternetBank.Mappers
{
    public class CardMapper : Mapper<Card, CardModel>
    {
        DateTime unix = new DateTime(1970, 1, 1);

        public override Card MapToEntity(CardModel model)
        {
            var entity = base.MapToEntity(model);
            entity.CreateDate = unix.AddSeconds(model.CreateDate);
            entity.ExpireDate = unix.AddSeconds(model.ExpireDate);
            return entity;
        }

        public override Card MapToEntity(CardModel model, ref Card entity)
        {
            entity = base.MapToEntity(model, ref entity);
            entity.CreateDate = unix.AddSeconds(model.CreateDate);
            entity.ExpireDate = unix.AddSeconds(model.ExpireDate);
            return entity;
        }

        public override ICollection<Card> MapToEntityList(ICollection<CardModel> models)
        {
            var result = new List<Card>();
            foreach (var model in models)
            {
                var entity = MapToEntity(model);
                entity.CreateDate = unix.AddSeconds(model.CreateDate);
                entity.ExpireDate = unix.AddSeconds(model.ExpireDate);
                result.Add(entity);
            }
            return result;
        }

        public override CardModel MapToModel(Card entity)
        {
            var model = base.MapToModel(entity);
            return model;
        }

        public override IList<CardModel> MapToModelList(ICollection<Card> entities)
        {
            var result = new List<CardModel>();
            foreach (var entity in entities)
            {
                var model = MapToModel(entity);
                model.CreateDate = (long)(entity.CreateDate - unix).TotalSeconds;
                model.ExpireDate = (long)(entity.ExpireDate - unix).TotalSeconds;
                result.Add(model);
            }
            return result;
        }
    }
}