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
    /// 客户_企业报价 
    /// </summary>
    public partial class CRM_CompanyPriceBLL : IBLL.ICRM_CompanyPriceBLL, IDisposable
    {
        #region 停用企业报价信息
        /// <summary>
        /// 停用修改企业报价信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool StopPrice(ref ValidationErrors validationErrors, int ID)
        {
            try
            {
                repository.StopPrice(ID);
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
        /// 获取企业报价信息列表
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="branchID">分支机构ID</param>
        /// <returns></returns>
        public Common.ClientResult.DataResult GetCompanyPirceList(int companyID, int branchID)
        {
            var queryData = repository.GetCompanyPirceList(companyID, branchID);

            return queryData;
        }

        /// <summary>
        /// 获取报价信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public string GetCompanyPrice(int ID)
        {
            return repository.GetCompanyPrice(ID);
        }

        /// <summary>
        /// 校验报价信息唯一性
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public int CheckPrice(int companyID)
        {
            return repository.CheckPrice(companyID);
        }

        /// <summary>
        /// 获取启用中（或者修改中）的报价信息
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public List<CRM_CompanyPrice> GetActiveProduct(int companyID)
        {
            return repository.GetActiveProduct(companyID);
        }
    }
}

