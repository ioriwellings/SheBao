using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Langben.DAL;
using Common;
using Langben.DAL.Model;

namespace Langben.BLL
{
    /// <summary>
    /// 员工调基 
    /// </summary>
    public partial class EmployeeGoonPayment2BLL : IBLL.IEmployeeGoonPayment2BLL, IDisposable
    {
        #region 正常服务中的员工列表
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
        public List<EmployeeGoonPayment2View> GetEmployeeList(int page, int rows, string search, int departmentScope, string departments, int branchID, int departmentID, int userID, ref int count)
        {
            return repository.GetEmployeeList(db, page, rows, search, departmentScope, departments, branchID, departmentID, userID, ref count);
        }
        #endregion

        #region 员工客服确认调基数据列表
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
        public List<EmployeeGoonPayment2View> GetApproveList(int page, int rows, string search, int departmentScope, string departments, int branchID, int departmentID, int userID, ref int count)
        {
            return repository.GetApproveList(db, page, rows, search, departmentScope, departments, branchID, departmentID, userID, ref count);
        }
        #endregion

        #region 调基数据状态变更
        /// <summary>
        /// 调基数据状态变更
        /// </summary>
        /// <param name="ApprovedId">员工关系id</param>
        /// <param name="StateNew">新审核状态</param>
        /// <returns></returns>
        public bool EmployeeGoonPayment2Approved(int?[] ApprovedId, string StateNew, string UserName)
        {
            return repository.EmployeeGoonPayment2Approved(db, ApprovedId, StateNew, UserName);
        }
        #endregion

        #region 员工客服终止调基
        /// <summary>
        /// 员工客服终止调基
        /// </summary>
        /// <param name="ApprovedId">员工关系id</param>
        /// <param name="UserName">登录人信息</param>
        /// <returns></returns>
        public bool EmployeeGoonPayment2End(int?[] ApprovedId, string UserName)
        {
            return repository.EmployeeGoonPayment2End(db, ApprovedId, UserName);
        }
        #endregion

        #region 王骁健
        /// <summary>
        /// 获取需要调基的员工的信息
        /// </summary>
        /// <param name="companyEmployeeRelationId"></param>
        /// <returns></returns>
        public EmployeeGoonPayment2View GetChangeWageEmployeeInfo(int companyEmployeeRelationId)
        {
            return repository.GetChangeWageEmployeeInfo(db, companyEmployeeRelationId);
        }
        /// <summary>
        /// 员工调基
        /// </summary>
        /// <param name="yanglao"></param>
        /// <param name="yiliao"></param>
        /// <param name="gongshang"></param>
        /// <param name="shiye"></param>
        /// <param name="shengyu"></param>
        /// <param name="gongjijinID"></param>
        /// <returns></returns>
        public int ChangeWage(AllInsuranceKind entity, int? yanglaoID, int? yiliaoID, int? gongshangID, int? shiyeID, int? shengyuID, int? gongjijinID, string loginUser)
        {
            return repository.ChangeWage(db, entity, yanglaoID, yiliaoID, gongshangID, shiyeID, shengyuID, gongjijinID, loginUser);
        }
        /// <summary>
        /// 检查是否有险种正在调基处理中
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="yanglaoID"></param>
        /// <param name="yiliaoID"></param>
        /// <param name="gongshangID"></param>
        /// <param name="shiyeID"></param>
        /// <param name="shengyuID"></param>
        /// <param name="gongjijinID"></param>
        /// <returns></returns>
        public string Verification(AllInsuranceKind entity, int? yanglaoID, int? yiliaoID, int? gongshangID, int? shiyeID, int? shengyuID, int? gongjijinID)
        {
            return repository.Verification(db, entity, yanglaoID, yiliaoID, gongshangID, shiyeID, shengyuID, gongjijinID);
        }
        #endregion

        #region 信伟青
        #region 查询社保专员列表
        /// <summary>
        /// 查询社保专员列表
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="search">查询条件</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        public List<EmployeeGoonPayment2View> GetCommissionerListByParam(int? id, int page, int rows, string search, ref int total)
        {
            string CompanyName = string.Empty;
            string Name = string.Empty;
            string CertificateNumber = string.Empty;
            Dictionary<string, string> queryDic = ValueConvert.StringToDictionary(search.GetString());
            if (queryDic != null && queryDic.Count > 0)
            {
                foreach (var item in queryDic)
                {
                    if (item.Key == "CertificateNumber")
                    {//查询一对多关系的列名
                        CertificateNumber = item.Value;
                        continue;
                    }
                }
            }
            List<EmployeeGoonPayment2View> queryData = repository.GetCommissionerList(db, CertificateNumber, page, rows, search, out total);
            return queryData;
        }
        #endregion

        #region 社保专员提取调基数据
        /// <summary>
        /// 社保专员提取调基数据
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>        
        /// <param name="search">查询条件</param>
        /// <returns></returns>      
        public List<EmployeeAddView> GetEmployeeAddExcelList(int page, int rows, string search, ref int count)
        {
            return repository.GetEmployeeAddExcelList(db, page, rows, search, ref count);
        }
        #endregion

        #endregion

        #region 社保报增查询
        /// <summary>
        /// 社保报增查询
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <returns></returns>      
        public List<EmployeeAddView> GetEmployeeGoonPayment2List(int page, int rows, string search, ref int total)
        {
            return repository.GetEmployeeGoonPayment2List(db, page, rows, search, ref total);
        }
        #endregion
    }
}

