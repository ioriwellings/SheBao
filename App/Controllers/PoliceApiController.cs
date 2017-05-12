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
    public class PoliceApiController : BaseApiController
    {

        #region 获取社保政策
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">城市ID</param>
        /// <returns></returns>
        /// 

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/policy")]
        public IHttpActionResult Get(int policeoperationid = 0, int categoriesId = 0)
        {
            if (!IsValidation())
            {
                return Json(new { code = -1, message = "验证未通过" });
            }
            try
            {
                using (SysEntities db = new SysEntities())
                {
                    ResultJson result = new ResultJson();
                    result.Id = categoriesId;
                    result.Category = db.PoliceAccountNature.Where(a => a.Id == categoriesId).FirstOrDefault().Name;
                    result.Pension = (from a in db.PoliceInsurance
                                      where a.PoliceOperationPoliceInsurancePoliceAccountNature.Where(b => b.PoliceOperationId == policeoperationid && b.PoliceAccountNatureId == categoriesId).FirstOrDefault()!=null
                                      where a.Name.Contains("养老")
                                      select new PoliceInsurance
                                      {
                                          Company_max_base = a.CompanyHighestNumber,
                                          Company_min_base = a.CompanyLowestNumber,
                                          Person_max_base = a.EmployeeHighestNumber,
                                          Person_min_base = a.EmployeeLowestNumber,
                                          Company_percentage = a.CompanyPercent,
                                          Person_percentage = a.EmployeePercent,
                                          Add_month = a.MaxPayMonth,
                                          Deadline = a.EndTime,


                                      }).FirstOrDefault();
                    result.Medical = (from a in db.PoliceInsurance
                                      where a.PoliceOperationPoliceInsurancePoliceAccountNature.Where(b => b.PoliceOperationId == policeoperationid && b.PoliceAccountNatureId == categoriesId).FirstOrDefault() != null
                                      where a.Name.Contains("医疗")
                                      select new PoliceInsurance
                                      {
                                          Company_max_base = a.CompanyHighestNumber,
                                          Company_min_base = a.CompanyLowestNumber,
                                          Person_max_base = a.EmployeeHighestNumber,
                                          Person_min_base = a.EmployeeLowestNumber,
                                          Company_percentage = a.CompanyPercent,
                                          Person_percentage = a.EmployeePercent,
                                          Add_month = a.MaxPayMonth,
                                          Deadline = a.EndTime,


                                      }).FirstOrDefault();
                    result.WorkInjury = (from a in db.PoliceInsurance
                                         where a.PoliceOperationPoliceInsurancePoliceAccountNature.Where(b => b.PoliceOperationId == policeoperationid && b.PoliceAccountNatureId == categoriesId).FirstOrDefault() != null
                                         where a.Name.Contains("工伤")
                                         select new PoliceInsurance
                                         {
                                             Company_max_base = a.CompanyHighestNumber,
                                             Company_min_base = a.CompanyLowestNumber,
                                             Person_max_base = a.EmployeeHighestNumber,
                                             Person_min_base = a.EmployeeLowestNumber,
                                             Company_percentage = a.CompanyPercent,
                                             Person_percentage = a.EmployeePercent,
                                             Add_month = a.MaxPayMonth,
                                             Deadline = a.EndTime,


                                         }).FirstOrDefault();
                    result.Unemployment = (from a in db.PoliceInsurance
                                           where a.PoliceOperationPoliceInsurancePoliceAccountNature.Where(b => b.PoliceOperationId == policeoperationid && b.PoliceAccountNatureId == categoriesId).FirstOrDefault() != null
                                           where a.Name.Contains("失业")
                                           select new PoliceInsurance
                                           {
                                               Company_max_base = a.CompanyHighestNumber,
                                               Company_min_base = a.CompanyLowestNumber,
                                               Person_max_base = a.EmployeeHighestNumber,
                                               Person_min_base = a.EmployeeLowestNumber,
                                               Company_percentage = a.CompanyPercent,
                                               Person_percentage = a.EmployeePercent,
                                               Add_month = a.MaxPayMonth,
                                               Deadline = a.EndTime,


                                           }).FirstOrDefault();
                    result.HousingFund = (from a in db.PoliceInsurance
                                          where a.PoliceOperationPoliceInsurancePoliceAccountNature.Where(b => b.PoliceOperationId == policeoperationid && b.PoliceAccountNatureId == categoriesId).FirstOrDefault() != null
                                          where a.Name.Contains( "公积金")
                                          select new PoliceInsurance
                                          {
                                              Company_max_base = a.CompanyHighestNumber,
                                              Company_min_base = a.CompanyLowestNumber,
                                              Person_max_base = a.EmployeeHighestNumber,
                                              Person_min_base = a.EmployeeLowestNumber,
                                              Company_percentage = a.CompanyPercent,
                                              Person_percentage = a.EmployeePercent,
                                              Add_month = a.MaxPayMonth,
                                              Deadline = a.EndTime,


                                          }).FirstOrDefault();

                
                    return Json(new { code = 0, message = result });
                }
            }
            catch (Exception ee)
            {
                return Json(new { code = -1, message = ee.ToString() });
            }

        }

        public class ResultJson
        {
            public int Id { get; set; }
            public string Category { get; set; }

            public PoliceInsurance Pension { get; set; }
            public PoliceInsurance Medical { get; set; }
            public PoliceInsurance WorkInjury { get; set; }
            public PoliceInsurance Unemployment { get; set; }
            public PoliceInsurance HousingFund { get; set; }
        }

        public class PoliceInsurance
        {

            public decimal? Company_max_base { get; set; }
            public decimal? Company_min_base { get; set; }
            public decimal? Person_max_base { get; set; }
            public decimal? Person_min_base { get; set; }
            public decimal? Company_percentage { get; set; }
            public decimal? Person_percentage { get; set; }

            public int? Add_month { get; set; }
            public DateTime? Deadline { get; set; }

        }




        // {
        //    "Id":"9988",
        //    "Category":"城镇",
        //    "Pension":{
        //        "Company_max_base":"1000",
        //        "Company_min_base":"2000",
        //        "Person_max_base":"1000",
        //        "Person_min_base":"2000",
        //        "Company_percentage":"20",
        //        "Person_percentage":"10",
        //        "Add_month":"3",
        //        "Deadline":"25",
        //         "Attachment":[{"Name":"身份证","Path":"/a.jpg"},{"Name":"身份证","Path":"/a.jpg"}]
        //    },
        //    "Medical":{
        //        "Company_max_base":"1000",
        //        "Company_min_base":"2000",
        //        "Person_max_base":"1000",
        //        "Person_min_base":"2000",
        //        "Company_percentage":"20",
        //        "Person_percentage":"10",
        //        "Add_month":"3",
        //        "Deadline":"25",
        //         "Attachment":[{"Name":"身份证","Path":"/a.jpg"},{"Name":"身份证","Path":"/a.jpg"}]

        //    },
        //    "WorkInjury":{
        //        "Company_max_base":"1000",
        //        "Company_min_base":"2000",
        //        "Person_max_base":"1000",
        //        "Person_min_base":"2000",
        //        "Company_percentage":"20",
        //        "Person_percentage":"10",
        //        "Add_month":"3",
        //        "Deadline":"25",
        //         "Attachment":[{"Name":"身份证","Path":"/a.jpg"},{"Name":"身份证","Path":"/a.jpg"}]
        //    },
        //    "Unemployment":{
        //        "Company_max_base":"1000",
        //        "Company_min_base":"2000",
        //        "Person_max_base":"1000",
        //        "Person_min_base":"2000",
        //        "Company_percentage":"20",
        //        "Person_percentage":"10",
        //        "Add_month":"3",
        //        "Deadline":"25",
        //         "Attachment":[{"Name":"身份证","Path":"/a.jpg"},{"Name":"身份证","Path":"/a.jpg"}]
        //    }
        //}




        #endregion

    }
}
