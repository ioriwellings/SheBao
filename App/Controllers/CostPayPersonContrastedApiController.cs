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
    /// 员工收支对比
    /// </summary>
    public class CostPayPersonContrastedApiController : BaseApiController
    {
        ValidationErrors validationErrors = new ValidationErrors();
        IBLL.ICOST_PayRecordStatusBLL m_BLL = new BLL.COST_PayRecordStatusBLL();

        string menuId = "1048";  // "员工社保收支对比"菜单编码
        //string groupUser_SBKF = "SBKF";  // 用户组中“社保客服”的编码
        //string groupUser_ZRKF = "ZRKF";  // 用户组中“责任客服”的编码
        //string groupUser_YGKF = "YGKF";  // 用户组中“员工客服”的编码
        
        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam) 
        {
            int total = 0;

            // 设置搜索默认值
            int yearMonthStart = Convert.ToInt32(DateTime.Now.ToString("yyyyMM"));
            int yearMonthEnd = yearMonthStart;
            int costType = -1;  // 险种默认什么都不选择
            int companyId = 0;
            string certificate = "";
            string employeeName = "";

            string cityID = "";

            // 各搜索项赋值
            if (!string.IsNullOrEmpty(getParam.search))
            {
                string[] search = getParam.search.Split('^');
                yearMonthStart = Convert.ToInt32(search[0].Split('&')[1]);
                yearMonthEnd = Convert.ToInt32(search[1].Split('&')[1]);
                costType = search[2].Split('&')[1] != "" ? Convert.ToInt32(search[2].Split('&')[1]) : 1;
                if (search[3].Split('&')[1] != "" && search[3].Split('&')[1] != "null" && search[3].Split('&')[1] != "0")
                {
                    companyId = Convert.ToInt32(search[3].Split('&')[1]);
                }
                certificate = search[4].Split('&')[1];
                employeeName = search[5].Split('&')[1];
                cityID = search[6].Split('&')[1];
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
            List<CostPayPersonContrasted> queryData = m_BLL.GetPayPersonContrastedList(getParam.id, getParam.page, getParam.rows, yearMonthStart, yearMonthEnd, costType, 
                companyId, cityID, certificate, employeeName, departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData.Select(s => new
                {
                    CompanyCode = s.CompanyCode,
                    CompanyName = s.CompanyName,
                    CityName = s.CityName,
                    Certificate = s.Certificate,
                    EmployeeName = s.EmployeeName,
                    PayCompanyCost = s.PayCompanyCost,
                    PayPersonCost = s.PayPersonCost,
                    CostCompanyCost = s.CostCompanyCost,
                    CostPersonCost = s.CostPersonCost
                })
            };
            return data;
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

        /// <summary>
        /// 获取险种
        /// </summary>
        /// <returns></returns>
        public Common.ClientResult.DataResult GetCostType()
        {
            var query = m_BLL.GetCostTypeByGroup(LoginInfo.UserID);
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

        public Common.ClientResult.DataResult GetCity(int costType)
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

            var query = m_BLL.GetCityListByGroup(LoginInfo.UserID, costType);
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
    }
}
