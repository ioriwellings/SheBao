using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using Langben.DAL;
using System.ServiceModel;

namespace Langben.IBLL
{
    /// <summary>
    /// 阶梯报价 接口
    /// </summary>
    public partial interface ILadderPriceBLL
    {

        /// <summary>
        /// 根据SupplierId，获取所有阶梯价格数据
        /// </summary>
        /// <param name="id">外键的主键</param>
        /// <returns></returns>
        List<LadderPrice> GetBySupplierId(int id);

        /// <summary>
        /// 校验阶梯范围合法性
        /// </summary>
        /// <param name="lowestPriceId">最低价格ID</param>
        /// <param name="beginLadder">开始人数</param>
        /// <param name="endLadder">结束人数</param>
        /// <param name="ladderPriceID">阶梯报价ID（添加新阶梯时为空字符串）</param>
        /// <returns></returns>
        bool CheckRange(string lowestPriceId, int beginLadder, int endLadder, string ladderPriceID);
    }
}

