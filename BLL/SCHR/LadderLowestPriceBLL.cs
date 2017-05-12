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
    /// 报价 
    /// </summary>
    public partial class LadderLowestPriceBLL : IBLL.ILadderLowestPriceBLL, IDisposable
    {

        /// <summary>
        /// 停用报价信息
        /// </summary>
        /// <param name="entity">修改信息</param>
        /// <returns></returns>
        public bool StopPrice(string ID)
        {
            return repository.StopPrice(db, ID);
        }

        /// <summary>
        /// 校验报价信息唯一性
        /// </summary>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        public int CheckLowestPrice(int supplierID)
        {
            return repository.CheckLowestPrice(supplierID);
        }

        /// <summary>
        /// 添加报价信息
        /// </summary>
        /// <param name="lowestPrice">最低报价</param>
        /// <param name="listLadderPrice">阶梯报价</param>
        /// <returns></returns>
        public bool CreatePrice(LadderLowestPrice lowestPrice, List<LadderPrice> listLadderPrice)
        {
            return repository.CreatePrice(lowestPrice, listLadderPrice);
        }

    }
}

