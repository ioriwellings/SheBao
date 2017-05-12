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
using System.IO;

namespace Langben.App.Controllers
{
    public class CreateBillApiController : BaseApiController
    {
        //  六、	确定生成账单
        #region

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/createbill/{id}")]

        public IHttpActionResult CreateCostTable(int Id,int yearMonth=0)//公司ID
        {

            if (!IsValidation())
            {
                return Json(new { code = -1, message = "验证未通过" });
            }

            if (yearMonth == 0)
            {
                yearMonth=Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
            }

            try
            {
                string Url = "http://110.249.162.18:8180/api/COST_CostTableApi/PostCreate";
              //  string Url = "http://localhost:55977/api/COST_CostTableApi/PostCreate";
                string PostDate = "CRM_Company_ID=" + Id + "&yearMonth=" + yearMonth;

                string res = HttpPost(Url, PostDate);
                return Json(new { code = 0, message = res });
            }
            catch
            {
                throw new Exception();
            }



            return Json(new { errcode = 0, });
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

        private string HttpPost(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
            Stream myRequestStream = request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
            myStreamWriter.Write(postDataStr);
            myStreamWriter.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }

        #endregion

    }
}
