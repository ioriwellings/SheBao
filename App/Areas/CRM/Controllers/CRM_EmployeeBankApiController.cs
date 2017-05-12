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
using System.Web;
namespace Langben.App.Areas.CRM.Controllers
{
    public class CRM_EmployeeBankApiController : BaseApiController
    {
        ValidationErrors validationErrors = new ValidationErrors();

        #region 银行信息
        /// <summary>
        /// 银行信息列表
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            IBLL.IEmployeeBankBLL m_BLL = new EmployeeBankBLL();
            int total = 0;
            List<EmployeeBank> queryData = m_BLL.GetByParam((int)getParam.id, getParam.page, getParam.rows, getParam.order, getParam.sort, getParam.search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData.Select(s => new
                {
                    Id = s.Id,
                    Bank = s.Bank,
                    BranchBank = s.BranchBank,
                    Account = s.Account,
                    AccountName = s.AccountName,
                    State = s.State
                })
            };
            return data;
        }

        /// <summary>
        /// 根据ID获取银行数据模型
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public string GetBank()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
            HttpRequestBase request = context.Request;//定义传统request对象             
            int id = int.Parse(request.QueryString["id"]);
            int eid = int.Parse(request.QueryString["eid"]);

            IBLL.IEmployeeBLL m_BLL = new EmployeeBLL();
            Employee item = m_BLL.GetById(eid);

            EmployeeInfo info = new EmployeeInfo();
            info.empId = item.Id;
            info.Empname = item.Name;
            info.CertificateNumber = item.CertificateNumber;

            info.bankList = (from a in item.EmployeeBank
                             where a.Id == id
                             select new EmployeeBanks
                             {
                                 Id = a.Id,
                                 Bank = a.Bank,
                                 BranchBank = a.BranchBank,
                                 Account = a.Account,
                                 AccountName = a.AccountName,
                                 //IsDefault=a.IsDefault,
                                 Remark = a.Remark,
                                 BState = a.State,
                                 CreateTime = a.CreateTime,
                                 CreatePerson = a.CreatePerson
                             }).ToList();
            return Newtonsoft.Json.JsonConvert.SerializeObject(info);
        }

        /// <summary>
        /// 创建银行信息
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public Common.ClientResult.Result Post([FromBody]EmployeeBank entity)
        {
            IBLL.IEmployeeBankBLL m_BLL = new EmployeeBankBLL();

            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {
                entity.State = "启用";
                entity.CreateTime = DateTime.Now;
                entity.CreatePerson = LoginInfo.RealName;
                //entity.EmployeeId = LoginInfo.UserID;
                string returnValue = string.Empty;

                if (m_BLL.Create(ref validationErrors, entity))
                {
                    LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，员工银行的信息的Id为" + entity.Id, "员工银行"
                        );//写入日志 
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，员工银行的信息，" + returnValue, "员工银行"
                        );//写入日志                      
                    result.Code = Common.ClientCode.Fail;
                    result.Message = Suggestion.InsertFail + returnValue;
                    return result; //提示插入失败
                }
            }

            result.Code = Common.ClientCode.FindNull;
            result.Message = Suggestion.InsertFail + "，请核对输入的数据的格式"; //提示输入的数据的格式不对 
            return result;
        }

        /// <summary>
        /// 编辑银行信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>  
        public Common.ClientResult.Result Put([FromBody]EmployeeBank entity)
        {
            IBLL.IEmployeeBankBLL b_BLL = new EmployeeBankBLL();
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {
                entity.UpdateTime = DateTime.Now;
                entity.UpdatePerson = LoginInfo.RealName;

                string returnValue = string.Empty;
                if (b_BLL.Edit(ref validationErrors, entity))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，员工银行信息的Id为" + entity.Id, "员工银行"
                        );//写入日志                   
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = Suggestion.UpdateSucceed;
                    return result; //提示更新成功 
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，员工银行信息的Id为" + entity.Id + "," + returnValue, "员工银行"
                        );//写入日志   
                    result.Code = Common.ClientCode.Fail;
                    result.Message = Suggestion.UpdateFail + returnValue;
                    return result; //提示更新失败
                }
            }
            result.Code = Common.ClientCode.FindNull;
            result.Message = Suggestion.UpdateFail + "请核对输入的数据的格式";
            return result; //提示输入的数据的格式不对         
        }

        #endregion
    }
}