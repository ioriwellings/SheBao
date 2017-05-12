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

namespace Langben.App.Controllers
{
    /// <summary>
    /// 费用支出
    /// </summary>
    public class COST_PayRecordApiController : BaseApiController
    {
        ValidationErrors validationErrors = new ValidationErrors();
        IBLL.ICOST_PayRecordStatusBLL m_BLL = new BLL.COST_PayRecordStatusBLL();

        string menuId = "1045";   // 支出费用确认菜单编码 
        string gropUser_SBKF = "SBKF";  // 用户组中“社保客服”的编码
        
        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam) 
        {
            int total = 0;
            // 设置搜索默认值
            string yearMonth = DateTime.Now.ToString("yyyy-MM").Replace("-", "");
            int costType = 0;  // 默认值为全部
            int personId = 0;
            string cityId = "";

            // 各搜索项赋值
            if (!string.IsNullOrEmpty(getParam.search))
            {
                string[] search = getParam.search.Split('^');
                yearMonth = search[0].Split('&')[1];
                costType = search[1].Split('&')[1] != "" ? Convert.ToInt32(search[1].Split('&')[1]) : 0;
                if (search[2].Split('&')[1] != "" && search[2].Split('&')[1] != "0")
                {
                    personId = Convert.ToInt32(search[2].Split('&')[1]);
                }
                cityId = search[3].Split('&')[1]; 
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

            List<COST_PayRecordConfirm> queryData = m_BLL.GetPayRecordList(getParam.id, getParam.page, getParam.rows, yearMonth, costType, personId, cityId,
                gropUser_SBKF, departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData.Select(s => new
                {
                    ID = s.ID,
                    CityId = s.CityId,
                    CityName = s.CityName
                    ,
                    CostType = ((Common.EmployeeAdd_InsuranceKindId)s.CostType).ToString()   // 险种
                    ,
                    CompanyCost = s.CompanyCost
                    ,
                    PersonCost = s.PersonCost  
                    ,
                    Sum = s.CompanyCost + s.PersonCost
                    ,
                    AllCount = s.AllCount  // 人数（导入条数）
                    ,
                    CreateUserID = s.CreateUserID  // 上传客服
                    ,
                    CreateUserName = s.CreateUserName
                    ,
                    CreateTime = s.CreateTime
                    ,
                    Status = s.Status
                    ,
                    StatusName = ((Common.COST_PayRecord_Status)s.Status).ToString()   // 状态
                })
            };
            return data;
        }

        // DELETE api/<controller>/5
        /// <summary>
        /// 删除支出费用(物理删除)
        /// </summary>   
        /// <param name="collection"></param>
        /// <returns></returns>  
        /// 
        [System.Web.Http.HttpPost]
        public Common.ClientResult.Result DeletePayRecord(int id)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            string returnValue = string.Empty;
            if (m_BLL.DeletePayRecord(ref validationErrors, id))
            {
                LogClassModels.WriteServiceLog(Suggestion.DeleteSucceed + "，信息的Id为" + id, "消息"
                    );//作废成功，写入日志
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
                LogClassModels.WriteServiceLog(Suggestion.DeleteFail + "，信息的Id为" + id + "," + returnValue, "消息"
                    );//作废失败，写入日志
                result.Code = Common.ClientCode.Fail;
                result.Message = Suggestion.DeleteFail + returnValue;
            }
            return result;
        }

        /// <summary>
        /// 解锁支出费用
        /// </summary>
        /// <param name="id">支出费用表ID</param>
        /// <returns></returns>
        public Common.ClientResult.Result UnLockedPayRecord(int id)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            string returnValue = string.Empty;
            if (m_BLL.UpdatePayRecordStatus(ref validationErrors, id, (int)Common.COST_PayRecord_Status.未锁定))
            {
                LogClassModels.WriteServiceLog("解锁成功" + "，信息的Id为" + id, "消息"
                    );//解锁成功，写入日志
                result.Code = Common.ClientCode.Succeed;
                result.Message = "解锁成功";
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
                LogClassModels.WriteServiceLog("解锁失败" + "，信息的Id为" + id + "," + returnValue, "消息"
                    );//解锁失败，写入日志
                result.Code = Common.ClientCode.Fail;
                result.Message = "解锁失败" + returnValue;
            }
            return result;
        }

        /// <summary>
        /// 锁定支出费用
        /// </summary>
        /// <param name="id">支出费用表ID</param>
        /// <returns></returns>
        public Common.ClientResult.Result LockedPayRecord(int id)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            string returnValue = string.Empty;
            if (m_BLL.UpdatePayRecordStatus(ref validationErrors, id, (int)Common.COST_PayRecord_Status.已锁定))
            {
                LogClassModels.WriteServiceLog("锁定成功" + "，信息的Id为" + id, "消息"
                    );//锁定成功，写入日志
                result.Code = Common.ClientCode.Succeed;
                result.Message = "锁定成功";
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
                LogClassModels.WriteServiceLog("锁定失败" + "，信息的Id为" + id + "," + returnValue, "消息"
                    );//锁定失败，写入日志
                result.Code = Common.ClientCode.Fail;
                result.Message = "锁定失败" + returnValue;
            }
            return result;
        }



        /// <summary>
        /// 根据ID获取数据模型
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public COST_PayRecordStatus Get(int id)
        {
            COST_PayRecordStatus item = m_BLL.GetById(id);
            return item;
        }

        /// <summary>
        /// 获取社保客服列表(有权限控制)
        /// </summary>
        /// <returns></returns>
        public Common.ClientResult.DataResult GetPerson()
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

            // 获取上传客服列表数据源
            var query = m_BLL.GetPersonListByGroupCode(gropUser_SBKF, departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID);
            var data = new Common.ClientResult.DataResult
            {
                rows = query.Select(s => new
                {
                    ID = s.ID,
                    Name = s.RName
                })
            };
            return data;
        }

        /// <summary>
        /// 获取缴纳地列表(社保客服自己负责的缴纳地)
        /// </summary>
        /// <returns></returns>
        public Common.ClientResult.DataResult GetCity()
        {
            var query = m_BLL.GetSBKFCityListByUser(LoginInfo.UserID);
            var data = new Common.ClientResult.DataResult
            {
                rows = query.Select(s => new
                {
                    ID = s.Id,
                    Name = s.Name
                })
            };
            return data;
        }

        /// <summary>
        /// 获取险种
        /// </summary>
        /// <returns></returns>
        public Common.ClientResult.DataResult GetCostType(string cityId)
        {
            var query = m_BLL.GetSBKFCostTypeByCity(LoginInfo.UserID, cityId);

            var data = new Common.ClientResult.DataResult
            {
                rows = query.Select(s => new
                {
                    Code = s.Code,
                    Name = s.Name
                })
            };
            return data;
        }

        #region 私有方法

        #endregion
    }
}
