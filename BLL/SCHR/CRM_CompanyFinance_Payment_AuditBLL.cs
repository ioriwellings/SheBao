using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Langben.DAL;
using Common;

namespace Langben.BLL
{
    /// <summary>
    /// 客户_企业财务信息_付款_待审核 
    /// </summary>
    public partial class CRM_CompanyFinance_Payment_AuditBLL :  IBLL.ICRM_CompanyFinance_Payment_AuditBLL, IDisposable
    {
        #region 企业财务信息
        /// <summary>
        /// 修改企业财务信息
        /// </summary>
        /// <param name="entity">企业财务信息</param>
        /// <returns></returns>
        public bool ModifyFinance_Payment(ref ValidationErrors validationErrors, CRM_CompanyFinance_Payment_Audit entity)
        {
            try
            {
                repository.ModifyFinance_Payment(entity);
                return true;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
            }
            return false;
        }
        #endregion

        /// <summary>
        /// 退回付款信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        public bool ReturnEdit(ValidationErrors validationErrors, int ID, int mainTableID)
        {
            try
            {
                repository.ReturnEdit(ID, mainTableID);
                return true;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
                return false;
            }
        }

        /// <summary>
        /// 提交付款信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        public bool PassEdit(ValidationErrors validationErrors, int ID, int mainTableID)
        {
            try
            {
                repository.PassEdit(ID, mainTableID);
                return true;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
                return false;
            }
        }

        /// <summary>
        /// 退回企业付款信息添加审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <returns></returns>
        public bool ReturnAdd(ValidationErrors validationErrors, int ID)
        {
            try
            {
                repository.ReturnAdd(ID);
                return true;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
                return false;
            }
        }

        /// <summary>
        /// 提交企业付款信息添加审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <returns></returns>
        public bool PassAdd(ValidationErrors validationErrors, int ID)
        {
            try
            {
                repository.PassAdd(ID);
                return true;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
                return false;
            }
        }
    }
}

