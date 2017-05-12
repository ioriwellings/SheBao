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
    /// 费用_费用表服务费 
    /// </summary>
    public partial class COST_CostTableServiceBLL : IBLL.ICOST_CostTableServiceBLL, IDisposable
    {
        /// <summary>
        /// 创建一个费用_费用表服务费明细
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="entity">一个费用_费用表服务费明细</param>
        /// <returns></returns>
        public void Create(SysEntities db, COST_CostTableService entity)
        {
            repository.Create(db, entity);
        }
    }
}

