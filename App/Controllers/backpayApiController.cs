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
    public class BackPayApiController : BaseApiController
    {
        //  补缴信息
        #region

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/backpay/{id}")]
        public IHttpActionResult Index(int CompanyId, List<PostInfo> postinfoList)
        {
            if (!IsValidation())
            {
                return Json(new { code = -1, message = "验证未通过" });
            }

            try
            {
                using (SysEntities db = new SysEntities())
                {


                    return Json(new { code = 0, message = "成功" });
                }
            }
            catch (Exception ee)
            {

                return Json(new { code = -1, message = ee.ToString() });
            }

        }


//[
//        {
//            "Name":"刘腾飞",
//            "IDType":"身份证",
//            "IDNumber":"123456789012345678",
//            "InsuranceType":[
//                {
//                    "Insurance":"Pension",
//                    "ReductionTime":"2015-7",
//                    "ReductionMode":"中断",
//                    "InsuranceNumber":"55121365"
//                },
//                {
//                    "Insurance":"medical",
//                    "ReductionTime":"2015-7",
//                    "ReductionMode":"中断",
//                    "InsuranceNumber":"55121365"
//                },
//                {
//                    "Insurance":"WorkInjury",
//                    "ReductionTime":"2015-7",
//                    "ReductionMode":"中断",
//                    "InsuranceNumber":"55121365"
//                },
//                {
//                    "Insurance":"Unemployment ",
//                    "ReductionTime":"2015-7",
//                    "ReductionMode":"中断",
//                    "InsuranceNumber":"55121365"
//                },
//                {
//                    "Insurance":"Maternity",
//                    "ReductionTime":"2015-7",
//                    "ReductionMode":"中断",
//                    "InsuranceNumber":"55121365"
//                },
//                {
//                    "Insurance":"HousingFund",
//                    "ReductionTime":"2015-7",
//                    "ReductionMode":"中断",
//                    "InsuranceNumber":"55121365"
//                }
//            ]
//        }
//    ]



        public class PostInfo
        {

            public string Name { get; set; }
            public string IDType { get; set; }
            public string IDNumber { get; set; }


            public List<InsuranceType> InsuranceType { get; set; }
        }

        public class InsuranceType
        {
            public string Insurance { get; set; }
         
            public DateTime ReductionTime { get; set; }
            public string ReductionMode { get; set; }
            public string InsuranceNumber { get; set; }
        }

        #endregion

    }
}
