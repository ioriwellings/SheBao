using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using Langben.DAL;
using System.ServiceModel;

namespace Langben.IBLL
{
    /// <summary>
    /// 客户_企业合同信息_待审核 接口
    /// </summary>
    public partial interface ICRM_CompanyContract_AuditBLL
    {

        //提交企业合同信息修改 到审核
        bool ModifyContract(ref Common.ValidationErrors validationErrors, CRM_CompanyContract_Audit entity);

        /// <summary>
        /// 退回合同信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        bool ReturnEdit(ValidationErrors validationErrors, int ID, int mainTableID);

        /// <summary>
        /// 提交合同信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        bool PassEdit(ValidationErrors validationErrors, int ID, int mainTableID);

        /// <summary>
        /// 退回合同信息添加审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <returns></returns>
        bool ReturnAdd(ValidationErrors validationErrors, int ID);

        /// <summary>
        /// 提交合同信息添加审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        bool PassAdd(ValidationErrors validationErrors, int ID);
    }
}

