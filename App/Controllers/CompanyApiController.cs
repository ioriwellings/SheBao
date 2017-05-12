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
    public class CompanyApiController : BaseApiController
    {
        SysEntities SysEntitiesO2O = new SysEntities();
        #region 增加企业信息(单个企业)
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/company")]
        public IHttpActionResult AddCompany(CompanyInfo companyinfo)
        {
            if (!IsValidation())
            {
                return Json(new { code = -1, message = "验证未通过" });
            }
            try
            {
                using (SysEntities db = new SysEntities())
                {
                    CRM_Company company = new CRM_Company();

                    company.CompanyCode = "0001测试";
                    company.CompanyName = companyinfo.Name;
                    company.OrganizationCode = companyinfo.OrganizationCode;
                    //company.TaxRegistryNumber = companyinfo.TaxNumber;
                    //company.InvoiceCompanyName = companyinfo.InvoiceName;
                    //company.PayCompanyName =   companyinfo.PaymentName;
                    company.CreateTime = DateTime.Now;
                    company.CreateUserID = 1;
                    company.CreateUserName = "尹岩贺";
                    company.OperateStatus = 1;//启用


                    //注意   是否需要转换
                    company.Dict_HY_Code = companyinfo.Industry;
                    company.Source = 1;//平台推送。


                    db.CRM_Company.Add(company);
                    db.SaveChanges();
                    //#region 开票回款

                    //CRM_CompanyFinance companyfinance = new CRM_CompanyFinance();
                    //companyfinance.CRM_Company_ID = company.ID;
                    //companyfinance.FinanceName = companyinfo.InvoiceName;
                    //companyfinance.FinanceType = 1;//开票
                    //companyfinance.Status = 1;//启用
                    //companyfinance.BranchID = 1;
                    //companyfinance.CreateTime = DateTime.Now;
                    //companyfinance.CreateUserID = 1;
                    //companyfinance.CreateUserName = "尹岩贺";

                    //db.CRM_CompanyFinance.Add(companyfinance);
                    //db.SaveChanges();

                    //companyfinance = new CRM_CompanyFinance();
                    //companyfinance.CRM_Company_ID = company.ID;
                    //companyfinance.FinanceName = companyinfo.PaymentName;
                    //companyfinance.FinanceType = 2;//回款
                    //companyfinance.Status = 1;//启用
                    //companyfinance.BranchID = 1;
                    //companyfinance.CreateTime = DateTime.Now;
                    //companyfinance.CreateUserID = 1;
                    //companyfinance.CreateUserName = "尹岩贺";

                    //db.CRM_CompanyFinance.Add(companyfinance);
                    //db.SaveChanges();
                    //#endregion

                    #region 联系人
                    CRM_CompanyLinkMan companylinkman = new CRM_CompanyLinkMan();
                    companylinkman.CRM_Company_ID = company.ID;
                    companylinkman.LinkManName = companyinfo.ContactPerson;
                    companylinkman.Telephone = companyinfo.ContactPhone;
                    companylinkman.Address = companyinfo.ContactAddress;

                    companylinkman.CreateTime = DateTime.Now;
                    companylinkman.BranchID = 1;//暂时写1
                    companylinkman.CreateUserID = 1;
                    companylinkman.CreateUserName = "尹岩贺";
                    companylinkman.Status = 1;

                    db.CRM_CompanyLinkMan.Add(companylinkman);
                    db.SaveChanges();

                    #endregion


                    return Json(new { code = 0, message = "成功", id = company.ID });
                };
            }
            catch (Exception ee)
            {

                return Json(new { code = -1, message = ee.ToString() });
            }
        }

        public class CompanyInfo
        {
            /// <summary>
            /// 企业名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 组织机构代码
            /// </summary>
            public string OrganizationCode { get; set; }
            /// <summary>
            /// 税务登记证号
            /// </summary>
            public string TaxNumber { get; set; }

            /// <summary>
            /// 开票名称
            /// </summary>
            public string InvoiceName { get; set; }

            /// <summary>
            /// 付款方名称
            /// </summary>
            public string PaymentName { get; set; }

            /// <summary>
            /// 所属行业
            /// </summary>
            public string Industry { get; set; }

            /// <summary>
            /// 销售所属地
            /// </summary>
            public string SaleLocation { get; set; }
            /// <summary>
            /// 联系人
            /// </summary>
            public string ContactPerson { get; set; }

            /// <summary>
            /// 联系电话
            /// </summary>
            public string ContactPhone { get; set; }


            /// <summary>
            /// 联系地址
            /// </summary>
            public string ContactAddress { get; set; }

            /// <summary>
            /// 企业资料附件
            /// </summary>
            public string CompanyAttachments { get; set; }

            /// <summary>
            /// 是否单立户
            /// </summary>
            public string IsBigAccount { get; set; }

        }


        #endregion


        #region 获得该企业本月缴纳社保人数
        public int getEmployee(int? CRM_Company_ID, int? YearMonuth)
        {
            string State = Common.Status.启用.ToString();
            var trueData = from a in SysEntitiesO2O.EmployeeMiddle.Where(em => em.StartDate <= YearMonuth && em.EndedDate >= YearMonuth && em.State == State )
                           join b in SysEntitiesO2O.CompanyEmployeeRelation.Where(x => x.CompanyId == CRM_Company_ID) on a.CompanyEmployeeRelationId equals b.Id
                           group new { b } by new
                           {
                               EmployeeID = b.EmployeeId
                           }
                               into s

                               select new
                               {
                                   Count = s.Key.EmployeeID                      
                               };
            if (trueData.FirstOrDefault() != null)
            {
                return trueData.Count();
            }
            else
            {
                return 0;
            }


        }
        #endregion

    }
}
