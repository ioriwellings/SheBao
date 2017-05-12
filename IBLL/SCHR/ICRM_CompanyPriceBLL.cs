using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using Langben.DAL;
using System.ServiceModel;

namespace Langben.IBLL
{
    /// <summary>
    /// 客户_企业报价 接口
    /// </summary>

    public partial interface ICRM_CompanyPriceBLL
    {

        /// <summary>
        /// 停用修改企业报价信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        bool StopPrice(ref ValidationErrors validationErrors, int ID);


        /// <summary>
        /// 获取企业报价信息列表
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="branchID">分支机构ID</param>
        /// <returns></returns>
        Common.ClientResult.DataResult GetCompanyPirceList(int companyID, int branchID);

        /// <summary>
        /// 获取报价信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        string GetCompanyPrice(int ID);

        /// <summary>
        /// 校验报价信息唯一性
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        int CheckPrice(int companyID);

        /// <summary>
        /// 获取启用中（或者修改中）的报价信息
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        List<CRM_CompanyPrice> GetActiveProduct(int companyID);
    }
}

