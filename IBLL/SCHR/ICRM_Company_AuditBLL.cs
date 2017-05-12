using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using Langben.DAL;
using System.ServiceModel;
using Langben.DAL.Model;

namespace Langben.IBLL
{
    public partial interface ICRM_Company_AuditBLL
    {
        //提交企业基本信息修改到审核
        bool ModifyBaseInfo(ref Common.ValidationErrors validationErrors, CRM_Company_Audit entity);
        //待审核企业列表
        Common.ClientResult.DataResult GetAuditCompanyList(int page, int rows, string companyName, string operateStatus);

        /// <summary>
        /// 需重新提交的企业列表（销售用）
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="companyName">企业名称</param>
        /// <param name="userID_XS">销售人员ID</param>
        /// <param name="branchID">分支机构ID</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        List<CRM_Company_Audit> GetCompany_AuditListForReSubmit(int? id, int page, int rows, string companyName, int userID_XS, int branchID, ref int total);

        /// <summary>
        /// 企业基本信息待审核信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        string GetCompanyBaseAudit(int ID);

        /// <summary>
        /// 待审核企业列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="companyName"></param>
        /// <param name="UserID_ZR"></param>
        /// <param name="UserID_XS"></param>
        /// <param name="auditType"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        Common.ClientResult.DataResult GetAuditCompanyListForQuality(int page, int rows, string companyName, int? UserID_ZR, int? UserID_XS, int? auditType, int? operateStatus, string companyCode, int departmentScope, string departments, int branchID, int departmentID, int userID);


        /// <summary>
        /// 退回基本信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        bool ReturnBaseEdit(ValidationErrors validationErrors, int ID, int mainTableID);

        /// <summary>
        /// 提交企业基本信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        bool PassBaseEdit(ValidationErrors validationErrors, int ID, int mainTableID);
    }
}

