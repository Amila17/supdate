using System;
using System.Collections.Generic;
using Supdate.Data.Base;
using Supdate.Model;

namespace Supdate.Data
{
  public interface IAreaRepository : ICrudRepository<Area>
  {
    Area GetArea(int companyId, Guid uniqueId);
    void DeleteArea(int companyId, Guid uniqueId);
    void SaveDisplayOrder(int companyId, IList<EntityDisplayOrder> displayOrders);
  }
}
