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
    /// 费用_费用表社保明细 
    /// </summary>
    public partial class COST_CostTableInsuranceBLL : IBLL.ICOST_CostTableInsuranceBLL, IDisposable
    {

        /// <summary>
        /// 创建一个费用_费用表社保明细
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="entity">一个费用_费用表社保明细</param>
        /// <returns></returns>
        public void Create(SysEntities db, COST_CostTableInsurance entity)
        {
            repository.Create(db, entity);
        }
    }
}

