using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using Langben.DAL;
using System.ServiceModel;

namespace Langben.IBLL
{
    public partial interface IAllotBLL
    {
     
        List<Allot> GetByParam(string id, int page, int rows, string order, string sort, string search, ref int total, int uid);
    }
}

