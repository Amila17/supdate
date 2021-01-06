using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Supdate.Data.Base;
using Supdate.Model;
using Supdate.Model.Exceptions;
using Supdate.Util;

namespace Supdate.Data
{
  public class SubscriptionRepository : CrudRepository<Subscription>, ISubscriptionRepository
  {

  }
}
