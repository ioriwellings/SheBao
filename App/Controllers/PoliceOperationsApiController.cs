using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Web.Mvc;
using System.Text;
using System.EnterpriseServices;
using System.Configuration;
using Models;
using Common;
using Langben.DAL;
using Langben.BLL;
using Langben.App.Models;

namespace Langben.App.Controllers
{
    public class PoliceOperationsApiController : BaseApiController
    {

        #region 获取社保手续
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">id为社保种类的主键（养老ID）</param>
        /// <returns></returns>
        /// 
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/policeOperations/{id}")]
        public IHttpActionResult Get(int ID)
        {
            if (!IsValidation())
            {
                return Json(new { code = -1, message = "验证未通过" });
            }
            try
            {
                using (SysEntities db = new SysEntities())
                {
                    var res = from b in db.InsuranceKind.Where(a => a.Id == ID)
                              from c in b.PoliceOperation
                              select new PoliceOperations
                              {
                                  Id = c.Id,
                                  Name = c.Name
                              };


                    return Json(new { code = 0, message = res.ToList() });

                }
            }
            catch (Exception ee)
            {

                return Json(new { code = -1, message = ee.ToString() });
            }

        }



        public class PoliceOperations
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        #endregion

    }
}
