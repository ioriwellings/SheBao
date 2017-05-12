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
    public partial class CRM_CompanyLadderPriceRepository : BaseRepository<CRM_CompanyLadderPrice>, IDisposable
    {
        /// <summary>
        /// 校验阶梯范围合法性
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="productID">产品ID</param>
        /// <param name="beginLadder">开始人数</param>
        /// <param name="endLadder">结束人数</param>
        /// <param name="branchID">分支机构ID</param>
        /// <param name="ladderPriceID">阶梯报价ID（添加新阶梯时为空）</param>
        /// <returns></returns>
        public bool CheckRange(int companyID, int productID, int beginLadder, int endLadder, int branchID, int? ladderPriceID)
        {
            if (beginLadder >= endLadder)
            {
                return false;
            }
            using (SysEntities db = new SysEntities())
            {
                //阶梯报价表
                List<CRM_CompanyLadderPrice> list = db.CRM_CompanyLadderPrice
                                                    .Where(c => c.CRM_Company_ID == companyID &&
                                                                c.PRD_Product_ID == productID &&
                                                                //c.BranchID == branchID &&
                                                                c.Status == 1).ToList()//启用
                                                    .OrderBy(c => c.BeginLadder).ToList();

                foreach (CRM_CompanyLadderPrice ladder in list)
                {
                    if (ladderPriceID == null || ladder.ID != ladderPriceID)
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

                //阶梯报价待审核表
                List<CRM_CompanyLadderPrice_Audit> list_Audit = db.CRM_CompanyLadderPrice_Audit
                                                    .Where(c => c.CRM_Company_ID == companyID &&
                                                                c.PRD_Product_ID == productID &&
                                                                //c.BranchID == branchID &&
                                                                c.OperateStatus == 1).ToList()//待处理
                                                    .OrderBy(c => c.BeginLadder).ToList();

                foreach (CRM_CompanyLadderPrice_Audit ladder in list_Audit)
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
            return true;
        }

        /// <summary>
        /// 获取企业阶梯报价信息列表
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="branchID">分支机构ID</param>
        /// <returns></returns>
        public Common.ClientResult.DataResult GetCompanyLadderPirceList(int companyID, int branchID)
        {
            using (SysEntities db = new SysEntities())
            {
                var queryData = from c in db.CRM_CompanyLadderPrice
                                join p in db.PRD_Product on c.PRD_Product_ID equals p.ID
                                where //c.BranchID == branchID &&
                                      (companyID == null || c.CRM_Company_ID == companyID)
                                select new
                                {
                                    c.ID,
                                    c.CRM_Company_ID,
                                    c.PRD_Product_ID,
                                    c.SinglePrice,
                                    c.BeginLadder,
                                    c.EndLadder,
                                    c.Status,
                                    c.BranchID,
                                    p.ProductName
                                };

                int total = queryData.Count();
                var queryList = queryData.ToList();

                var data = new Common.ClientResult.DataResult
                {
                    total = total,
                    rows = queryList
                };
                return data;
            }

        }

        /// <summary>
        /// 获取阶梯报价信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public string GetCompanyLadderPrice(int ID)
        {
            using (SysEntities db = new SysEntities())
            {
                var queryData = (from c in db.CRM_CompanyLadderPrice
                                 join p in db.PRD_Product on c.PRD_Product_ID equals p.ID into temp
                                 from tt in temp.DefaultIfEmpty()
                                 where c.ID == ID
                                 select new
                                 {
                                     c.ID,
                                     c.PRD_Product_ID,
                                     tt.ProductName,
                                     c.SinglePrice,
                                     c.BeginLadder,
                                     c.EndLadder
                                 }).FirstOrDefault();

                return Newtonsoft.Json.JsonConvert.SerializeObject(queryData);
            }
        }
    }
}

