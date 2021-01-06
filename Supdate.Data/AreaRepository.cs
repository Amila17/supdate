using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Supdate.Data.Base;
using Supdate.Model;
using Supdate.Util;

namespace Supdate.Data
{
  public class AreaRepository : CrudRepository<Area>, IAreaRepository
  {
    public override Area Create(Area model)
    {
      try
      {
        OpenConnection();
        model.Id = Connection.Execute("AreaCreate", new { companyId = model.CompanyId, name = model.Name },
          commandType: CommandType.StoredProcedure);

        return model;
      }
      finally
      {
        CloseConnection();
      }
    }

    public override Area Update(Area model)
    {
      var modelCheck = GetArea(model.CompanyId, model.UniqueId);
      if (modelCheck.Id == model.Id)
      {
        return base.Update(model);
      }
      return model;
    }

    public override bool Delete(int id)
    {
      try
      {
        OpenConnection();

        return Connection.Execute("AreaDeleteById", new { areaId = id }, commandType: CommandType.StoredProcedure) > 0;
      }
      finally
      {
        CloseConnection();
      }
    }

    public Area GetArea(int companyId, Guid uniqueId)
    {
      try
      {
        OpenConnection();
        var results = Connection.QueryMultiple("AreaGet", new { companyId, uniqueId }, commandType: CommandType.StoredProcedure);
        var area = results.Read<Area>().FirstOrDefault();
        return area;
      }
      finally
      {
        CloseConnection();
      }
    }

    public void DeleteArea(int companyId, Guid uniqueId)
    {
      var area = GetArea(companyId, uniqueId);
      Delete(area.Id);
    }

    public void SaveDisplayOrder(int companyId, IList<EntityDisplayOrder> displayOrders)
    {
      try
      {
        var orderData = ConversionUtil.EntityDisplayOrderToDataTable(displayOrders);
        OpenConnection();
        Connection.Execute("AreasSaveOrder", new { companyId = companyId, OrderData = orderData }, commandType: CommandType.StoredProcedure);
      }
      finally
      {
        CloseConnection();
      }

    }
  }
}
