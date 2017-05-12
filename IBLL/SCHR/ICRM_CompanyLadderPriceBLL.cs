using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using Langben.DAL;
using System.ServiceModel;

namespace Langben.IBLL
{
    /// <summary>
    /// 客户_企业阶梯报价 接口
    /// </summary>
    public partial interface ICRM_CompanyLadderPriceBLL
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
        bool CheckRange(int companyID, int productID, int beginLadder, int endLadder, int branchID, int? ladderPriceID);

        /// <summary>
        /// 获取企业阶梯报价信息列表
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="branchID">分支机构ID</param>
        /// <returns></returns>
        Common.ClientResult.DataResult GetCompanyLadderPirceList(int companyID, int branchID);

        /// <summary>
        /// 获取阶梯报价信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        string GetCompanyLadderPrice(int ID);
    }
}

