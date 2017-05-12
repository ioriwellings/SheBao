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
    public class RemoveEmployeeApiController : BaseApiController
    {
        //  报减信息
        #region


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/removeemployee/{id}")]

        public IHttpActionResult Index(int id, List<PostInfo> postinfoList)// id为公司ID
        {
            if (!IsValidation())
            {
                return Json(new { code = 1, message = "验证未通过" });
            }

            try
            {
                using (SysEntities db = new SysEntities())
                {

                    var res = InsertStopPaymentInfo(db, postinfoList, id);

                    return Json(new { code = 0, message = res });
                }
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



        public class PostInfo
        {
            public int EmployeeId { get; set; }
            public string Name { get; set; }
            public string IDType { get; set; }
            public string IDNumber { get; set; }

            public List<InsuranceType> InsuranceType { get; set; }
        }

        public class InsuranceType
        {
            //报减手续
            public int PoliceOperationId { get; set; }

            //报增时政策
            public int PoliceInsuranceId { get; set; }
            public string Insurance { get; set; }

            public DateTime ReductionTime { get; set; }
            public string ReductionMode { get; set; }
            public string InsuranceNumber { get; set; }

        }


        public string InsertStopPaymentInfo(SysEntities db, List<PostInfo> lstStopPaymentEmployeeInfo, int CompanyId)
        {
            string result = string.Empty;
            StringBuilder error = new StringBuilder();
            List<EmployeeStopPayment> lstStopPayment = new List<EmployeeStopPayment>();
            DateTime now = DateTime.Now;
            List<string> lstStopingKind = new List<string> { "待责任客服确认", "待员工客服经理分配", "员工客服确认", "社保专员已提取" };

            #region 验证

            try
            {
                foreach (PostInfo emp in lstStopPaymentEmployeeInfo)
                {
                    StringBuilder err = new StringBuilder();

                    var e = db.Employee.FirstOrDefault(o => o.CertificateNumber == emp.IDNumber);
                    if (e == null)
                    {
                        err.AppendLine(string.Format("不存在员工 {0} - {1}，不能报减。", emp.Name, emp.IDNumber));
                        continue;
                    }
                    emp.EmployeeId = e.Id;

                    var cer =
                        db.CompanyEmployeeRelation.Where(
                            o => o.CompanyId == CompanyId && o.EmployeeId == emp.EmployeeId && o.State == "在职")
                            .OrderByDescending(o => o.Id)
                            .FirstOrDefault();
                    if (cer == null)
                    {
                        err.AppendLine(string.Format("{0} 没有任何申报成功的社保或公积金，不能报减。", emp.Name));
                        continue;
                    }

                    var empAdd = db.EmployeeAdd.Where(o => o.CompanyEmployeeRelationId == cer.Id && o.State == "申报成功");
                    if (!empAdd.Any())
                    {
                        err.AppendLine(string.Format("{0}  没有任何申报成功的社保或公积金，不能报减。", emp.Name));
                        continue;
                    }
                    foreach (InsuranceType k in emp.InsuranceType)
                    {
                        var add = empAdd.FirstOrDefault(o => o.PoliceInsuranceId == k.PoliceInsuranceId);
                        if (add == null)
                        {
                            err.AppendLine(string.Format(" {1} 没有申报成功 {2}，不能报减。", "", emp.Name,
                                k.Insurance));
                        }
                        else
                        {
                            var stopingKind = add.EmployeeGoonPayment.Where(o => lstStopingKind.Contains(o.State));
                            if (stopingKind.Any())
                            {
                                err.AppendLine(string.Format("{0} 的员工 {1}  {2} 正在报减，不能重复报减。", "",
                                    emp.Name, k.Insurance));
                            }
                            else
                            {

                                EmployeeStopPayment stopPayment = new EmployeeStopPayment();
                                stopPayment.EmployeeAddId = add.Id;
                                stopPayment.InsuranceMonth = k.ReductionTime;
                                stopPayment.PoliceOperationId = k.PoliceOperationId;
                                stopPayment.Remark = "";
                                stopPayment.State = "待责任客服确认";
                                stopPayment.CreateTime = now;
                                stopPayment.CreatePerson = "1";
                                stopPayment.UpdateTime = now;
                                stopPayment.UpdatePerson = "1";
                                stopPayment.YearMonth = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));

                                lstStopPayment.Add(stopPayment);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(err.ToString()))
                    {
                        error.AppendLine(err.ToString());
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            #endregion
            result = error.ToString();
            try
            {
                if (lstStopPayment.Any() && string.IsNullOrEmpty(result))
                {
                    foreach (EmployeeStopPayment stopPayment in lstStopPayment)
                    {
                        db.EmployeeStopPayment.Add(stopPayment);
                        db.SaveChanges();
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }



        #endregion

    }
}
