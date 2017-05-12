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
    /// 客户_企业阶梯报价_待审核
    /// </summary>
    public class CRM_CompanyLadderPrice_AuditApiController : BaseApiController
    {
        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            int intBranchID = 1;
            string strSearch = "";
            strSearch += "BranchIDDDL_Int&" + intBranchID;
            strSearch += "^CRM_Company_IDDDL_Int&" + getParam.search;

            List<CRM_CompanyLadderPrice_Audit> queryData = m_BLL.GetByParam(getParam.id, getParam.order, getParam.sort, strSearch);
            var data = new Common.ClientResult.DataResult
            {
                total = queryData.Count,
                rows = queryData.Select(s => new
                {
                    ID = s.ID
                    ,
                    CRM_CompanyLadderPrice_ID = s.CRM_CompanyLadderPrice_ID
                    ,
                    CRM_Company_Audit_ID = s.CRM_Company_Audit_ID
                    ,
                    CRM_Company_ID = s.CRM_Company_ID
                    ,
                    PRD_Product_ID = s.PRD_Product_ID
                    ,
                    SinglePrice = s.SinglePrice
                    ,
                    EndLadder = s.EndLadder
                    ,
                    BeginLadder = s.BeginLadder
                    ,
                    BranchID = s.BranchID
                    ,
                    OperateStatus = s.OperateStatus
                    ,
                    OperateNode = s.OperateNode


                })
            };
            return data;
        }

        /// <summary>
        /// 根据ID获取数据模型
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public CRM_CompanyLadderPrice_Audit Get(int id)
        {
            CRM_CompanyLadderPrice_Audit item = m_BLL.GetById(id);
            return item;
        }

        /// <summary>
        /// 根据ID获取数据
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public string GetLadderPrice_Audit(int id)
        {
            string item = m_BLL.GetCompanyLadderPrice_Audit(id);
            return item;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public Common.ClientResult.Result Post([FromBody]CRM_CompanyLadderPrice_Audit entity)
        {

            Common.ClientResult.Result result = new Common.ClientResult.Result();
            IBLL.ICRM_CompanyLadderPriceBLL bll = new CRM_CompanyLadderPriceBLL();
            if (entity != null && ModelState.IsValid)
            {

                int currentPerson = LoginInfo.UserID;
                string currentPersonName = LoginInfo.RealName;
                int intBranchID = LoginInfo.BranchID;
                entity.CreateTime = DateTime.Now;
                entity.CreateUserID = currentPerson;
                entity.CreateUserName = currentPersonName;
                entity.BranchID = intBranchID;
                entity.OperateStatus = 1;//待处理
                entity.OperateNode = 2;//质控

                if (!bll.CheckRange(Convert.ToInt32(entity.CRM_Company_ID), entity.PRD_Product_ID, Convert.ToInt32(entity.BeginLadder), Convert.ToInt32(entity.EndLadder), intBranchID, null))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业阶梯报价信息", "客户_企业阶梯报价信息_启用"
                      );//写入日志   
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "请检查阶梯报价范围！";
                    return result; //提示更新失败
                }

                string returnValue = string.Empty;
                if (m_BLL.Create(ref validationErrors, entity))
                {
                    LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，客户_企业阶梯报价_待审核的信息的Id为" + entity.ID, "客户_企业阶梯报价_待审核"
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，客户_企业阶梯报价_待审核的信息，" + returnValue, "客户_企业阶梯报价_待审核"
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

        // PUT api/<controller>/5
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>  
        public Common.ClientResult.Result Put([FromBody]CRM_CompanyLadderPrice entity)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            IBLL.ICRM_CompanyLadderPriceBLL bll = new CRM_CompanyLadderPriceBLL();
            if (entity != null && ModelState.IsValid)
            {
                //数据校验
                if (!bll.CheckRange(entity.CRM_Company_ID, entity.PRD_Product_ID, Convert.ToInt32(entity.BeginLadder), Convert.ToInt32(entity.EndLadder), LoginInfo.BranchID, entity.ID))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业阶梯报价信息的Id为" + entity.ID, "客户_企业阶梯报价"
                      );//写入日志   
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "请检查阶梯报价范围！";
                    return result; //提示更新失败
                }

                CRM_CompanyLadderPrice_Audit model = GetModel(entity);

                string returnValue = string.Empty;
                if (m_BLL.ModifyLadderPrice(ref validationErrors, model))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，客户_企业阶梯报价信息的Id为" + entity.ID, "客户_企业阶梯报价_待审批"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业阶梯报价_待审批信息的Id为" + entity.ID + "," + returnValue, "客户_企业阶梯报价_待审批"
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

        /// <summary>
        /// 退回企业阶梯报价信息修改审核
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
                    LogClassModels.WriteServiceLog("操作成功" + "，客户_企业阶梯报价信息_审核未通过的Id为" + ID, "客户_企业阶梯报价信息_审核修改内容"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业阶梯报价信息_审核未通过的Id为" + ID + "," + returnValue, "客户_企业阶梯报价信息_审核修改内容"
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
        /// 通过企业阶梯报价信息修改审核
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
                    LogClassModels.WriteServiceLog("操作成功" + "，客户_企业阶梯报价信息_审核通过的Id为" + ID, "客户_企业阶梯报价信息_审核修改内容"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业阶梯报价信息_审核通过的Id为" + ID + "," + returnValue, "客户_企业阶梯报价信息_审核修改内容"
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
        /// 退回企业阶梯报价信息添加审核
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
                    LogClassModels.WriteServiceLog("操作成功" + "，客户_企业阶梯报价信息_审核未通过的Id为" + ID, "客户_企业阶梯报价信息_审核修改内容"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业阶梯报价信息_审核未通过的Id为" + ID + "," + returnValue, "客户_企业阶梯报价信息_审核修改内容"
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
        /// 通过企业阶梯报价信息修改审核
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
                    LogClassModels.WriteServiceLog("操作成功" + "，客户_企业阶梯报价信息_审核通过的Id为" + ID, "客户_企业阶梯报价信息_审核修改内容"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业阶梯报价信息_审核通过的Id为" + ID + "," + returnValue, "客户_企业阶梯报价信息_审核修改内容"
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
        public CRM_CompanyLadderPrice_Audit GetModel(CRM_CompanyLadderPrice item)
        {
            CRM_CompanyLadderPrice_Audit model = new CRM_CompanyLadderPrice_Audit();
            object value;
            if (item != null)
            {
                string[] arrField = new string[] { "CRM_Company_ID", "PRD_Product_ID", "SinglePrice", "EndLadder", "BeginLadder" };
                Type t1 = typeof(CRM_CompanyLadderPrice);
                PropertyInfo[] propertys1 = t1.GetProperties();
                Type t2 = typeof(CRM_CompanyLadderPrice_Audit);
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
                model.CRM_CompanyLadderPrice_ID = item.ID;
                model.CreateTime = DateTime.Now;
                model.CreateUserID = LoginInfo.UserID;
                model.CreateUserName = LoginInfo.RealName;
                model.BranchID = LoginInfo.BranchID;
                model.OperateStatus = 1;//待处理
                model.OperateNode = 2;//质控
            }
            return model;
        }
        #endregion

        IBLL.ICRM_CompanyLadderPrice_AuditBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public CRM_CompanyLadderPrice_AuditApiController()
            : this(new CRM_CompanyLadderPrice_AuditBLL()) { }

        public CRM_CompanyLadderPrice_AuditApiController(CRM_CompanyLadderPrice_AuditBLL bll)
        {
            m_BLL = bll;
        }

    }
}


