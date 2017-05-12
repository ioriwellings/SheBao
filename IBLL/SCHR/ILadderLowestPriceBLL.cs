using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using Langben.DAL;
using System.ServiceModel;

namespace Langben.IBLL
{
    /// <summary>
    /// 最低报价 接口
    /// </summary>
    public partial interface ILadderLowestPriceBLL
    {
        /// <summary>
        /// 停用报价信息
        /// </summary>
        /// <param name="entity">修改信息</param>
        /// <returns></returns>
        bool StopPrice(string ID);

        /// <summary>
        /// 校验报价信息唯一性
        /// </summary>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        int CheckLowestPrice(int supplierID);

        /// <summary>
        /// 添加报价信息
        /// </summary>
        /// <param name="lowestPrice">最低报价</param>
        /// <param name="listLadderPrice">阶梯报价</param>
        /// <returns></returns>
        bool CreatePrice(LadderLowestPrice lowestPrice, List<LadderPrice> listLadderPrice);
    }
}

