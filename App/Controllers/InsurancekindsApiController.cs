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
    public class InsurancekindsApiController : BaseApiController
    {

        #region 获取社保种类
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">城市ID</param>
        /// <returns></returns>
        /// 
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/insurancekinds/{id}")]
        public IHttpActionResult Get(string id)
        {
            if (!IsValidation())
            {
                return Json(new { code = -1, message = "验证未通过" });
            }
            try
            {
                using (SysEntities db = new SysEntities())
                {
                    var res = from a in db.InsuranceKind.Where(x => x.City == id)
                              select new insurancekinds
                              {
                                  Id = a.Id,
                                  Name = a.Name,
                                  Attachment = (from b in db.Attachment
                                                where b.InsuranceKindId == a.Id
                                                select new Attachment
                                                {
                                                    Name = b.Name,
                                                    Path = b.Path
                                                }).ToList()
                              };


                    return Json(new { code = 0, message = res.ToList() });

                }
            }
            catch (Exception ee)
            {

                return Json(new { code = -1, message = ee.ToString() });
            }
        }



        public class insurancekinds
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public List<Attachment> Attachment { get; set; }
        }

        public class Attachment
        {

            public string Name { get; set; }

            public string Path { get; set; }
        }

        #endregion

    }
}
