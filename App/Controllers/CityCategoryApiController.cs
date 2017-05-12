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
using System.Web;

namespace Langben.App.Controllers
{
    public class CityCategoryApiController : BaseApiController
    {

        #region 获取不同缴纳地的户口性质
        // 获取不同缴纳地的户口性质
        //http请求方式: GET

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/citycategory")]
        public IHttpActionResult GetCityCtegory()
        {
            if (!IsValidation())
            {
                return Json(new { code = -1, message = "验证未通过" });
            }

            List<CityCategory> cityCategoryList = new List<CityCategory>();
            try
            {
                using (SysEntities db = new SysEntities())
                {
                    var aa = from employee in db.Employee
                             from bank in db.EmployeeBank
                             from contact in employee.EmployeeContact


                             select new
                             {
                                 Empname = employee.Name,
                                 AccountName = bank.AccountName
                             };
                    var res = from a in db.City
                              select new CityCategory
                              {
                                  Id = a.Id,
                                  Name = a.Name,
                                  Categories = (from c in a.PoliceAccountNature
                                                select new Categories
                                                {

                                                    Id = c.Id,
                                                    Name = c.Name
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

        public class CityCategory
        {
            public string Id { get; set; }
            public string Name { get; set; }

            public List<Categories> Categories { get; set; }
        }

        public class Categories
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        #endregion

    }
}
