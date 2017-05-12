using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using System.Data;
namespace Langben.DAL
{
    /// <summary>
    /// 客户_企业阶梯报价
    /// </summary>
    public partial class LadderPriceRepository : BaseRepository<LadderPrice>, IDisposable
    {
        /// <summary>
        /// 查询的数据
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="order">排序字段</param>
        /// <param name="sort">升序asc（默认）还是降序desc</param>
        /// <param name="search">查询条件</param>
        /// <param name="listQuery">额外的参数</param>
        /// <returns></returns>      
        public IQueryable<LadderPrice> GetBySupplierId(SysEntities db, int id)
        {
            var data = from a in db.LadderLowestPrice
                       join b in db.LadderPrice on a.Id equals b.LadderLowestPriceId
                       where a.SupplierId == id
                       select b;
            return data;
        }

        /// <summary>
        /// 校验阶梯范围合法性
        /// </summary>
        /// <param name="lowestPriceId">最低价格ID</param>
        /// <param name="beginLadder">开始人数</param>
        /// <param name="endLadder">结束人数</param>
        /// <param name="branchID">分支机构ID</param>
        /// <param name="ladderPriceID">阶梯报价ID（添加新阶梯时为空字符串）</param>
        /// <returns></returns>
        public bool CheckRange(string lowestPriceId, int beginLadder, int endLadder, string ladderPriceID)
        {
            if (beginLadder >= endLadder)
            {
                return false;
            }
            using (SysEntities db = new SysEntities())
            {
                string stateOK = Common.Status.启用.ToString();
                //阶梯报价表
                List<LadderPrice> list = db.LadderPrice
                                                    .Where(c => c.LadderLowestPriceId == lowestPriceId &&
                                                                c.Status == stateOK).ToList()//启用
                                                    .OrderBy(c => c.BeginLadder).ToList();

                foreach (LadderPrice ladder in list)
                {
                    if (string.IsNullOrEmpty(ladderPriceID) || ladder.Id != ladderPriceID)
                    {
                        //两个不同范围的开始人数不能相同
                        if (ladder.BeginLadder == beginLadder)
                        {
                            return false;
                        }
                        // 如果范围1的开始人数小于范围2的开始人数，则范围1的结束人数也必须小于范围2的开始人数
                        if (ladder.BeginLadder < beginLadder && ladder.EndLadder >= beginLadder)
                        {
                            return false;
                        }
                        // 如果范围1的开始人数大于范围2的开始人数，则其必须大于范围2的结束人数
                        if (ladder.BeginLadder > beginLadder && ladder.BeginLadder <= endLadder)
                        {
                            return false;
                        }
                    }
                }

             
            }
            return true;
        }

    }
}

