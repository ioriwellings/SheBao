using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using Langben.DAL;
using System.ServiceModel;
using Langben.DAL.Model;
using System.Data;

namespace Langben.IBLL
{
    /// <summary>
    /// 费用_社保支出导入汇总 接口
    /// </summary>
    public partial interface ICOST_PayRecordStatusBLL
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
        List<COST_PayRecordConfirm> GetPayRecordList(int? id, int page, int rows, string dateTime, int costType, int personId, string cityId,
            string groupCode, int departmentScope, string departments, int branchID, int departmentID, int userID, ref int total);

        /// <summary>
        /// 查询的数据（支出费用汇总，“加入对比”列表的数据源)
        /// </summary>
        /// <param name="yearMonth">年月</param>
        /// <param name="costType">险种</param>
        /// <param name="cityId">缴纳地</param>
        /// <returns>结果集</returns>
        List<COST_PayRecordSummary> GetPayRecordContrastedList(string yearMonth, int costType, string cityId);

        /// <summary>
        /// 删除支出费用
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="id">费用_社保支出导入汇总 主键</param>
        /// <returns></returns>
        bool DeletePayRecord(ref ValidationErrors validationErrors, int id);

        /// <summary>
        /// 修改支出费用表状态
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="id">支出表的主键</param>
        /// <param name="status">要修改成的状态</param>
        /// <returns></returns>  
        bool UpdatePayRecordStatus(ref ValidationErrors validationErrors, int id, int status);

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
        bool ContrastedInsurance(ref ValidationErrors validationErrors, int yearMonth, int costType, string cityId, string userName);

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
        List<CostPayPersonContrasted> GetPayPersonContrastedList(int? id, int page, int rows, int yearMonthStart, int yearMonthEnd, int costType, int companyId,
            string cityId, string certificate, string employeeName, int departmentScope, string departments, int branchID, int departmentID, int userID, ref int total);

        /// <summary>
        /// 根据用户组权限获取公司列表（需进行权限判断）
        /// </summary>
        /// 责任客服可查询：自己负责的企业
        /// 员工客服可查询：自己负责的企业
        /// 社保客服可查询：所有企业
        /// <param name="departmentScope">部门业务权限</param>
        /// <param name="departments">部门范围权限</param>
        /// <param name="branchID">登录人机构ID</param>
        /// <param name="departmentID">登录人部门ID</param>
        /// <param name="userID">登录人ID</param>
        List<CRM_Company> GetCompanyListByGroup(int departmentScope, string departments, int branchID, int departmentID, int userID);

        /// <summary>
        /// 根据用户组权限获取险种列表（需进行权限判断）
        /// </summary>
        /// <param name="userID">登录人ID</param>
        /// <returns></returns>
        List<EnumsCommon.EnumsListModel> GetCostTypeByGroup(int userID);

        /// <summary>
        /// 根据用户组权限获取缴纳地列表（需进行权限判断）
        /// </summary>
        /// <param name="userID">登录人ID</param>
        /// <param name="costType">险种</param>
        /// <returns></returns>
        List<City> GetCityListByGroup(int userID, int costType);

        /// <summary>
        /// 获取社保客服负责的险种及缴纳地信息
        /// </summary>
        /// <param name="userID">登录人ID</param>
        /// <returns></returns>
        List<InsuranceKind> GetSBKFInsuranceKindByUser(int userID);

        /// <summary>
        /// 根据缴纳地获取社保客服负责的险种
        /// </summary>
        /// <param name="userID">登录人ID</param>
        /// <returns></returns>
        List<Langben.DAL.Model.CostType> GetSBKFCostTypeByCity(int userID, string cityId);

        /// <summary>
        /// 根据用户编号获取所负责的缴纳地(社保客服)
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        List<City> GetSBKFCityListByUser(int userID);

        /// <summary>
        /// 获取用户组列表（有权限控制）
        /// </summary>
        List<ORG_User> GetPersonListByGroupCode(string groupCode, int departmentScope, string departments, int branchID, int departmentID, int userID);

        /// <summary>
        /// 根据用户编号获取员工客服所负责的缴纳地
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        List<City> GetYGKFCityListByUser(int userID);
        List<CostPayFenGe> GetCostPayFenGeList(int Kinds, int QIJIAN, int? CompanyId, bool Page, int PageSize, int CurPage, out int Tatal_Count);
        string ImportExcelForGYS(DataTable dt, int CostTable_CreateFrom, int YearMouth, int Suppler_ID, int UserID, int BranchID, string UserName);
    }
}

