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
using System.Linq.Expressions;

namespace Langben.App.Controllers
{
    /// <summary>
    /// 员工费用中间表
    /// </summary>
    public class EmployeeMiddleApiController : BaseApiController
    {
        string menuId = "1041";   // 菜单“费用中间表管理”

        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            BLL.EmployeeMiddleBLL mybll = new EmployeeMiddleBLL();
            int total = 0;

            int yearMonth = 0;
            int companyId = 0;
            int insuranceId = 0;
            string certificate = "";
            string name = "";

            // 各搜索项赋值
            Expression<Func<EmployeeMiddleShow, bool>> where = u => true;
            if (!string.IsNullOrEmpty(getParam.search))
            {
                string[] search = getParam.search.Split('^');
                if (search[0].Split('&')[1] != "")
                {
                    string start = search[0].Split('&')[1].Replace("-", "");
                    yearMonth = Convert.ToInt32(start);
                }
                if (search[1].Split('&')[1] != "" && search[1].Split('&')[1] != "null" && search[1].Split('&')[1] != "0")
                {// 公司
                    companyId = Convert.ToInt32(search[1].Split('&')[1]);
                }
                if (search[2].Split('&')[1] != "")
                {// 种类
                    insuranceId = Convert.ToInt32(search[2].Split('&')[1]);
                }
                // 身份证号
                certificate = search[3].Split('&')[1];
                // 员工姓名
                name = search[4].Split('&')[1];
            }

            #region 获取权限配置
            //部门范围权限
            int departmentScope = base.MenuDepartmentScopeAuthority(menuId);
            string departments = "";

            if (departmentScope == (int)DepartmentScopeAuthority.无限制)//无限制
            {
                //部门业务权限
                departments = MenuDepartmentAuthority(menuId);
            }
            #endregion

            List<EmployeeMiddleShow> queryData = mybll.GetData(getParam.page, getParam.rows, out total, yearMonth, companyId, insuranceId, certificate, name,
                departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData.Select(s => new
                {
                    EmployeeId = s.EmployeeId,
                    EmployeeName = s.EmployeeName,
                    CardId = s.CardId,
                    CompanyId = s.CompanyId,
                    CompanyName = s.CompanyName,
                    CompanyEmployeeRelationId = s.CompanyEmployeeRelationId,
                    InsuranceKindId = s.InsuranceKindId,
                    InsuranceKindName = Enum.GetName(typeof(Common.CostType), s.InsuranceKindId),
                    Id = s.Id,
                    PaymentBetween = s.PaymentBetween,
                    PaymentStyle = s.PaymentStyle,
                    PaymentStyleName = Enum.GetName(typeof(Common.EmployeeMiddle_PaymentStyle), s.PaymentStyle),
                    CompanyBasePayment = s.CompanyBasePayment,
                    CompanyPayment = s.CompanyPayment,
                    EmployeeBasePayment = s.EmployeeBasePayment,
                    EmployeePayment = s.EmployeePayment,
                    PaymentMonth = s.PaymentMonth,
                    StartDate = s.StartDate,
                    EndedDate = s.EndedDate,
                    UseBetween = s.UseBetween,
                    Remark = s.Remark,
                    State = s.State,
                    CreateTime = s.CreateTime,
                    CreatePerson = s.CreatePerson,
                    UpdateTime = s.UpdateTime,
                    UpdatePerson = s.UpdatePerson,
                    CityId = s.CityId
                })
            };
            return data;
        }

        /// <summary>
        /// 根据ID获取数据模型
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public EmployeeMiddle Get(int id)
        {
            EmployeeMiddle item = m_BLL.GetById(id);
            return item;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public Common.ClientResult.Result Post([FromBody]EmployeeMiddle entity)
        {

            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {
                entity.State = Common.Status.启用.ToString();
                entity.CreatePerson = LoginInfo.UserName;
                entity.CreateTime = DateTime.Now;

                string returnValue = string.Empty;
                if (m_BLL.Create(ref validationErrors, entity))
                {
                    LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，员工费用中间表的信息的Id为" + entity.Id, "员工费用中间表"
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，员工费用中间表的信息，" + returnValue, "员工费用中间表"
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
        /// 启用中间表数据
        /// </summary>
        /// <param name="id">中间表编号</param>
        /// <returns></returns>
        public Common.ClientResult.Result PostStart(int id)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            string returnValue = string.Empty;

            if (id != 0)
            {
                if (m_BLL.UpdateEmployeeMiddleState(ref validationErrors, id, Common.Status.启用.ToString(), LoginInfo.UserName))
                {
                    LogClassModels.WriteServiceLog("费用中间表数据启用成功" + "，信息的Id为" + id, "消息"
                        );//启用成功，写入日志
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = "启用成功";
                }
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
                LogClassModels.WriteServiceLog("费用中间表数据启用失败" + "，信息的Id为" + id + "," + returnValue, "消息"
                    );//启用失败，写入日志
                result.Code = Common.ClientCode.Fail;
                result.Message = "启用失败" + returnValue;
            }
            return result;
        }

        /// <summary>
        /// 停用中间表数据
        /// </summary>
        /// <param name="id">中间表编号</param>
        /// <returns></returns>
        public Common.ClientResult.Result PostStop(int id)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            string returnValue = string.Empty;

            if (id != 0)
            {
                if (m_BLL.UpdateEmployeeMiddleState(ref validationErrors, id, Common.Status.停用.ToString(), LoginInfo.UserName))
                {
                    LogClassModels.WriteServiceLog("费用中间表数据停用成功" + "，信息的Id为" + id, "消息"
                        );//停用成功，写入日志
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = "停用成功";
                }
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
                LogClassModels.WriteServiceLog("费用中间表数据停用失败" + "，信息的Id为" + id + "," + returnValue, "消息"
                    );//停用失败，写入日志
                result.Code = Common.ClientCode.Fail;
                result.Message = "停用失败" + returnValue;
            }
            return result;
        }

        // PUT api/<controller>/5
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>  
        public Common.ClientResult.Result Put([FromBody]EmployeeMiddle entity)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {   //数据校验
                EmployeeMiddle Model = m_BLL.GetById(entity.Id);

                //Model.Id = entity.Id;
                //Model.InsuranceKindId = entity.InsuranceKindId;
                //Model.CompanyEmployeeRelationId = entity.CompanyEmployeeRelationId;
                Model.PaymentBetween = entity.PaymentBetween;
                Model.PaymentStyle = entity.PaymentStyle;
                Model.CompanyBasePayment = entity.CompanyBasePayment;
                Model.CompanyPayment = entity.CompanyPayment;
                Model.EmployeeBasePayment = entity.EmployeeBasePayment;
                Model.EmployeePayment = entity.EmployeePayment;
                Model.PaymentMonth = entity.PaymentMonth;
                Model.StartDate = entity.StartDate;
                Model.EndedDate = entity.EndedDate;
                Model.UseBetween = entity.UseBetween;
                //Model.BillId = entity.BillId;
                Model.Remark = entity.Remark;
                //Model.State = entity.State;
                //Model.CreateTime = entity.CreateTime;
                //Model.CreatePerson = entity.CreatePerson;
                Model.UpdateTime = entity.UpdateTime;
                Model.UpdatePerson = entity.UpdatePerson;
                //Model.CityId = entity.CityId;

                Model.UpdateTime = DateTime.Now;
                Model.UpdatePerson = LoginInfo.UserName;

                string returnValue = string.Empty;
                if (m_BLL.Edit(ref validationErrors, Model))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，员工费用中间表信息的Id为" + entity.Id, "员工费用中间表"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，员工费用中间表信息的Id为" + entity.Id + "," + returnValue, "员工费用中间表"
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

        IBLL.IEmployeeMiddleBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public EmployeeMiddleApiController()
            : this(new EmployeeMiddleBLL()) { }

        public EmployeeMiddleApiController(EmployeeMiddleBLL bll)
        {
            m_BLL = bll;
        }


        /// <summary>
        /// 获取公司列表
        /// </summary>
        /// <returns></returns>
        public Common.ClientResult.DataResult GetCompany()
        {
            #region 获取权限配置
            //部门范围权限
            int departmentScope = base.MenuDepartmentScopeAuthority(menuId);
            string departments = "";

            if (departmentScope == (int)DepartmentScopeAuthority.无限制)//无限制
            {
                //部门业务权限
                departments = MenuDepartmentAuthority(menuId);
            }
            #endregion
           
            var query = m_BLL.GetCompanyListByGroup(departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID);  // 参数为该页面菜单ID
            var data = new Common.ClientResult.DataResult
            {
                rows = query.Select(s => new
                {
                    ID = s.ID,
                    Name = s.CompanyName
                })
            };
            return data;
        }

        #region 私有方法

        #endregion


    }
}


