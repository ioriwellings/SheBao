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
    /// 费用_费用表其他费用 
    /// </summary>
    public partial class COST_CostTableOtherBLL :  IBLL.ICOST_CostTableOtherBLL, IDisposable
    {
        /// <summary>
        /// 创建一个费用_费用表其他费用明细
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="entity">一个费用_费用表其他费用明细</param>
        /// <returns></returns>
        public void Create(SysEntities db, COST_CostTableOther entity)
        {
            repository.Create(db, entity);
        }
    }
}

