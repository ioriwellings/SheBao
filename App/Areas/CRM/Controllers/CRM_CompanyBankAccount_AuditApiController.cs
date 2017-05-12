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
using System.Reflection;

namespace Langben.App.Areas.CRM.Controllers
{
    /// <summary>
    /// 客户_企业银行账户_待审核
    /// </summary>
    public class CRM_CompanyBankAccount_AuditApiController : BaseApiController
    {

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public Common.ClientResult.Result Post([FromBody]CRM_CompanyBankAccount_Audit entity)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {
                entity.CreateTime = DateTime.Now;
                entity.CreateUserID = LoginInfo.UserID;
                entity.CreateUserName = LoginInfo.RealName;
                entity.BranchID = 1;
                entity.OperateStatus = 1;
                entity.OperateNode = 2;

                string returnValue = string.Empty;
                if (m_BLL.Create(ref validationErrors, entity))
                {
                    LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，客户_企业银行账户_待审核的信息的Id为" + entity.ID, "客户_企业银行账户_待审核"
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，客户_企业银行账户_待审核的信息，" + returnValue, "客户_企业银行账户_待审核"
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
        /// 根据ID获取数据模型
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public CRM_CompanyBankAccount_Audit Get(int id)
        {
            CRM_CompanyBankAccount_Audit item = m_BLL.GetById(id);
            return item;
        }

        /// <summary>
        /// 修改银行信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Common.ClientResult.Result Put([FromBody]CRM_CompanyBankAccount entity)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {
                CRM_CompanyBankAccount_Audit model = GetModel(entity);

                string returnValue = string.Empty;
                if (m_BLL.ModifyBank(ref validationErrors, model))
                {
                    LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，客户_企业银行账户信息_待审核的信息的Id为" + entity.ID, "客户_企业银行账户信息_待审核"
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，客户_企业银行账户信息_待审核的信息，" + returnValue, "客户_企业银行账户信息_待审核"
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
        /// 退回企业银行信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        public Common.ClientResult.Result ReturnEdit(int ID, int MainTableID)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (ModelState.IsValid)
            {
                string returnValue = string.Empty;
                if (m_BLL.ReturnEdit(validationErrors, ID, MainTableID))
                {
                    LogClassModels.WriteServiceLog("操作成功" + "，客户_企业银行信息_审核未通过的Id为" + ID, "客户_企业银行信息_审核修改内容"
                        );//写入日志                   
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = "操作成功";
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业银行信息_审核未通过的Id为" + ID + "," + returnValue, "客户_企业银行信息_审核修改内容"
                        );//写入日志   
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "操作成功" + returnValue;
                    return result; //提示更新失败
                }
            }
            result.Code = Common.ClientCode.FindNull;
            result.Message = "操作成功" + "请核对输入的数据的格式";
            return result; //提示输入的数据的格式不对         
        }

        /// <summary>
        /// 通过企业银行信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        public Common.ClientResult.Result PassEdit(int ID, int MainTableID)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (ModelState.IsValid)
            {
                string returnValue = string.Empty;
                if (m_BLL.PassEdit(validationErrors, ID, MainTableID))
                {
                    LogClassModels.WriteServiceLog("操作成功" + "，客户_企业银行信息_审核通过的Id为" + ID, "客户_企业银行信息_审核修改内容"
                        );//写入日志                   
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = "操作成功";
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业银行信息_审核通过的Id为" + ID + "," + returnValue, "客户_企业银行信息_审核修改内容"
                        );//写入日志   
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "操作成功" + returnValue;
                    return result; //提示更新失败
                }
            }
            result.Code = Common.ClientCode.FindNull;
            result.Message = "操作成功" + "请核对输入的数据的格式";
            return result; //提示输入的数据的格式不对         
        }


        /// <summary>
        /// 退回企业银行信息添加审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <returns></returns>
        public Common.ClientResult.Result ReturnAdd(int ID)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (ModelState.IsValid)
            {
                string returnValue = string.Empty;
                if (m_BLL.ReturnAdd(validationErrors, ID))
                {
                    LogClassModels.WriteServiceLog("操作成功" + "，客户_企业银行信息_审核未通过的Id为" + ID, "客户_企业银行信息_审核修改内容"
                        );//写入日志                   
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = "操作成功";
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业银行信息_审核未通过的Id为" + ID + "," + returnValue, "客户_企业银行信息_审核修改内容"
                        );//写入日志   
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "操作成功" + returnValue;
                    return result; //提示更新失败
                }
            }
            result.Code = Common.ClientCode.FindNull;
            result.Message = "操作成功" + "请核对输入的数据的格式";
            return result; //提示输入的数据的格式不对         
        }

        /// <summary>
        /// 通过企业银行信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <returns></returns>
        public Common.ClientResult.Result PassAdd(int ID)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (ModelState.IsValid)
            {
                string returnValue = string.Empty;
                if (m_BLL.PassAdd(validationErrors, ID))
                {
                    LogClassModels.WriteServiceLog("操作成功" + "，客户_企业银行信息_审核通过的Id为" + ID, "客户_企业银行信息_审核修改内容"
                        );//写入日志                   
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = "操作成功";
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业银行信息_审核通过的Id为" + ID + "," + returnValue, "客户_企业银行信息_审核修改内容"
                        );//写入日志   
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "操作成功" + returnValue;
                    return result; //提示更新失败
                }
            }
            result.Code = Common.ClientCode.FindNull;
            result.Message = "操作成功" + "请核对输入的数据的格式";
            return result; //提示输入的数据的格式不对         
        }

        #region 内置
        //Model转换
        public CRM_CompanyBankAccount_Audit GetModel(CRM_CompanyBankAccount item)
        {
            CRM_CompanyBankAccount_Audit model = new CRM_CompanyBankAccount_Audit();
            object value;
            if (item != null)
            {
                string[] arrField = new string[] { "CRM_Company_ID", "Account", "Bank" };
                Type t1 = typeof(CRM_CompanyBankAccount);
                PropertyInfo[] propertys1 = t1.GetProperties();
                Type t2 = typeof(CRM_CompanyBankAccount_Audit);
                PropertyInfo[] propertys2 = t2.GetProperties();

                foreach (PropertyInfo pi in propertys2)
                {
                    string name = pi.Name;
                    if (arrField.Contains(name))
                    {
                        value = t1.GetProperty(name).GetValue(item, null);
                        t2.GetProperty(name).SetValue(model, value, null);
                    }
                }
                model.CRM_CompanyBankAccount_ID = item.ID;
                model.CreateTime = DateTime.Now;
                model.CreateUserID = LoginInfo.UserID;
                model.CreateUserName = LoginInfo.RealName;
                model.BranchID = 1;
                model.OperateStatus = 1;
                model.OperateNode = 2;//质控
            }
            return model;
        }
        #endregion
        IBLL.ICRM_CompanyBankAccount_AuditBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public CRM_CompanyBankAccount_AuditApiController()
            : this(new CRM_CompanyBankAccount_AuditBLL()) { }

        public CRM_CompanyBankAccount_AuditApiController(CRM_CompanyBankAccount_AuditBLL bll)
        {
            m_BLL = bll;
        }

    }
}


