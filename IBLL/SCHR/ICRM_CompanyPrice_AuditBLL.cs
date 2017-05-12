using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using Langben.DAL;
using System.ServiceModel;

namespace Langben.IBLL
{
    /// <summary>
    /// 客户_企业报价_待审批 接口
    /// </summary>
    public partial interface ICRM_CompanyPrice_AuditBLL
    {
        //提交企业报价信息修改 到审核
        bool ModifyPrice(ref Common.ValidationErrors validationErrors, CRM_CompanyPrice_Audit entity);

        /// <summary>
        /// 获取报价审核信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        string GetCompanyPrice_Audit(int ID);

        /// <summary>
        /// 退回报价信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        bool ReturnEdit(ValidationErrors validationErrors, int ID, int mainTableID);

        /// <summary>
        /// 提交报价信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        bool PassEdit(ValidationErrors validationErrors, int ID, int mainTableID);

        /// <summary>
        /// 退回报价信息添加审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <returns></returns>
        bool ReturnAdd(ValidationErrors validationErrors, int ID);

        /// <summary>
        /// 提交报价信息添加审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        bool PassAdd(ValidationErrors validationErrors, int ID);

        /// <summary>
        /// 添加企业报价信息到审核表
        /// </summary>
        /// <param name="listPrice">报价信息</param>
        /// <param name="listLadderPrice">阶梯报价</param>
        /// <returns></returns>
        bool CreatePrice_Audit(List<CRM_CompanyPrice_Audit> listPrice, List<CRM_CompanyLadderPrice_Audit> listLadderPrice);
    }
}

