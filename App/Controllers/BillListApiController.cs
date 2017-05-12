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
using Langben.DAL.Model;

namespace Langben.App.Controllers
{
    public class BillListApiController : BaseApiController
    {

        #region 七、	客户端查询业务端账单明细信息

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/billlist/{id}")]
        public IHttpActionResult GetCompany(int id = 0, int billid = 0) //id为公司ID billid 为费用单ID
        {
            if (!IsValidation())
            {
                return Json(new { code = -1, message = "验证未通过" });
            }

            try
            {
                using (SysEntities db = new SysEntities())
                {

                    if (billid == 0)
                    {
                        string res = "{\"Id\":null,\"Remark\":null,\"BillDate\":null,\"Employees\":[{\"IDNumber\":\"123456789012345678\",\"Name\":\"刘腾飞\",\"Style\":\"正常\",\"Remark\":[{\"Name\":\"有问题说明的原因\"}],\"PayType\":\"补缴\",\"PensionCity\":\"杭州\",\"PensionSection\":\"2015-7~2015-8\",\"PensionMonth\":2,\"PensionCompanyWage\":\"5565\",\"PensionPersonWage\":\"3565\",\"PensionCompanyPercentage\":25,\"PensionPersonPercentage\":3,\"MedicalCity\":\"杭州\",\"MedicalSection\":\"2015-7~2015-8\",\"MedicalMonth\":2,\"MedicalCompanyWage\":\"5565\",\"MedicalPersonWage\":\"3565\",\"MedicalCompanyPercentage\":25,\"MedicalPersonPercentage\":3,\"WorkInjuryCity\":\"杭州\",\"WorkInjurySection\":\"2015-7~2015-8\",\"WorkInjuryMonth\":2,\"WorkInjuryCompanyWage\":\"5565\",\"WorkInjuryPersonWage\":\"3565\",\"WorkInjuryCompanyPercentage\":25,\"WorkInjuryPersonPercentage\":3,\"UnemploymentCity\":\"杭州\",\"UnemploymentSection\":\"2015-7~2015-8\",\"UnemploymentMonth\":2,\"UnemploymentCompanyWage\":\"5565\",\"UnemploymentPersonWage\":\"3565\",\"UnemploymentCompanyPercentage\":25,\"UnemploymentPersonPercentage\":3,\"MaternityCity\":\"杭州\",\"MaternitySection\":\"2015-7~2015-8\",\"MaternityMonth\":2,\"MaternityCompanyWage\":\"5565\",\"MaternityPersonWage\":\"3565\",\"MaternityCompanyPercentage\":25,\"MaternityPersonPercentage\":3,\"HousingFundCity\":\"杭州\",\"HousingFundSection\":\"2015-7~2015-8\",\"HousingFundMonth\":2,\"HousingFundCompanyWage\":\"5565\",\"HousingFundPersonWage\":\"3565\",\"HousingFundCompanyPercentage\":25,\"HousingFundPersonPercentage\":3,\"LateFees\":\"3565\",\"DisabledInsuranceFee\":\"3565\",\"HeatingFees\":\"3565\",\"MaterialFees\":\"3565\",\"ServiceFees\":\"3565\"}]}";
                        return Json(new { code = 0, message = res });
                    }
                    else
                    {
                   
                        IBLL.ICOST_CostTableBLL m_BLL = new BLL.COST_CostTableBLL();
                        int total = 0;
                        List<CostFeeModel> queryData2 = m_BLL.GetCostFeeList(null, 1, int.MaxValue, null, null, 0, 0, ref total);


                        var res = (from a in queryData2
                                   where a.ID == billid
                                   select new
                                   {
                                       Id = a.ID,
                                       Remark = a.Status,
                                       BillDate = a.YearMonth,
                                       Employees = (from s in m_BLL.GetCostFeeDetailList(a.ID)
                                                    select new
                                            {

                                                IDNumber = s.CertificateNumber,
                                                Name = s.EmployName,
                                                Style = "",
                                                Remark2 = "",
                                                PayType = ((Common.EmployeeMiddle_PaymentStyle)s.PaymentStyle).ToString(),
                                                PensionCity = s.CityName,
                                                PensionSection = s.YanglaoPaymentInterval,
                                                PensionMonth = s.YanglaoPaymentMonth,
                                                PensionCompanyWage = s.YanglaoCompanyCost,
                                                PensionPersonWage = s.YanglaoPersonCost,
                                                PensionCompanyPercentage = s.YanglaoCompanyRadix,
                                                PensionPersonPercentage = s.YanglaoPersonRatio,


                                                MedicalCity = s.CityName,
                                                MedicalSection = s.YiliaoPaymentInterval,
                                                MedicalMonth = s.YiliaoPaymentMonth,
                                                MedicalCompanyWage = s.YiliaoCompanyCost,
                                                MedicalPersonWage = s.YiliaoPersonCost,
                                                MedicalCompanyPercentage = s.YiliaoCompanyRadix,
                                                MedicalPersonPercentage = s.YiliaoPersonRatio,



                                                WorkInjuryCity = s.CityName,
                                                WorkInjurySection = s.GongshangPaymentInterval,
                                                WorkInjuryMonth = s.GongshangPaymentMonth,
                                                WorkInjuryCompanyWage = s.GongshangCompanyCost,
                                                WorkInjuryPersonWage = s.GongshangPersonCost,
                                                WorkInjuryCompanyPercentage = s.GongshangCompanyRadix,
                                                WorkInjuryPersonPercentage = s.GongshangPersonRatio,



                                                UnemploymentCity = s.CityName,
                                                UnemploymentSection = s.ShiyePaymentInterval,
                                                UnemploymentMonth = s.ShiyePaymentMonth,
                                                UnemploymentCompanyWage = s.ShiyeCompanyCost,
                                                UnemploymentPersonWage = s.ShiyePersonCost,
                                                UnemploymentCompanyPercentage = s.ShiyeCompanyRadix,
                                                UnemploymentPersonPercentage = s.ShiyePersonRatio,



                                                MaternityCity = s.CityName,
                                                MaternitySection = s.ShengyuPaymentInterval,
                                                MaternityMonth = s.ShengyuPaymentMonth,
                                                MaternityCompanyWage = s.ShengyuCompanyCost,
                                                MaternityPersonWage = s.ShengyuPersonCost,
                                                MaternityCompanyPercentage = s.ShengyuCompanyRadix,
                                                MaternityPersonPercentage = s.ShengyuPersonRatio,


                                                HousingFundCity = s.CityName,
                                                HousingFundSection = s.GongjijinPaymentInterval,
                                                HousingFundMonth = s.GongjijinPaymentMonth,
                                                HousingFundCompanyWage = s.GongjijinCompanyCost,
                                                HousingFundPersonWage = s.GongjijinPersonCost,
                                                HousingFundCompanyPercentage = s.GongjijinCompanyRadix,
                                                HousingFundPersonPercentage = s.GongjijinPersonRatio,


                                                OtherCost = s.OtherCost,
                                                OtherInsuranceCost = s.OtherInsuranceCost,
                                                MaterialFees = s.ProductionCost,
                                                ServiceFees = s.ServiceCost,
                                            }).ToList()
                                   }).ToList();

                        return Json(new { code = 0, message = res });
                    }

                };
            }
            catch (Exception ee)
            {
                return Json(new { code = -1, message = ee.ToString() });

            }
        }



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


        //        {
        //  "Id": "9988",
        //  "Remark": "确认",
        //  "BillDate": "201507",
        //  "Employees": [{
        //"IDNumber": "123456789012345678",
        //"Name": "刘腾飞",
        //"Style": "正常",
        //"Remark":[{"Name":"有问题说明的原因"}],
        //    "PayType": "补缴",
        //    "PensionCity": "杭州",
        //    "PensionSection": "2015-7~2015-8",
        //    "PensionMonth": 2,
        //    "PensionCompanyWage": "5565",
        //    "PensionPersonWage": "3565",
        //    "PensionCompanyPercentage": 25,
        //    "PensionPersonPercentage": 3,
        //    "MedicalCity": "杭州",
        //    "MedicalSection": "2015-7~2015-8",
        //    "MedicalMonth": 2,
        //    "MedicalCompanyWage": "5565",
        //    "MedicalPersonWage": "3565",
        //    "MedicalCompanyPercentage": 25,
        //    "MedicalPersonPercentage": 3,
        //    "WorkInjuryCity": "杭州",
        //    "WorkInjurySection": "2015-7~2015-8",
        //    "WorkInjuryMonth": 2,
        //    "WorkInjuryCompanyWage": "5565",
        //    "WorkInjuryPersonWage": "3565",
        //    "WorkInjuryCompanyPercentage": 25,
        //    "WorkInjuryPersonPercentage": 3,
        //    "UnemploymentCity": "杭州",
        //    "UnemploymentSection": "2015-7~2015-8",
        //    "UnemploymentMonth": 2,
        //    "UnemploymentCompanyWage": "5565",
        //    "UnemploymentPersonWage": "3565",
        //    "UnemploymentCompanyPercentage": 25,
        //    "UnemploymentPersonPercentage": 3,
        //    "MaternityCity": "杭州",
        //    "MaternitySection": "2015-7~2015-8",
        //    "MaternityMonth": 2,
        //    "MaternityCompanyWage": "5565",
        //    "MaternityPersonWage": "3565",
        //    "MaternityCompanyPercentage": 25,
        //    "MaternityPersonPercentage": 3,
        //    "HousingFundCity": "杭州",
        //    "HousingFundSection": "2015-7~2015-8",
        //    "HousingFundMonth": 2,
        //    "HousingFundCompanyWage": "5565",
        //    "HousingFundPersonWage": "3565",
        //    "HousingFundCompanyPercentage": 25,
        //    "HousingFundPersonPercentage": 3,
        //    "LateFees": "3565",
        //    "DisabledInsuranceFee": "3565",
        //    "HeatingFees": "3565",
        //    "MaterialFees": "3565",
        //    "ServiceFees": "3565"}
        //  ]
        //}

        #endregion

    }
}
