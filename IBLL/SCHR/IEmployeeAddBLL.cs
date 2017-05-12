using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using Langben.DAL;
using System.ServiceModel;
using System.Data;
using Langben.DAL.Model;
namespace Langben.IBLL
{
    /// <summary>
    /// 增加员工 接口
    /// </summary>
    public partial interface IEmployeeAddBLL
    {


        /// <summary>
        /// 查询待责任客服审核列表
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="search">查询条件</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        [OperationContract]
        List<EmployeeApprove> GetApproveListByParam(int? id, int page, int rows, string search, ref int total);
        /// <summary>
        /// 查询待责任客服审核列表
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="search">查询条件</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        [OperationContract]
        List<EmployeeApprove> GetApproveListByParam(int? id, string search);
        /// <summary>
        /// 人员报增审核
        /// </summary>
        /// <param name="validationErrors"></param>
        /// <param name="ApprovedId">审核人员主键</param>
        /// <param name="state">审核状态</param>
        /// <returns></returns>
        bool EmployeeAddApproved(ref ValidationErrors validationErrors, int?[] ApprovedId, string StateOld, string StateNew);

        /// <summary>
        /// 查询待客服经理分配列表
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <returns></returns>     
        [OperationContract]
        List<EmployeeAllot> GetAllotList(int page, int rows, string search, ref int count);
        #region 缴纳地户口性质
        /// <summary>
        /// 缴纳地户口性质 敬
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<idname__> getPoliceAccountNatureid(string id);
        #endregion
        #region 社保种类
        List<idname__> getInsuranceKindid(string id);
        #endregion
        #region 政策手续
        List<idname__> getPoliceOperationid(int id);
        #endregion
        //#region 报增保险 不保存数据库
        ///// <summary>
        ///// 报增保险 不保存数据库
        //[OperationContract]
        //bool CreateEmployee(SysEntities entities, EmployeeAdd entity);
        //#endregion

        List<EmployeeAddView> GetEmployeeAddList(int page, int rows, string search, List<ORG_User> userList, ref int count);

        #region 社保单人报增身份证号多行查询
        /// <summary>
        /// 社保单人报增身份证号多行查询
        /// </summary>
        /// <param name="zrUserId">当前登录人</param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="search"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        [OperationContract]
        List<Employee> GetEmployeeList(int zrUserId, int page, int rows, string search, ref int total);
        #endregion

        #region 企业初始绑定
        List<idname__> getCompanyList();
        #endregion
        #region 缴纳地初始绑定
        List<idname__> getCitylist();
        #endregion

        #region
        /// <summary>
        /// 查询社保专员列表
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="search">查询条件</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        [OperationContract]
        List<EmployeeApprove> GetCommissionerListByParam(int? id, int page, int rows, string search, ref int total);
        #endregion

        List<EmployeeAddView> GetEmployeeAddExcelList(int page, int rows, string search, ref int count);
        List<EmployeeAddView> GetEmployeeAddListForExcel(string search, List<ORG_User> userList, ref int total);


        List<EmployeeAddCheckModel> EmployeeAddCkeck(string City, int PoliceAccountNatureId, int? PoliceOperationId, int? PoliceInsuranceId, string InsuranceKind);

        List<EmployeeAddView> GetEmployeeAddExcelList1(int page, int rows, string search, ref int count);
        #region 社保失败
        string ChangeServicecharge(EmployeeAdd item, string message, string LoginName);
        #endregion

        /// <summary>
        /// 模板设置报增失败
        /// </summary>
        /// <param name="dt">Excel数据</param>
        /// <param name="userID">操作人ID</param>
        /// <param name="userName">操作人姓名</param>
        /// <returns></returns>
        string SetAddFailByExcel(DataTable dt, int userID, string userName);

        /// <summary>
        /// 供应商客服提取报增信息
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <returns></returns>      
        List<SupplierAddView> GetEmployeeAddExcelListBySupplier(string search);

        /// <summary>
        /// 修改报增数据状态
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ids">要修改数据的ID数组</param>
        /// <param name="oldStatus">原状态(可为空字符)</param>
        /// <param name="newStatus">新状态</param>
        /// <param name="name">修改人</param>
        /// <returns></returns>
        bool ChangeStatus(int[] ids, string oldStatus, string newStatus, string name);
    }
}

