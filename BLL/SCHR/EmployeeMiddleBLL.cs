using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Langben.DAL;
using Common;
using Langben.DAL.Model;
using System.Linq.Expressions;

namespace Langben.BLL
{
    /// <summary>
    /// 员工费用中间表 
    /// </summary>
    public partial class EmployeeMiddleBLL : IBLL.IEmployeeMiddleBLL, IDisposable
    {
        /// <summary>
        /// 编辑一个员工费用中间表
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="entity">一个员工费用中间表</param>
        /// <returns></returns>
        public void Edit(SysEntities db, EmployeeMiddle entity)
        {
            repository.Edit(db, entity);
        }

        #region 报增保险。不更新数据库
        /// <summary>
        /// 报增保险。不更新数据库
        /// </summary>
        /// <param name="db">实体类</param>
        /// <param name="entity">初始值</param>
        /// <returns></returns>
        public bool CreateEmployee(SysEntities db, EmployeeMiddle entity)
        {

            repository.Create(db, entity);
            return true;

        }

        #endregion

        public List<EmployeeMiddleShow> GetData(int page, int rows, out int total, Expression<Func<EmployeeMiddleShow, bool>> where)
        {
            IQueryable<EmployeeMiddleShow> queryData = repository.GetData(db, where);
            total = queryData.Count();
            if (total > 0)
            {
                if (page <= 1)
                {
                    queryData = queryData.Take(rows);
                }
                else
                {
                    queryData = queryData.Skip((page - 1) * rows).Take(rows);
                }

            }
            return queryData.OrderBy(o => o.EmployeeId).ToList();
        }

        /// <summary>
        /// 获取费用中间表中数据
        /// </summary>
        /// 责任客服可查询：自己负责的企业
        /// 社保客服可查询：自己负责的险种和缴纳地
        /// 需要判断当前用户的角色，并将其结果取并集
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="total"></param>
        /// <param name="yearMonth">时间</param>
        /// <param name="companyId">公司编号</param>
        /// <param name="insuranceId">种类</param>
        /// <param name="certificate">身份证号</param>
        /// <param name="name">姓名</param>
        /// <returns></returns>
        public List<EmployeeMiddleShow> GetData(int page, int rows, out int total, int yearMonth, int companyId, int insuranceId, string certificate, string name,
            int departmentScope, string departments, int branchID, int departmentID, int userID)
        {
            // 获取公司列表（进行权限配置之后的）
            List<int> companyList = new List<int>();
            if (companyId != 0)
            {
                companyList.Add(companyId);
            }
            else
            {
                // 根据权限获取企业列表
                List<CRM_Company> company_list = repository.GetCompanyListByGroup(departmentScope, departments, branchID, departmentID, userID);
                foreach (var item in company_list)
                {
                    companyList.Add(item.ID);
                }
            }

            // 获取缴纳地列表（权限控制）
            List<string> cityList = new List<string>();
            List<City> city_list = repository.GetCityListByGroup(userID);
            foreach (var cityItem in city_list)
            {
                cityList.Add(cityItem.Id);
            }

            IQueryable<EmployeeMiddleShow> queryData = repository.GetData(db, yearMonth, companyList, cityList, insuranceId, certificate, name);
            total = queryData.Count();
            if (total > 0)
            {
                if (page <= 1)
                {
                    queryData = queryData.Take(rows);
                }
                else
                {
                    queryData = queryData.Skip((page - 1) * rows).Take(rows);
                }

            }
            return queryData.OrderBy(o => o.EmployeeId).ToList();
        }

        /// <summary>
        /// 修改费用中间表状态
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="id">费用中间表的主键</param>
        /// <param name="status">要修改成的状态</param>
        /// <param name="person">当前操作人</param>
        /// <returns></returns>  
        public bool UpdateEmployeeMiddleState(ref ValidationErrors validationErrors, int id, string status, string person)
        {
            try
            {
                return repository.UpdateEmployeeMiddleState(id, status, person) == 1;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
            }
            return false;
        }

        /// <summary>
        /// 根据用户组权限获取公司列表（需进行权限判断）
        /// </summary>
        /// 责任客服可查询：自己负责的企业
        /// 社保客服可查询：所有的企业
        /// <param name="departmentScope">部门业务权限</param>
        /// <param name="departments">部门范围权限</param>
        /// <param name="branchID">登录人机构ID</param>
        /// <param name="departmentID">登录人部门ID</param>
        /// <param name="userID">登录人ID</param>
        public List<CRM_Company> GetCompanyListByGroup(int departmentScope, string departments, int branchID, int departmentID, int userID)
        {
            return repository.GetCompanyListByGroup(departmentScope, departments, branchID, departmentID, userID);
        }

        /// <summary>
        /// 获取费用中间表中数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EmployeeMiddleShow GetDataByID(int id)
        {
            EmployeeMiddleShow Model = repository.GetDataByID(db, id);
            return Model;
        }
        /// <summary>
        /// 批量插入费用表
        /// </summary>
        /// <param name="employeeList"></param>
        /// <returns></returns>
        public int InsertList(List<EmployeeMiddle> employeeList)
        {
            return repository.InsertList(employeeList);
        }
    }
}

