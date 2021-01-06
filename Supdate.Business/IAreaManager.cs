using System;
using System.Collections.Generic;
using Supdate.Model;

namespace Supdate.Business
{
  public interface IAreaManager : IManager<Area>
  {
    Area GetArea(int companyId, Guid uniqueId);
    void DeleteArea(int companyId, Guid areaUniqueId);
    void SaveDisplayOrder(int companyId, IList<EntityDisplayOrder> displayOrders);
  }
}
