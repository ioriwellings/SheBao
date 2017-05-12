using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using Langben.DAL;
using System.ServiceModel;

namespace Langben.IBLL
{
    /// <summary>
    /// 员工调基 接口
    /// </summary>
    public partial interface IEmployeeGoonPayment2BLL
    {
        /// <summary>
        /// 正常服务中的员工列表
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="rows">行数</param>
        /// <param name="search">查询条件</param>
        /// <param name="departmentScope">数据范围权限</param>
        /// <param name="departments">数据部门权限</param>
        /// <param name="branchID">登录人所属分支机构ID</param>
        /// <param name="departmentID">登录人所属部门ID</param>
        /// <param name="userID">登录人ID</param>
        /// <param name="count">数据总条数</param>
        /// <returns></returns>
        List<EmployeeGoonPayment2View> GetEmployeeList(int page, int rows, string search, int departmentScope, string departments, int branchID, int departmentID, int userID, ref int count);

        /// <summary>
        /// 员工客服确认调基数据列表
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="rows">行数</param>
        /// <param name="search">查询条件</param>
        /// <param name="departmentScope">数据范围权限</param>
        /// <param name="departments">数据部门权限</param>
        /// <param name="branchID">登录人所属分支机构ID</param>
        /// <param name="departmentID">登录人所属部门ID</param>
        /// <param name="userID">登录人ID</param>
        /// <param name="count">数据总条数</param>
        /// <returns></returns>
        List<EmployeeGoonPayment2View> GetApproveList(int page, int rows, string search, int departmentScope, string departments, int branchID, int departmentID, int userID, ref int count);

        /// <summary>
        /// 调基数据状态变更
        /// </summary>
        /// <param name="ApprovedId">员工关系id</param>
        /// <param name="StateNew">新审核状态</param>
        /// <returns></returns>
        bool EmployeeGoonPayment2Approved(int?[] ApprovedId, string StateNew,string UserName);

        /// <summary>
        /// 员工客服终止调基
        /// </summary>
        /// <param name="ApprovedId">员工关系id</param>
        /// <param name="UserName">登录人信息</param>
        /// <returns></returns>
        bool EmployeeGoonPayment2End(int?[] ApprovedId, string UserName);

        //调基
        int ChangeWage(AllInsuranceKind entity, int? yanglaoID, int? yiliaoID, int? gongshangID, int? shiyeID, int? shengyuID, int? gongjijinID, string loginUser);

        // 获取需要调基的员工的信息
        EmployeeGoonPayment2View GetChangeWageEmployeeInfo(int companyEmployeeRelationId);
        //检查险种是否在处理中
        string Verification(AllInsuranceKind entity, int? yanglaoID, int? yiliaoID, int? gongshangID, int? shiyeID, int? shengyuID, int? gongjijinID);

        #region 信伟青
        /// <summary>
        /// 查询社保专员列表
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="search">查询条件</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        List<EmployeeGoonPayment2View> GetCommissionerListByParam(int? id, int page, int rows, string search, ref int total);

        #region 社保专员提取调基数据
        /// <summary>
        /// 社保专员提取调基数据
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <param name="search">查询条件</param>
        /// <returns></returns>      
        List<EmployeeAddView> GetEmployeeAddExcelList(int page, int rows, string search, ref int count);
        #endregion
        #endregion


        List<EmployeeAddView> GetEmployeeGoonPayment2List(int page, int rows, string search, ref int count);
    }
}

