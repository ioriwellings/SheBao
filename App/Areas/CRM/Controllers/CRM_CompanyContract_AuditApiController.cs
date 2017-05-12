using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Common;
using Langben.BLL;
using Langben.DAL;
using Models;

namespace Langben.App.Areas.CRM.Controllers
{
    public class CRM_CompanyContract_AuditApiController : BaseApiController
    {
        IBLL.ICRM_CompanyContract_AuditBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public CRM_CompanyContract_AuditApiController()
            : this(new CRM_CompanyContract_AuditBLL()) { }

        public CRM_CompanyContract_AuditApiController(CRM_CompanyContract_AuditBLL bll)
        {
            m_BLL = bll;
        }

        /// <summary>
        /// 根据ID获取数据模型
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public CRM_CompanyContract_Audit Get(int id)
        {
            CRM_CompanyContract_Audit item = m_BLL.GetById(id);
            return item;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns> 
        public Common.ClientResult.Result Post([FromBody]CRM_CompanyContract_Audit entity)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {
                entity.CreateUserID = LoginInfo.UserID;
                entity.CreateTime = DateTime.Now;
                entity.CreateUserName = LoginInfo.RealName;
                entity.BranchID = 1;
                entity.OperateStatus = 1;
                entity.OperateNode = 2;


                string returnValue = string.Empty;
                if (m_BLL.Create(ref validationErrors, entity))
                {
                    LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，客户_合同信息_待审核的信息的Id为" + entity.ID, "客户_合同信息_待审核"
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，客户_合同信息_待审核的信息，" + returnValue, "客户_合同信息_待审核"
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
        /// 提交编辑信息
        /// </summary> 
        public Common.ClientResult.Result Put([FromBody]CRM_CompanyContract entity)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {
                CRM_CompanyContract_Audit model = GetModel(entity);

                string returnValue = string.Empty;
                if (m_BLL.ModifyContract(ref validationErrors, model))
                {
                    LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，客户_企业合同信息_待审核的信息的Id为" + entity.ID, "客户_企业合同信息_待审核"
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，客户_企业合同信息_待审核的信息，" + returnValue, "客户_企业合同信息_待审核"
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
        /// 退回企业合同信息修改审核
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
                    LogClassModels.WriteServiceLog("操作成功" + "，客户_企业合同信息_审核未通过的Id为" + ID, "客户_企业合同信息_审核修改内容"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业合同信息_审核未通过的Id为" + ID + "," + returnValue, "客户_企业合同信息_审核修改内容"
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
        /// 通过企业合同信息修改审核
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
                    LogClassModels.WriteServiceLog("操作成功" + "，客户_企业合同信息_审核通过的Id为" + ID, "客户_企业合同信息_审核修改内容"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业合同信息_审核通过的Id为" + ID + "," + returnValue, "客户_企业合同信息_审核修改内容"
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
        /// 退回企业合同信息添加审核
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
                    LogClassModels.WriteServiceLog("操作成功" + "，客户_企业合同信息_审核未通过的Id为" + ID, "客户_企业合同信息_审核修改内容"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业合同信息_审核未通过的Id为" + ID + "," + returnValue, "客户_企业合同信息_审核修改内容"
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
        /// 通过企业合同信息修改审核
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
                    LogClassModels.WriteServiceLog("操作成功" + "，客户_企业合同信息_审核通过的Id为" + ID, "客户_企业合同信息_审核修改内容"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业合同信息_审核通过的Id为" + ID + "," + returnValue, "客户_企业合同信息_审核修改内容"
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
        public CRM_CompanyContract_Audit GetModel(CRM_CompanyContract item)
        {
            CRM_CompanyContract_Audit model = new CRM_CompanyContract_Audit();
            object value;
            if (item != null)
            {
                string[] arrField = new string[] { "CRM_Company_ID", "BillDay", "ReceivedDay", "FeesCycle", "ChangeDay", "DatumDay", "ServceEndDay", "SendBillDay", "ServiceBeginDay" };
                Type t1 = typeof(CRM_CompanyContract);
                PropertyInfo[] propertys1 = t1.GetProperties();
                Type t2 = typeof(CRM_CompanyContract_Audit);
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
                model.CRM_CompanyContract_ID = item.ID;
                model.CreateTime = DateTime.Now;
                model.CreateUserID = LoginInfo.UserID;
                model.CreateUserName = LoginInfo.RealName;
                model.BranchID = LoginInfo.BranchID;
                model.OperateStatus = 1;
                model.OperateNode = 2;//质控
            }
            return model;
        }
        #endregion
    }
}