using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Langben.DAL;
using Models;

namespace Langben.App.Controllers
{
    public class COST_PayCreateApiController : BaseApiController
    {
        public Common.ClientResult.DataResult GetSuppliers()
        {
            using (SysEntities db = new SysEntities())
            {
                var state = ((int)Common.Status.启用).ToString();
                var query = db.Supplier.Where(a => a.Status == state).ToList();
                 
                var data = new Common.ClientResult.DataResult
                {
                    rows = query.Select(s => new
                    {
                        ID = s.Id,
                        Name = s.Name
                    })
                };
               
                return data;
            }
        }
    }
}
