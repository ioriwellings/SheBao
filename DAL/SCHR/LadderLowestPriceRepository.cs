using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using System.Data;
namespace Langben.DAL
{
    /// <summary>
    /// 企业报价
    /// </summary>
    public partial class LadderLowestPriceRepository : BaseRepository<LadderLowestPrice>, IDisposable
    {

        /// <summary>
        /// 停用企业报价信息
        /// </summary>
        /// <param name="entity">修改信息</param>
        /// <returns></returns>
        public bool StopPrice(SysEntities db, string ID)
        {
            using (db)
            {
                LadderLowestPrice model = db.LadderLowestPrice.Where(c => c.Id == ID).FirstOrDefault();
                model.Status = Common.Status.停用.ToString();//停用

                List<LadderPrice> list = new List<LadderPrice>();
                list = db.LadderPrice.Where(c => c.LadderLowestPriceId == model.Id).ToList();

                foreach (LadderPrice l in list)
                {
                    l.Status = Common.Status.停用.ToString();//停用
                }
                return db.SaveChanges().GetBool();
            }
        }

        /// <summary>
        /// 校验报价信息唯一性
        /// </summary>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        public int CheckLowestPrice(int supplierID)
        {
            using (SysEntities db = new SysEntities())
            {
                string statusOK = Common.Status.停用.ToString();
                List<LadderLowestPrice> q = db.LadderLowestPrice.Where(c => c.SupplierId == supplierID && c.Status != statusOK).ToList();
              
                if (q.Count() > 0)
                {
                    return q.Count();
                }
            }
            return 0;
        }

        /// <summary>
        /// 添加报价信息
        /// </summary>
        /// <param name="lowestPrice">最低报价</param>
        /// <param name="listLadderPrice">阶梯报价</param>
        /// <returns></returns>
        public bool CreatePrice(LadderLowestPrice lowestPrice, List<LadderPrice> listLadderPrice)
        {
            try
            {
                using (SysEntities db = new SysEntities())
                {
                    //报价
                    if (lowestPrice != null)
                    {
                        db.LadderLowestPrice.Add(lowestPrice);
                    }
                    //阶梯报价
                    if (listLadderPrice != null && listLadderPrice.Count > 0)
                    {
                        db.LadderPrice.AddRange(listLadderPrice);
                    }
                    db.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

