using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using Langben.DAL;
using System.ServiceModel;

namespace Langben.IBLL
{
    /// <summary>
    /// 客户_企业阶梯报价_待审核 接口
    /// </summary>

    public partial interface ICRM_CompanyLadderPrice_AuditBLL
    {
        /// <summary>
        /// 修改企业阶梯报价信息
        /// </summary>
        /// <param name="entity">企业阶梯报价信息</param>
        /// <returns></returns>
        bool ModifyLadderPrice(ref ValidationErrors validationErrors, CRM_CompanyLadderPrice_Audit entity);

        /// <summary>
        /// 获取阶梯报价审核信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        string GetCompanyLadderPrice_Audit(int ID);

        /// <summary>
        /// 退回阶梯报价信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        bool ReturnEdit(ValidationErrors validationErrors, int ID, int mainTableID);

        /// <summary>
        /// 提交阶梯报价信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        bool PassEdit(ValidationErrors validationErrors, int ID, int mainTableID);

        /// <summary>
        /// 退回阶梯报价信息添加审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <returns></returns>
        bool ReturnAdd(ValidationErrors validationErrors, int ID);

        /// <summary>
        /// 提交阶梯报价信息添加审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        bool PassAdd(ValidationErrors validationErrors, int ID);
    }
}

