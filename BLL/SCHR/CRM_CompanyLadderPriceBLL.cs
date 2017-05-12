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
    /// 客户_企业阶梯报价 
    /// </summary>
    public partial class CRM_CompanyLadderPriceBLL : IBLL.ICRM_CompanyLadderPriceBLL, IDisposable
    {
        /// <summary>
        /// 校验阶梯范围合法性
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="productID">产品ID</param>
        /// <param name="beginLadder">开始人数</param>
        /// <param name="endLadder">结束人数</param>
        /// <param name="branchID">分支机构ID</param>
        /// <param name="ladderPriceID">阶梯报价ID（添加新阶梯时为空）</param>
        /// <returns></returns>
        public bool CheckRange(int companyID, int productID, int beginLadder, int endLadder, int branchID, int? ladderPriceID)
        {
            return repository.CheckRange(companyID, productID, beginLadder, endLadder, branchID, ladderPriceID);
        }

        /// <summary>
        /// 获取企业阶梯报价信息列表
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="branchID">分支机构ID</param>
        /// <returns></returns>
        public Common.ClientResult.DataResult GetCompanyLadderPirceList(int companyID, int branchID)
        {
            var queryData = repository.GetCompanyLadderPirceList(companyID, branchID);

            return queryData;
        }

        /// <summary>
        /// 获取阶梯报价信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public string GetCompanyLadderPrice(int ID)
        {
            return repository.GetCompanyLadderPrice(ID);
        }
    }
}

