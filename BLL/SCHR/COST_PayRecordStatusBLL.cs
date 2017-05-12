using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Langben.DAL;
using Common;
using Langben.DAL.Model;
using System.Data;
namespace Langben.BLL
{
    /// <summary>
    /// 费用_社保支出导入汇总
    /// </summary>
    public partial class COST_PayRecordStatusBLL
    {
        /// <summary>
        /// 查询的数据
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="dateTime">时间</param>
        /// <param name="costType">险种</param>
        /// <param name="personId">客服ID</param>
        /// <param name="currentUser">当前用户</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        public List<COST_PayRecordConfirm> GetPayRecordList(int? id, int page, int rows, string dateTime, int costType, int personId, string cityId,
            string groupCode, int departmentScope, string departments, int branchID, int departmentID, int userID, ref int total)
        {
            // 获取社保客服员工列表（全部的）
            List<int?> personList = new List<int?>();
            if (personId != 0)
            {
                personList.Add(personId);
            }
            else
            {
                // 根据权限获取社保客服列表
                List<ORG_User> person_list = repository.GetPersonListByGroupCode(groupCode, departmentScope, departments, branchID, departmentID, userID);
                foreach (var item in person_list)
                {
                    personList.Add(item.ID);
                }
            }

            IQueryable<COST_PayRecordConfirm> queryData = repository.GetPayRecordStatusList(db, dateTime, costType, personList, cityId, userID);
            List<COST_PayRecordConfirm> queryList = new List<COST_PayRecordConfirm>();
            total = queryData.Count();
            if (total > 0)
            {
                if (page <= 1)
                {
                    queryList = queryData.Take(rows).ToList();
                }
                else
                {
                    queryList = queryData.Skip((page - 1) * rows).Take(rows).ToList();
                }
            }
            return queryList;
        }

        /// <summary>
        /// 查询的数据(支出费用汇总，“加入对比”列表的数据源)
        /// </summary>
        /// <param name="yearMonth">年月</param>
        /// <param name="costType">险种</param>
        /// <param name="cityId">缴纳地</param>
        /// <returns>结果集</returns>
        public List<COST_PayRecordSummary> GetPayRecordContrastedList(string yearMonth, int costType, string cityId)
        {
            List<COST_PayRecordSummary> queryList = repository.GetPayRecordContrastedList(db, yearMonth, costType, cityId);

            return queryList;
        }

        /// <summary>
        /// 删除支出费用
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="id">费用_社保支出导入汇总 主键</param>
        /// <returns></returns>
        public bool DeletePayRecord(ref ValidationErrors validationErrors, int id)
        {
            using (TransactionScope transactionScope = new TransactionScope())
            {
                try
                {
                    repository.DeletePayRecord(db, id);
                    db.SaveChanges();
                    transactionScope.Complete();
                    return true;
                }
                catch (Exception ex)
                {
                    Transaction.Current.Rollback();
                    validationErrors.Add(ex.Message);
                    ExceptionsHander.WriteExceptions(ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// 修改支出费用表状态
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="id">支出表的主键</param>
        /// <param name="status">要修改成的状态</param>
        /// <returns></returns>  
        public bool UpdatePayRecordStatus(ref ValidationErrors validationErrors, int id, int status)
        {
            try
            {
                return repository.UpdatePayRecordStatus(id, status) == 1;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
            }
            return false;
        }

        /// <summary>
        /// 社保费收支费用对比
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="yearMonth">年月</param>
        /// <param name="costType">费用类型</param>
        /// <param name="suppliersId">供应商</param>
        /// <param name="cityId">缴纳地</param>
        /// <param name="userName">当前操作用户</param>
        /// <returns></returns>
        public bool ContrastedInsurance(ref ValidationErrors validationErrors, int yearMonth, int costType, string cityId, string userName)
        {
            using (TransactionScope transactionScope = new TransactionScope())
            {
                try
                {
                    repository.ContrastedInsurance(db, yearMonth, cityId, costType, userName);
                    db.SaveChanges();
                    transactionScope.Complete();
                    return true;
                }
                catch (Exception ex)
                {
                    Transaction.Current.Rollback();
                    validationErrors.Add(ex.Message);
                    ExceptionsHander.WriteExceptions(ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// 查询的数据(员工收支对比列表)
        /// </summary>
        /// 责任客服可查询：自己负责的企业
        /// 员工客服可查询：自己负责的企业，负责的缴纳地
        /// 社保客服可查询：自己负责的险种和缴纳地
        /// 需要判断当前用户的角色，并将其结果取并集
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="yearMonthStart">起始年月</param>
        /// <param name="yearMonthEnd">结束年月</param>
        /// <param name="costType">险种</param>
        /// <param name="companyId">公司ID</param>
        /// <param name="cityId">缴纳地</param>
        /// <param name="certificate">身份证号（支持多行）</param>
        /// <param name="employeeName">员工姓名</param>
        /// <param name="total">结果集的总数</param>
        /// <returns></returns>      
        public List<CostPayPersonContrasted> GetPayPersonContrastedList(int? id, int page, int rows, int yearMonthStart, int yearMonthEnd, int costType, int companyId,
            string cityId, string certificate, string employeeName, int departmentScope, string departments, int branchID, int departmentID, int userID, ref int total)
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
            if (!string.IsNullOrEmpty(cityId))
            {
                cityList.Add(cityId);
            }
            else
            {
                List<City> city_list = repository.GetCityListByGroup(userID, costType);
                foreach (var cityItem in city_list)
                {
                    cityList.Add(cityItem.Id);
                }
            }

            List<CostPayPersonContrasted> queryList = repository.GetPayPersonContrastedList(db, yearMonthStart, yearMonthEnd, costType, companyList, cityList, certificate, employeeName);
            total = queryList.Count();
            if (total > 0)
            {
                if (page <= 1)
                {
                    queryList = queryList.Take(rows).ToList();
                }
                else
                {
                    queryList = queryList.Skip((page - 1) * rows).Take(rows).ToList();
                }
            }
            return queryList;
        }

        /// <summary>
        /// 根据用户组权限获取公司列表（需进行权限判断）
        /// </summary>
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
        /// 根据用户组权限获取险种列表（需进行权限判断）
        /// </summary>
        /// <param name="userID">登录人ID</param>
        /// <returns></returns>
        public List<EnumsCommon.EnumsListModel> GetCostTypeByGroup(int userID)
        {
            return repository.GetCostTypeByGroup(userID);
        }

        /// <summary>
        /// 根据用户组权限获取缴纳地列表（需进行权限判断）
        /// </summary>
        /// <param name="userID">登录人ID</param>
        /// <param name="costType">险种</param>
        /// <returns></returns>
        public List<City> GetCityListByGroup(int userID, int costType)
        {
            return repository.GetCityListByGroup(userID, costType);
        }

        /// <summary>
        /// 根据员工ID获取社保客服负责的险种及缴纳地
        /// </summary>
        /// <param name="userID">登录人ID</param>
        /// <returns></returns>
        public List<InsuranceKind> GetSBKFInsuranceKindByUser(int userID)
        {
            return repository.GetSBKFInsuranceKindByUser(userID);
        }

        /// <summary>
        /// 根据缴纳地获取社保客服负责的险种
        /// </summary>
        /// <param name="userID">登录人ID</param>
        /// <returns></returns>
        public List<Langben.DAL.Model.CostType> GetSBKFCostTypeByCity(int userID, string cityId)
        {
            return repository.GetSBKFCostTypeByCity(userID, cityId);
        }

        /// <summary>
        /// 根据用户编号获取所负责的缴纳地(社保客服)
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<City> GetSBKFCityListByUser(int userID)
        {
            return repository.GetSBKFCityListByUser(userID);
        }

        /// <summary>
        /// 获取用户组列表（有权限控制）
        /// </summary>
        public List<ORG_User> GetPersonListByGroupCode(string groupCode, int departmentScope, string departments, int branchID, int departmentID, int userID)
        {
            return repository.GetPersonListByGroupCode(groupCode, departmentScope, departments, branchID, departmentID, userID);
        }

        /// <summary>
        /// 根据用户编号获取员工客服所负责的缴纳地
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<City> GetYGKFCityListByUser(int userID)
        {
            return repository.GetYGKFCityListByUser(userID);
        }
        public List<CostPayFenGe> GetCostPayFenGeList(int Kinds, int QIJIAN, int? CompanyId, bool Page, int PageSize, int CurPage, out int Tatal_Count)
        {
            return repository.GetCostPayFenGeList(Kinds, QIJIAN, CompanyId, Page, PageSize, CurPage, out Tatal_Count);
        }
        public string ImportExcelForGYS(DataTable dt, int CostTable_CreateFrom, int YearMouth, int Suppler_ID, int UserID, int BranchID, string UserName)
        {
             return repository. ImportExcelForGYS( dt,  CostTable_CreateFrom,  YearMouth,  Suppler_ID,  UserID,  BranchID,  UserName);
        }
    }
}


