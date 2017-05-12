using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using System.EnterpriseServices;
using System.Configuration;
using Models;
using Common;
using Langben.DAL;
using Langben.BLL;
using System.Web.Http;
using Langben.App.Models;
using Langben.DAL.Model;

namespace Langben.App.Areas.CRM.Controllers
{
    /// <summary>
    /// 客户_企业信息
    /// </summary>
    public class CRM_CompanyApiController : BaseApiController
    {
        /// <summary>
        /// 根据ID获取企业基本信息
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public string Get(int id)
        {
            string item = m_BLL.GetCompanyBase(id);
            return item;
        }
        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            int total = 0;
            int intBranchID = LoginInfo.BranchID;
            string strCompanyName = "";
            string strUserID_XS = "";
            int? intUserID_XS = null;
            string menuID = "10";
            #region 权限
            string departments = "";
            int departmentScope = base.MenuDepartmentScopeAuthority(menuID);
            if (departmentScope == (int)DepartmentScopeAuthority.无限制)//无限制
            {
                //部门业务权限
                departments = MenuDepartmentAuthority(menuID);
            }
            #endregion

            if (!string.IsNullOrEmpty(getParam.search))
            {
                string[] search = getParam.search.Split('^');
                strCompanyName = search[0];
                strUserID_XS = search[1];
            }
            if (!string.IsNullOrEmpty(strUserID_XS))
            {
                intUserID_XS = Convert.ToInt32(strUserID_XS);
            }

            List<CRM_CompanyView> queryData = m_BLL.GetCompanyListForSales(getParam.id, getParam.page, getParam.rows, strCompanyName, intUserID_XS, intBranchID, menuID, departmentScope, LoginInfo.UserID, LoginInfo.DepartmentID, departments, ref total);

            // List<CRM_Company> queryData = m_BLL.GetByParam(getParam.id, getParam.page, getParam.rows, getParam.order, getParam.sort, getParam.search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }

        //创建新公司
        public Common.ClientResult.Result PostNewCompany([FromBody]CRM_CompanyInfo entity)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {
                CRM_Company baseModel = entity.BasicInfo;//基本信息    
                baseModel.CreateTime = DateTime.Now;
                baseModel.CreateUserID = LoginInfo.UserID;
                baseModel.CreateUserName = LoginInfo.RealName;
                baseModel.OperateStatus = (int)Common.Status.启用;
                CRM_CompanyContract contractModel = entity.Contract; // 合同信息
                contractModel.CreateTime = DateTime.Now;
                contractModel.CreateUserID = LoginInfo.UserID;
                contractModel.CreateUserName = LoginInfo.RealName;
                contractModel.Status = (int)Common.Status.启用;
                contractModel.BranchID = LoginInfo.BranchID;
                //公司分支机构
                CRM_CompanyToBranch branchModel = new CRM_CompanyToBranch();
                branchModel.BranchID = LoginInfo.BranchID;
                branchModel.UserID_XS = LoginInfo.UserID;
                branchModel.Status = (int)Common.Status.启用;
                //联系人信息
                List<CRM_CompanyLinkMan> listLink = new List<CRM_CompanyLinkMan>();
                string linkMan = entity.LinkMan;
                if (!string.IsNullOrEmpty(linkMan))
                {
                    listLink = GetLinkList(linkMan);
                }
                //银行信息
                List<CRM_CompanyBankAccount> listBank = new List<CRM_CompanyBankAccount>();
                string bank = entity.Bank;
                if (!string.IsNullOrEmpty(bank))
                    listBank = GetBankList(bank);
                //财务信息开票
                List<CRM_CompanyFinance_Bill> listBill = new List<CRM_CompanyFinance_Bill>();
                CRM_CompanyFinance_Bill billModel = entity.Bill;
                billModel.CreateTime = DateTime.Now;
                billModel.CreateUserID = LoginInfo.UserID;
                billModel.CreateUserName = LoginInfo.RealName;
                billModel.Status = (int)Common.Status.启用;
                billModel.BranchID = LoginInfo.BranchID;
                listBill.Add(billModel);
                //财务信息收款
                List<CRM_CompanyFinance_Payment> listPay = new List<CRM_CompanyFinance_Payment>();
                string payment = entity.Payment;
                if (!string.IsNullOrEmpty(payment))
                    listPay = GetPayList(payment);
                //企业报价
                List<CRM_CompanyPrice> listPrice = new List<CRM_CompanyPrice>();
                string price = entity.Price;
                if (!string.IsNullOrEmpty(price))
                    listPrice = GetPriceList(price);
                //企业阶梯报价
                List<CRM_CompanyLadderPrice> listLadderPrice = new List<CRM_CompanyLadderPrice>();
                string ladderPrice = entity.LadderPrice;
                if (!string.IsNullOrEmpty(ladderPrice))
                    listLadderPrice = GetLadderPriceList(ladderPrice);
                //企业社保政策和社保信息
                SheBao shebao = Newtonsoft.Json.JsonConvert.DeserializeObject<SheBao>(entity.SheBaoInfo);
                List<CRM_Company_PoliceInsurance> CompanyPoliceInsurance = GetPoliceInsuance(shebao);
                List<CRM_Company_Insurance> CompanyInsurance = GetInsurance(shebao);
                string returnValue = string.Empty;
                if (m_BLL.CreateNewCompany(ref validationErrors, baseModel, contractModel, branchModel, listLink, listBank, listBill, listPay, listPrice, listLadderPrice, CompanyPoliceInsurance, CompanyInsurance))
                //if (m_BLL.CreateNewCompany(ref validationErrors, baseModel, contractModel, branchModel, listLink, listBank, listBill, listPay, listPrice, listLadderPrice))
                {
                    //LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，客户_企业信息_待审核的信息的Id为" + entity.ID, "客户_企业信息_待审核"
                    //);//写入日志 
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = Suggestion.InsertSucceed;
                    return result; //提示创建成功
                }
                else
                {
                    if (validationErrors != null && validationErrors.Count > 0)
                    {
                        validationErrors.All(a =>
                        {
                            returnValue += a.ErrorMessage;
                            return true;
                        });
                    }
                    //LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，客户_企业信息_待审核的信息，" + returnValue, "客户_企业信息_待审核"
                    //    );//写入日志                      
                    result.Code = Common.ClientCode.Fail;
                    result.Message = Suggestion.InsertFail + returnValue;
                    return result; //提示插入失败
                }
            }
            result.Code = Common.ClientCode.FindNull;
            result.Message = Suggestion.InsertFail + "，请核对输入的数据的格式"; //提示输入的数据的格式不对 
            return result;
        }
        //验证用户名唯一
        public Common.ClientResult.Result CheckCompanyName(string companyID, string CompanyName)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            int count = m_BLL.CheckCompanyName(companyID, CompanyName);
            if (count == 0)
            {
                result.Code = Common.ClientCode.FindNull;
                result.Message = "该公司名可以使用！";
            }
            else
            {
                result.Code = Common.ClientCode.Succeed;
                result.Message = "该公司名已存在！";
            }
            return result;
        }

        /// <summary>
        /// 根据社保种类ID缴纳地获取政策内容
        /// </summary>
        /// <param name="InsuranceKindId"></param>
        /// <param name="CityID"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult GetZCByID(int InsuranceKindId, string CityID)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            dynamic listPoliceInsurance = m_BLL.GetZCByID(InsuranceKindId, CityID);
            int total = 0;
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = listPoliceInsurance
            };
            return data;
        }
        #region 内置
        //获得联系人信息
        public List<CRM_CompanyLinkMan> GetLinkList(string linkMan)
        {
            List<CRM_CompanyLinkMan> list = new List<CRM_CompanyLinkMan>();
            if (!string.IsNullOrEmpty(linkMan))
            {
                string[] arrGroup = linkMan.Split('^');
                string[] arrItem;
                for (int i = 0; i < arrGroup.Length; i++)
                {
                    CRM_CompanyLinkMan model = new CRM_CompanyLinkMan();
                    arrItem = arrGroup[i].Split('&');
                    model.LinkManName = arrItem[0];
                    model.Position = arrItem[1];
                    model.Address = arrItem[2];
                    model.Mobile = arrItem[3];
                    model.Telephone = arrItem[4];
                    model.Email = arrItem[5];
                    model.IsDefault = arrItem[6];
                    model.CreateTime = DateTime.Now;
                    model.CreateUserID = LoginInfo.UserID;
                    model.CreateUserName = LoginInfo.RealName;
                    model.BranchID = LoginInfo.BranchID;
                    model.Status = (int)Common.Status.启用;
                    list.Add(model);
                }
            }
            return list;
        }
        // 获取银行信息
        public List<CRM_CompanyBankAccount> GetBankList(string bank)
        {
            List<CRM_CompanyBankAccount> list = new List<CRM_CompanyBankAccount>();
            if (!string.IsNullOrEmpty(bank))
            {
                string[] arrGroup = bank.Split('^');
                string[] arrItem;
                for (int i = 0; i < arrGroup.Length; i++)
                {
                    CRM_CompanyBankAccount model = new CRM_CompanyBankAccount();
                    arrItem = arrGroup[i].Split('&');
                    model.Bank = arrItem[0];
                    model.Account = arrItem[1];

                    model.CreateTime = DateTime.Now;
                    model.CreateUserID = LoginInfo.UserID;
                    model.CreateUserName = LoginInfo.RealName;
                    model.BranchID = LoginInfo.BranchID;
                    model.Status = (int)Common.Status.启用;
                    list.Add(model);
                }
            }
            return list;
        }
        // 获取回款信息
        public List<CRM_CompanyFinance_Payment> GetPayList(string payment)
        {
            List<CRM_CompanyFinance_Payment> list = new List<CRM_CompanyFinance_Payment>();
            if (!string.IsNullOrEmpty(payment))
            {
                string[] arrGroup = payment.Split('^');
                string[] arrItem;
                for (int i = 0; i < arrGroup.Length; i++)
                {
                    CRM_CompanyFinance_Payment model = new CRM_CompanyFinance_Payment();
                    arrItem = arrGroup[i].Split('&');
                    model.PaymentName = arrItem[0];

                    model.CreateTime = DateTime.Now;
                    model.CreateUserID = LoginInfo.UserID;
                    model.CreateUserName = LoginInfo.RealName;
                    model.BranchID = LoginInfo.BranchID;
                    model.Status = (int)Common.Status.启用;
                    list.Add(model);
                }
            }
            return list;
        }
        // 获取企业报价
        public List<CRM_CompanyPrice> GetPriceList(string price)
        {
            List<CRM_CompanyPrice> list = new List<CRM_CompanyPrice>();
            if (!string.IsNullOrEmpty(price))
            {
                string[] arrGroup = price.Split('^');
                string[] arrItem;
                for (int i = 0; i < arrGroup.Length; i++)
                {
                    CRM_CompanyPrice model = new CRM_CompanyPrice();
                    arrItem = arrGroup[i].Split('&');
                    model.PRD_Product_ID = int.Parse(arrItem[0]);
                    if (!string.IsNullOrEmpty(arrItem[2]))
                        model.LowestPrice = Convert.ToDecimal(arrItem[2]);
                    if (!string.IsNullOrEmpty(arrItem[3]))
                        model.AddPrice = Convert.ToDecimal(arrItem[3]);
                    model.CreateTime = DateTime.Now;
                    model.CreateUserID = LoginInfo.UserID;
                    model.CreateUserName = LoginInfo.RealName;
                    model.BranchID = LoginInfo.BranchID;
                    model.Status = (int)Common.Status.启用;
                    list.Add(model);
                }
            }
            return list;
        }
        // 获取企业阶梯报价
        public List<CRM_CompanyLadderPrice> GetLadderPriceList(string ladderPrice)
        {
            List<CRM_CompanyLadderPrice> list = new List<CRM_CompanyLadderPrice>();
            if (!string.IsNullOrEmpty(ladderPrice))
            {
                string[] arrGroup = ladderPrice.Split('^');
                string[] arrItem;
                for (int i = 0; i < arrGroup.Length; i++)
                {
                    CRM_CompanyLadderPrice model = new CRM_CompanyLadderPrice();
                    arrItem = arrGroup[i].Split('&');
                    model.PRD_Product_ID = int.Parse(arrItem[0]);
                    if (!string.IsNullOrEmpty(arrItem[2]))
                        model.SinglePrice = Convert.ToDecimal(arrItem[2]);
                    if (!string.IsNullOrEmpty(arrItem[3]))
                        model.BeginLadder = Convert.ToInt32(arrItem[3]);
                    if (!string.IsNullOrEmpty(arrItem[4]))
                        model.EndLadder = Convert.ToInt32(arrItem[4]);
                    model.CreateTime = DateTime.Now;
                    model.CreateUserID = LoginInfo.UserID;
                    model.CreateUserName = LoginInfo.RealName;
                    model.BranchID = LoginInfo.BranchID;
                    model.Status = (int)Common.Status.启用;
                    list.Add(model);
                }
            }
            return list;
        }
        /// <summary>
        /// 社保政策
        /// </summary>
        /// <param name="shebaoInfo"></param>
        /// <returns></returns>
        public List<CRM_Company_PoliceInsurance> GetPoliceInsuance(SheBao shebao)
        {

            List<CRM_Company_Insurance> list = new List<CRM_Company_Insurance>();
            List<CRM_Company_PoliceInsurance> listPoliceInsurance = new List<CRM_Company_PoliceInsurance>();
            DateTime dtnow = DateTime.Now;
            string[] arrItem;
            for (int i = 0; i < shebao.data.Count; i++)
            {

                if (shebao.data[i].GongShangZhengCe != null && shebao.data[i].GongShangZhengCe != "")
                {
                    //社保政策
                    string[] GSZhengCe = shebao.data[i].GongShangZhengCe.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < GSZhengCe.Length; j++)
                    {
                        CRM_Company_PoliceInsurance model = new CRM_Company_PoliceInsurance();
                        model.City = shebao.data[i].JiaoNaDi;
                        model.CreatePerson = LoginInfo.RealName;
                        model.CreateTime = dtnow;
                        model.InsuranceKind = (int)Common.EmployeeAdd_InsuranceKindId.工伤;
                        model.PoliceInsurance = Convert.ToInt32(GSZhengCe[j]);
                        model.State = shebao.data[i].State;
                        listPoliceInsurance.Add(model);
                    }

                }
                if (shebao.data[i].GongJiJinZhengCe != null && shebao.data[i].GongJiJinZhengCe != "")
                {
                    string[] GSZhengCe = shebao.data[i].GongJiJinZhengCe.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < GSZhengCe.Length; j++)
                    {
                        CRM_Company_PoliceInsurance model = new CRM_Company_PoliceInsurance();
                        model.City = shebao.data[i].JiaoNaDi;
                        model.CreatePerson = LoginInfo.RealName;
                        model.CreateTime = dtnow;
                        model.InsuranceKind = (int)Common.EmployeeAdd_InsuranceKindId.公积金;
                        model.PoliceInsurance = Convert.ToInt32(GSZhengCe[j]);
                        model.State = shebao.data[i].State;
                        listPoliceInsurance.Add(model);
                    }
                }
            }

            return listPoliceInsurance;
        }
        /// <summary>
        /// 社保信息
        /// </summary>
        /// <param name="shebaoInfo"></param>
        /// <returns></returns>
        public List<CRM_Company_Insurance> GetInsurance(SheBao shebao)
        {

            List<CRM_Company_Insurance> list = new List<CRM_Company_Insurance>();
            DateTime dtnow = DateTime.Now;

            string[] arrItem;
            for (int i = 0; i < shebao.data.Count; i++)
            {
                if (shebao.data[i].QiYeSheBaoAccount != null && shebao.data[i].QiYeSheBaoAccount != "")
                {
                    string[] SheBaoAccount = shebao.data[i].QiYeSheBaoAccount.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] SheBaoName = shebao.data[i].QiYeSheBaoName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < SheBaoAccount.Length; j++)
                    {
                        CRM_Company_Insurance model = new CRM_Company_Insurance();
                        model.City = shebao.data[i].JiaoNaDi;
                        model.InsuranceKind = (int)(Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), SheBaoName[j]);
                        model.Account = SheBaoAccount[j];
                        model.State = shebao.data[i].State;
                        model.CreateTime = dtnow;
                        model.CreatePerson = LoginInfo.RealName;
                        list.Add(model);
                    }
                }

                if (shebao.data[i].GongJiJinAccount != null && shebao.data[i].GongJiJinAccount != "")
                {
                    //string[] GongJiJinAccount = shebao.data[i].GongJiJinAccount.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    //string[] SheBaoName = shebao.data[i].QiYeSheBaoName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    //for (int j = 0; j < GongJiJinAccount.Length; j++)
                    //{
                    CRM_Company_Insurance model = new CRM_Company_Insurance();
                    model.City = shebao.data[i].JiaoNaDi;
                    model.InsuranceKind = (int)Common.EmployeeAdd_InsuranceKindId.公积金;
                    model.Account = shebao.data[i].GongJiJinAccount;
                    model.State = shebao.data[i].State;
                    model.CreateTime = dtnow;
                    model.CreatePerson = LoginInfo.RealName;
                    list.Add(model);
                    //}
                }
            }

            return list;
        }
        #endregion
        IBLL.ICRM_CompanyBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public CRM_CompanyApiController()
            : this(new CRM_CompanyBLL()) { }

        public CRM_CompanyApiController(CRM_CompanyBLL bll)
        {
            m_BLL = bll;
        }

    }
}


