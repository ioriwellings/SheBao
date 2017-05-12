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
    public partial class CRM_CompanyLinkManBLL : IBLL.ICRM_CompanyLinkManBLL, IDisposable
    {
        #region 企业联系人信息
        /// <summary>
        /// 设置默认联系人
        /// </summary>
        /// <param name="id">联系人id</param>
        /// <returns></returns>
        public bool SetDefault(ref ValidationErrors validationErrors, int id,int companyID)
        {
            try
            {
                repository.SetDefault(id,companyID);
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
    }
}
