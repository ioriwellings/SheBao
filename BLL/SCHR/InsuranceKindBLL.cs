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
    /// 社保种类 
    /// </summary>
    public partial class InsuranceKindBLL : IBLL.IInsuranceKindBLL, IDisposable
    {


        /// <summary>
        /// 获取在该表一条数据中，出现的所有外键实体
        /// </summary>
        /// <param name="kindId">社保种类ID</param>
        /// <returns>外键实体集合</returns>
        public List<PoliceOperation> GetRefPoliceOperationForStop(int kindId)
        {
            var query = repository.GetRefPoliceOperation(db, kindId).Where(o => o.Style == "报减");
            return query.ToList();
        }


    }
}

