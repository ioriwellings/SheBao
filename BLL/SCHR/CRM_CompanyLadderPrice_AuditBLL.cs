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
    /// 客户_企业阶梯报价_待审核 
    /// </summary>
    public partial class CRM_CompanyLadderPrice_AuditBLL :  IBLL.ICRM_CompanyLadderPrice_AuditBLL, IDisposable
    {
        #region 企业阶梯报价信息
        /// <summary>
        /// 修改企业阶梯报价信息
        /// </summary>
        /// <param name="entity">企业阶梯报价信息</param>
        /// <returns></returns>
        public bool ModifyLadderPrice(ref ValidationErrors validationErrors, CRM_CompanyLadderPrice_Audit entity)
        {
            try
            {
                repository.ModifyLadderPrice(entity);
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
        /// 获取阶梯报价审核信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public string GetCompanyLadderPrice_Audit(int ID)
        {
            return repository.GetCompanyLadderPrice_Audit(ID);
        }

        /// <summary>
        /// 退回阶梯报价信息修改审核
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
        /// 提交阶梯报价信息修改审核
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
        /// 退回企业阶梯报价信息添加审核
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
        /// 提交企业阶梯报价信息添加审核
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

