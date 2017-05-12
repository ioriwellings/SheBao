using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Langben.DAL;

namespace Langben.BLL
{
    public partial class CRM_CompanyContract_AuditBLL : IBLL.ICRM_CompanyContract_AuditBLL, IDisposable
    {
        #region 企业合同信息
        /// <summary>
        /// 修改企业合同信息
        /// </summary>
        /// <param name="entity">修改企业合同信息</param>
        /// <returns></returns>
        public bool ModifyContract(ref ValidationErrors validationErrors, CRM_CompanyContract_Audit entity)
        {
            try
            {
                repository.ModifyContract(entity);
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
        /// 退回企业合同信息修改审核
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
        /// 提交企业合同信息修改审核
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
        /// 退回企业合同信息添加审核
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
        /// 提交企业合同信息添加审核
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
