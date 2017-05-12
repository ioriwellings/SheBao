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
    /// 客户_企业报价_待审批 
    /// </summary>
    public partial class CRM_CompanyPrice_AuditBLL : IBLL.ICRM_CompanyPrice_AuditBLL, IDisposable
    {
        #region 企业报价信息
        /// <summary>
        /// 修改企业报价信息
        /// </summary>
        /// <param name="entity">企业报价信息</param>
        /// <returns></returns>
        public bool ModifyPrice(ref ValidationErrors validationErrors, CRM_CompanyPrice_Audit entity)
        {
            try
            {
                repository.ModifyPrice(entity);
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
        /// 获取报价审核信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public string GetCompanyPrice_Audit(int ID)
        {
            return repository.GetCompanyPrice_Audit(ID);
        }

        /// <summary>
        /// 退回报价信息修改审核
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
        /// 提交报价信息修改审核
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
        /// 退回企业报价信息添加审核
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
        /// 提交企业报价信息添加审核
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

        /// <summary>
        /// 添加企业报价信息到审核表
        /// </summary>
        /// <param name="listPrice">报价信息</param>
        /// <param name="listLadderPrice">阶梯报价</param>
        /// <returns></returns>
        public bool CreatePrice_Audit( List<CRM_CompanyPrice_Audit> listPrice, List<CRM_CompanyLadderPrice_Audit> listLadderPrice)
        {
            return repository.CreatePrice_Audit(listPrice, listLadderPrice);
        }
    }
}

