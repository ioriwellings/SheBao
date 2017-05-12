using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Models;
using Common;
using Langben.DAL;
using Langben.BLL;
using System.Web.Http;
using System.Reflection;
using System.Text;
using Langben.DAL.Model;

namespace Langben.App.Areas.CRM.Controllers
{
    /// <summary>
    /// 客户_企业信息_待审核
    /// </summary>
    public class CRM_Company_AuditApiController : BaseApiController
    {
        private string menuID = "";

        /// <summary>
        /// 根据ID获取数据模型
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public string GetBaseInfo(int id)
        {
            string item = m_BLL.GetCompanyBaseAudit(id);
            return item;
        }
        /// <summary>
        /// 异步加载数据--待审核公司信息
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            StringBuilder sqlWhere = new StringBuilder();
            sqlWhere.Append("OperateNodeDDL_Int&1");
            string companyName = string.Empty;
            string operateStatus = string.Empty;
            if (!string.IsNullOrEmpty(getParam.search))
            {
                string[] search = getParam.search.Split('^');
                companyName = search[0];
                operateStatus = search[1];
            }

            var data = m_BLL.GetAuditCompanyList(getParam.page, getParam.rows, companyName, operateStatus);
            return data;
        }

        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostReSubmitData([FromBody]GetDataParam getParam)
        {
            int total = 0;
            int intBranchID = LoginInfo.BranchID;
            string strCompanyName = getParam.search ?? "";
            int intUserID_XS = LoginInfo.UserID;
            List<CRM_Company_Audit> queryData = m_BLL.GetCompany_AuditListForReSubmit(getParam.id, getParam.page, getParam.rows, strCompanyName, intUserID_XS, intBranchID, ref total);

            //List<CRM_Company_Audit> queryData = m_BLL.GetByParam(getParam.id, getParam.page, getParam.rows, getParam.order, getParam.sort, getParam.search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }

        /// <summary>
        /// 异步加载数据-企业审核列表
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostAuditData([FromBody]GetDataParam getParam)
        {
            //int intBranchID = LoginInfo.BranchID;

            #region 获取权限配置
            //部门范围权限
            int departmentScope = base.MenuDepartmentScopeAuthority(menuID);
            string departments = "";

            if (departmentScope == (int)DepartmentScopeAuthority.无限制)//无限制
            {
                //部门业务权限
                departments = MenuDepartmentAuthority(menuID);
            }
            #endregion

            string companyName = "";
            string companyCode = "";
            int? userID_ZR = null;
            int? userID_XS = null;
            int? auditType = null;
            int? operateStatus = null;

            if (!string.IsNullOrEmpty(getParam.search))
            {
                string[] searchs = getParam.search.Split('^');
                int intUserID = LoginInfo.UserID;

                companyName = searchs[0];
                companyCode = searchs[5];

                if (!string.IsNullOrEmpty(searchs[1]))
                {
                    userID_ZR = int.Parse(searchs[1]);
                }

                if (!string.IsNullOrEmpty(searchs[2]))
                {
                    userID_XS = int.Parse(searchs[2]);
                }

                if (!string.IsNullOrEmpty(searchs[3]))
                {
                    auditType = int.Parse(searchs[3]);
                }

                if (!string.IsNullOrEmpty(searchs[4]))
                {
                    operateStatus = int.Parse(searchs[4]);
                }
            }
            var queryData = m_BLL.GetAuditCompanyListForQuality(getParam.page, getParam.rows, companyName, userID_ZR, userID_XS, auditType, operateStatus, companyCode, departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID);

            return queryData;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public Common.ClientResult.Result Post([FromBody]CRM_Company entity)
        {

            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {
                CRM_Company_Audit model = GetModel(entity);

                string returnValue = string.Empty;
                if (m_BLL.ModifyBaseInfo(ref validationErrors, model))
                {
                    LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，客户_企业信息_待审核的信息的Id为" + entity.ID, "客户_企业信息_待审核"
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，客户_企业信息_待审核的信息，" + returnValue, "客户_企业信息_待审核"
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
        public Common.ClientResult.Result Put([FromBody]CRM_Company_Audit entity)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {   //数据校验

                //string currentPerson = GetCurrentPerson();
                //entity.UpdateTime = DateTime.Now;
                //entity.UpdatePerson = currentPerson;

                string returnValue = string.Empty;
                if (m_BLL.Edit(ref validationErrors, entity))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，客户_企业信息_待审核信息的Id为" + entity.ID, "客户_企业信息_待审核"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业信息_待审核信息的Id为" + entity.ID + "," + returnValue, "客户_企业信息_待审核"
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
        /// 修改企业基本信息待审核数据
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public Common.ClientResult.Result PutOnly([FromBody]CRM_Company_Audit model)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (model != null && ModelState.IsValid)
            {
                string returnValue = string.Empty;
                if (m_BLL.Edit(ref validationErrors, model))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，客户_企业信息_待审核的信息的Id为" + model.ID, "客户_企业信息_待审核"
                        );//写入日志 
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = Suggestion.UpdateSucceed;
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业信息_待审核的信息，" + returnValue, "客户_企业信息_待审核"
                        );//写入日志                      
                    result.Code = Common.ClientCode.Fail;
                    result.Message = Suggestion.UpdateFail + returnValue;
                    return result; //提示插入失败
                }
            }

            result.Code = Common.ClientCode.FindNull;
            result.Message = Suggestion.UpdateFail + "，请核对输入的数据的格式"; //提示输入的数据的格式不对 
            return result;
        }

        // DELETE api/<controller>/5
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>  
        public Common.ClientResult.Result Delete(string query)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            string returnValue = string.Empty;
            int[] deleteId = Array.ConvertAll<string, int>(query.Split(','), delegate(string s) { return int.Parse(s); });
            if (deleteId != null && deleteId.Length > 0)
            {
                if (m_BLL.DeleteCollection(ref validationErrors, deleteId))
                {
                    LogClassModels.WriteServiceLog(Suggestion.DeleteSucceed + "，信息的Id为" + string.Join(",", deleteId), "消息"
                        );//删除成功，写入日志
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = Suggestion.DeleteSucceed;
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
                    LogClassModels.WriteServiceLog(Suggestion.DeleteFail + "，信息的Id为" + string.Join(",", deleteId) + "," + returnValue, "消息"
                        );//删除失败，写入日志
                    result.Code = Common.ClientCode.Fail;
                    result.Message = Suggestion.DeleteFail + returnValue;
                }
            }
            return result;
        }

        // 销售经理审核
        public Common.ClientResult.Result NotPass(string id)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (!string.IsNullOrEmpty(id))
            {
                CRM_Company_Audit entity = m_BLL.GetById(int.Parse(id));
                entity.OperateStatus = 0;//审核不通过
                string returnValue = string.Empty;
                if (m_BLL.Edit(ref validationErrors, entity))
                {
                    LogClassModels.WriteServiceLog("操作成功" + "，客户_企业信息_待审核信息的Id为" + entity.ID, "客户_企业信息_待审核"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业信息_待审核信息的Id为" + entity.ID + "," + returnValue, "客户_企业信息_待审核"
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
        /// 退回基本信息修改审核
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
                if (m_BLL.ReturnBaseEdit(validationErrors, ID, MainTableID))
                {
                    LogClassModels.WriteServiceLog("操作成功" + "，客户_企业信息_审核未通过的Id为" + ID, "客户_企业信息_审核修改内容"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业信息_审核未通过的Id为" + ID + "," + returnValue, "客户_企业信息_审核修改内容"
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
        /// 通过基本信息修改审核
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
                if (m_BLL.PassBaseEdit(validationErrors, ID, MainTableID))
                {
                    LogClassModels.WriteServiceLog("操作成功" + "，客户_企业信息_审核通过的Id为" + ID, "客户_企业信息_审核修改内容"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业信息_审核通过的Id为" + ID + "," + returnValue, "客户_企业信息_审核修改内容"
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
        public CRM_Company_Audit GetModel(CRM_Company item)
        {
            CRM_Company_Audit model = new CRM_Company_Audit();
            object value;
            if (item != null)
            {
                string[] arrField = new string[] { "CompanyCode", "CompanyName", "Dict_HY_Code", "OrganizationCode", "RegisterAddress", "OfficeAddress" };
                Type t1 = typeof(CRM_Company);
                PropertyInfo[] propertys1 = t1.GetProperties();
                Type t2 = typeof(CRM_Company_Audit);
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
                model.CRM_Company_ID = item.ID;
                model.CreateTime = DateTime.Now;
                model.CreateUserID = LoginInfo.UserID;
                model.CreateUserName = LoginInfo.RealName;
                model.BranchID = LoginInfo.BranchID;
                model.Source = 2;
                model.OperateStatus = 1;
                model.OperateNode = 2;//质控
            }
            return model;
        }
        #endregion

        IBLL.ICRM_Company_AuditBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public CRM_Company_AuditApiController()
            : this(new CRM_Company_AuditBLL()) { }

        public CRM_Company_AuditApiController(CRM_Company_AuditBLL bll)
        {
            m_BLL = bll;
        }

    }
}


