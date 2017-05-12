using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using Langben.DAL;
using System.ServiceModel;
using Langben.DAL.Model;

namespace Langben.IBLL
{
    /// <summary>
    /// 费用_费用表 接口
    /// </summary>
    public partial interface ICOST_CostTableBLL
    {
        /// <summary>
        /// 查询的数据
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="dateTime">时间</param>
        /// <param name="companyId">企业ID</param>
        /// <param name="costTableType">费用表类型</param>
        /// <param name="status">状态</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        List<CostFeeModel> GetCostFeeList(int? id, int page, int rows, string dateTime, int companyId, int costTableType, int status,
            int departmentScope, string departments, int branchID, int departmentID, int userID, ref int total);

        /// <summary>
        /// 查询的数据
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="dateTime">时间</param>
        /// <param name="companyId">企业ID</param>
        /// <param name="costTableType">费用表类型</param>
        /// <param name="status">状态</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        List<CostFeeModel> GetCostFeeList(int? id, int page, int rows, string dateTime, List<int> companyId, int costTableType, int status, ref int total);
        List<CostFeeModel> GetAllCostFeeList(int page, int rows, string Search, ref int total);
        /// <summary>
        /// 查询的数据（财务确认费用表列表）
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="dateTime">时间</param>
        /// <param name="status">状态</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        List<CostFeeModel> GetCostFeeFinanceAduitList(int? id, int page, int rows, string dateTime, int status, ref int total);

        /// <summary>
        /// 获取费用明细表信息（包含五险一金、其他费用、服务费等）
        /// </summary>
        /// <param name="costId">费用表ID</param>
        /// <returns></returns>
        List<Cost_CostTableDetails> GetCostFeeDetailList(int costId);

        /// <summary>
        /// 获取费用表信息（包含企业名称）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CostFeeModel GetCostFeeModelById(int id);
        /// <summary>
        /// 费用表作废
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="id">费用表主键</param>
        /// <param name="status">要更新成的状态</param>
        /// <returns></returns>
        bool CancelCostTable(ref ValidationErrors validationErrors, int id, int status);
        /// <summary>
        /// 修改费用表状态
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="ids">费用_费用表的主键</param>
        /// <param name="status">要修改成的状态</param>
        /// <returns></returns>  
        bool UpdateCostTableStatus(ref ValidationErrors validationErrors, int id, int status);
        /// <summary>
        /// 批量修改费用表状态
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="ids">费用_费用表的主键</param>
        /// <param name="status">要修改成的状态</param>
        /// <returns></returns>  
        bool UpdateCostTableStatusCollection(ref ValidationErrors validationErrors, int[] ids, int status);

        /// <summary>
        /// 根据用户组权限获取公司列表（需进行权限判断,责任客服权限）
        /// </summary>
        /// <param name="departmentScope">部门业务权限</param>
        /// <param name="departments">部门范围权限</param>
        /// <param name="branchID">登录人机构ID</param>
        /// <param name="departmentID">登录人部门ID</param>
        /// <param name="userID">登录人ID</param>
        List<CRM_Company> GetCompanyList(int departmentScope, string departments, int branchID, int departmentID, int userID);


        bool Save(ref ValidationErrors validationErrors, COST_CostTable cOST_CostTable, int CRM_Company_ID, int yearMonth, int CreateUserID, string CreateUserName, int BranchID);

        [OperationContract]
        List<COST_CostTableService> Get_List_COST_CostTableService_Z(int count, decimal payService_One, int COST_CostTable_ID, int CRM_Company_ID, int CreateUserID, string CreateUserName, int BranchID, int[] Emploees);
        List<Cost_Cost_Company> GetCost_Cost_Company(int Cost_TableID);
        string GetSerialNumber(int yearMonth);

    }
}

