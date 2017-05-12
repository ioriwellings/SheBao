using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using Langben.DAL;
using System.ServiceModel;
using Langben.DAL.Model;

namespace Langben.IBLL
{
     public partial interface ICRM_CompanyLinkMan_AuditBLL
    {
         //提交企业联系人信息修改 到审核
         bool ModifyContact(ref Common.ValidationErrors validationErrors, CRM_CompanyLinkMan_Audit entity);

         /// <summary>
         /// 退回联系人信息修改审核
         /// </summary>
         /// <param name="ID">审核表ID</param>
         /// <param name="mainTableID">原表ID</param>
         /// <returns></returns>
         bool ReturnEdit(ValidationErrors validationErrors, int ID, int mainTableID);

         /// <summary>
         /// 提交联系人信息修改审核
         /// </summary>
         /// <param name="ID">审核表ID</param>
         /// <param name="mainTableID">原表ID</param>
         /// <returns></returns>
         bool PassEdit(ValidationErrors validationErrors, int ID, int mainTableID);

         /// <summary>
         /// 退回联系人信息添加审核
         /// </summary>
         /// <param name="ID">审核表ID</param>
         /// <returns></returns>
         bool ReturnAdd(ValidationErrors validationErrors, int ID);

         /// <summary>
         /// 提交联系人信息添加审核
         /// </summary>
         /// <param name="ID">审核表ID</param>
         /// <param name="mainTableID">原表ID</param>
         /// <returns></returns>
         bool PassAdd(ValidationErrors validationErrors, int ID);
    }
}

