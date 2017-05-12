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
    public partial class LadderPriceBLL : IBLL.ILadderPriceBLL, IDisposable
    {

        /// <summary>
        /// 根据SupplierId，获取所有阶梯价格数据
        /// </summary>
        /// <param name="id">外键的主键</param>
        /// <returns></returns>
        public List<LadderPrice> GetBySupplierId(int id)
        {
            return repository.GetBySupplierId(db, id).OrderBy(c => c.LadderLowestPriceId).ThenBy(c => c.Status).ThenBy(c => c.BeginLadder).ToList();
        }

        /// <summary>
        /// 校验阶梯范围合法性
        /// </summary>
        /// <param name="lowestPriceId">最低价格ID</param>
        /// <param name="beginLadder">开始人数</param>
        /// <param name="endLadder">结束人数</param>
        /// <param name="ladderPriceID">阶梯报价ID（添加新阶梯时为空字符串）</param>
        /// <returns></returns>
        public bool CheckRange(string lowestPriceId, int beginLadder, int endLadder, string ladderPriceID)
        {
            return repository.CheckRange(lowestPriceId, beginLadder, endLadder, ladderPriceID);
        }

    }
}

