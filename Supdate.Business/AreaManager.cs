using System;
using System.Collections.Generic;
using Supdate.Data;
using Supdate.Model;

namespace Supdate.Business
{
  public class AreaManager : Manager<Area>, IAreaManager
  {
    private readonly IAreaRepository _areaRepository;

    public AreaManager(IAreaRepository areaRepository)
      : base(areaRepository)
    {
      _areaRepository = areaRepository;
    }

    public Area GetArea(int companyId, Guid uniqueId)
    {
      return _areaRepository.GetArea(companyId, uniqueId);
    }
    public void DeleteArea(int companyId, Guid areaUniqueId)
    {
       _areaRepository.DeleteArea(companyId, areaUniqueId);
    }
    public void SaveDisplayOrder(int companyId, IList<EntityDisplayOrder> displayOrders)
    {
      _areaRepository.SaveDisplayOrder(companyId, displayOrders);
    }


  }
}
