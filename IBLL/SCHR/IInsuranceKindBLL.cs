using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using Langben.DAL;
using System.ServiceModel;

namespace Langben.IBLL
{
    /// <summary>
    /// 社保种类 接口
    /// </summary>
 public partial interface IInsuranceKindBLL
    {
        /// <summary>
        /// 获取在该表中出现的所有外键实体
        /// </summary>
        /// <param name="kindId">社保种类</param>
        /// <returns></returns>
        List<PoliceOperation> GetRefPoliceOperationForStop(int kindId);
        
       
    }
}

