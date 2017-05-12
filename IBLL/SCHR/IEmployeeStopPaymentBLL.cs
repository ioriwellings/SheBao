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
    /// 员工停缴 接口
    /// </summary>
    public partial interface IEmployeeStopPaymentBLL
    {
        /// <summary>
        /// 获取可单人报减员工信息
        /// </summary>
        /// <param name="zrUserId">责任客服ID</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="search">查询条件</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        [OperationContract]
        List<SingleStopPaymentView> GetSingleStopPaymentInfo(int zrUserId, int page, int rows, string search, ref int total);


        /// <summary>
        /// 责任客服审核平台数据列表
        /// </summary>
        /// <param name="zrUserId"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="search"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        [OperationContract]
        List<SingleStopPaymentView> GetListFromP(int zrUserId, int page, int rows, string search, ref int total);


        /// <summary>
        /// 员工客服审核平台数据列表
        /// </summary>
        /// <param name="zrUserId"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="search"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        [OperationContract]
        List<SingleStopPaymentView> GetEmployeeStopForCustomerList(int zrUserId, int page, int rows, string search, ref int total,int yguserid);

        /// <summary>
        /// 责任客服操作平台数据，通过还是退回
        /// </summary>
        /// <param name="kindsIDS"></param>
        /// <param name="IsPass"></param>
        /// <returns></returns>
        [OperationContract]
        bool PassStopPaymentRepository(string kindsIDS, int IsPass);


        /// 员工客服操作平台数据，通过还是退回
        /// </summary>
        /// <param name="kindsIDS"></param>
        /// <param name="IsPass"></param>
        /// <returns></returns>
        [OperationContract]
        bool PassStopPaymentRepositoryForCustomer(string kindsIDS, int IsPass);
        bool PassStopPaymentRepositoryForCustomer(string kindsIDS, int IsPass,string remark);

        /// <summary>
        /// 获取停缴时员工的信息
        /// </summary>
        /// <param name="companyEmployeeRelationId">企业员工关系表ID</param>
        /// <returns></returns>
        [OperationContract]
        StopPaymentEmployeeInfo GetStopPaymentEmployeeInfo(int companyEmployeeRelationId);

        /// <summary>
        /// 获取停缴时员工的信息
        /// </summary>
        /// <param name="companyEmployeeRelationId">企业员工关系表ID</param>
        /// <returns></returns>
        [OperationContract]
        StopPaymentEmployeeInfo GetStopPaymentEmployeeInfoForStop(int companyEmployeeRelationId, string State);

        /// <summary>
        /// 保存单人报减信息并修改员工费用中间表的相关信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [OperationContract]
        bool InsertStopPayment(List<EmployeeStopPaymentSingle> list);

        /// <summary>
        /// 通过企业ID获取企业名称
        /// </summary>
        /// <param name="companyId">企业ID</param>
        /// <returns>返回企业名称，如果没有查到返回""</returns>
        [OperationContract]
        string GetCompanyNameById(int companyId);

        /// <summary>
        /// 通过企业名称获取企业ID
        /// </summary>
        /// <param name="companyName">企业名称（全字符匹配）</param>
        /// <returns>返回企业ID，如果没有查到返回0</returns>
        [OperationContract]
        int GetCompanyIdByName(string companyName);

        /// <summary>
        /// 根据责任客服Id和企业名称获取企业ID.如果该企业存在并且是该责任客服负责返回企业ID，如果企业存在但不是该责任客服负责返回-1，如果企业名称不存在返回0
        /// </summary>
        /// <param name="companyName">企业名称（全字符匹配）</param>
        /// <param name="userZRId">责任客服Id</param>
        /// <returns>如果该企业存在并且是该责任客服负责反悔企业ID，如果企业存在但不是该责任客服负责反悔-1，如果企业名称不存在返回0</returns>
        [OperationContract]
        int GetCompanyIdByNameAndUserZRId(string companyName, int userZRId);

        /// <summary>
        /// 通过员工姓名和身份证号获取员工ID
        /// </summary>
        /// <param name="employeeName">员工姓名</param>
        /// <param name="cardId">身份证号</param>
        /// <returns>返回员工ID，如果没有查到返回0</returns>
        [OperationContract]
        int GetEmployeeIdByNameAndCardId(string employeeName, string cardId);

        /// <summary>
        /// 通过企业ID，员工ID获取企业员工关系表ID
        /// </summary>
        /// <param name="companyId">企业ID</param>
        /// <param name="employeeId">员工ID</param>
        /// <returns>返回企业员工关系表ID，如果没有查到返回0</returns>
        [OperationContract]
        int GetCompanyEmployeeRelationId(int companyId, int employeeId);

        /// <summary>
        /// 通过险种和企业员工关系表ID判断
        /// </summary>
        /// <param name="kind">险种</param>
        /// <param name="companyEmployeeRelationId">企业员工关系表ID</param>
        /// <returns>返回状态是报增成功的报增记录ID，如果没查到返回0</returns>
        [OperationContract]
        int GetEmployeeAddIdByKindIdAndRelationId(EmployeeAdd_InsuranceKindId kind, int companyEmployeeRelationId);

        /// <summary>
        /// 通过增员表ID和手续名称获取手续ID
        /// </summary>
        /// <param name="employeeAddId">增员表ID</param>
        /// <param name="operationName">手续名称</param>
        /// <returns>返回手续ID，如果没查到返回0</returns>
        [OperationContract]
        int GetPoliceOperationIdByEmployeeAddIdAndOperationName(int employeeAddId, string operationName);

        /// <summary>
        /// 通过增员表ID获取政策
        /// </summary>
        /// <param name="addId">增员表ID</param>
        /// <returns></returns>
        [OperationContract]
        PoliceInsurance GetPoliceInsuranceByEmployeeAddId(int addId);

        /// <summary>
        /// 通过增员表ID 判断是否有正在报减的数据
        /// </summary>
        /// <param name="addId">增员表ID</param>
        /// <returns></returns>
        [OperationContract]
        bool IsHaveStopingByEmployeeAddId(int addId);

        /// <summary>
        /// 社保报减查询
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <param name="search"></param>
        /// <param name="userList"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        List<EmployeeStopView> GetEmployeeStopList(int page, int rows, string search,List<ORG_User> userList, ref int count);

        /// <summary>
        /// 供应商客服提取报减数据
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="search">查询条件</param>
        /// <returns></returns>      
        List<SupplierAddView> GetEmployeeStopExcelListBySupplier(string search);

        /// <summary>
        /// 社保报减查询-无分页（导出excel时用）
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <param name="userList">员工列表</param>
        /// <returns></returns>
        List<EmployeeStopView> GetEmployeeStopListForExcel(string search, List<ORG_User> userList);

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
        List<EmployeeApprove> GetApproveList(int? id, int page, int rows, string search, ref int total);
        #endregion

        /// <summary>
        /// 人员报减赚头修改
        /// </summary>
        /// <param name="validationErrors"></param>
        /// <param name="ApprovedId">审核人员主键</param>
        /// <param name="state">审核状态</param>
        /// <returns></returns>
        bool EmployeeStopPaymentApproved(ref ValidationErrors validationErrors, int[] ApprovedId, string StateOld, string StateNew, string message, string UpdatePerson);
        /// <summary>
        /// 获取员工客服报减信息
        /// </summary>
        /// <param name="zrUserId">责任客服ID</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="search">查询条件</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        List<SingleStopPaymentViewDuty> GetEmployeeStopForDutyList(int zrUserId, int page, int rows, string search, ref int total);

        /// <summary>
        /// 报减政策手续
        /// </summary>
        /// <param name="id">社保类型id</param>
        /// <returns></returns>
        List<idname__> getPoliceOperationid(int id);

        /// <summary>
        /// 修改停缴手续
        /// </summary>
        /// <param name="lstStopPayments"></param>
        /// <returns></returns>
        bool EditStopPaymentOperation(List<EmployeeStopPayment> lstStopPayments);

        /// <summary>
        /// 设置报减成功
        /// </summary>
        /// <param name="stopIds">报减表ID List</param>
        /// <returns></returns>
        bool SetStopPaymentSuccess(string stopIds);

        /// <summary>
        /// 设置报减失败
        /// </summary>
        /// <param name="stopIds">报减表ID list</param>
        /// <param name="remark">失败原因</param>
        /// <returns></returns>
        bool SetStopPaymentFail(string stopIds, string remark);

        /// <summary>
        /// 模板设置报减失败
        /// </summary>
        /// <param name="dt">Excel数据</param>
        /// <param name="userID">操作人ID</param>
        /// <param name="userName">操作人姓名</param>
        /// <returns></returns>
        string SetStopPaymentFailByExcel(DataTable dt, int userID, string userName);

        /// <summary>
        /// 供应商根据报减编号及险种设置报减失败
        /// </summary>
        /// <param name="stopIds">报减编号</param>
        /// <param name="remark">失败原因</param>
        /// <param name="insuranceKindTypes">报减类型</param>
        /// <returns></returns>
        bool SetStopPaymentFailByInsuranceKind(string stopIds, string remark, List<int?> insuranceKindTypes);
    }
}

