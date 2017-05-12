using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using System.Data;
namespace Langben.DAL
{
    /// <summary>
    /// 客户_企业报价
    /// </summary>
    public partial class CRM_CompanyPriceRepository : BaseRepository<CRM_CompanyPrice>, IDisposable
    {
        /// <summary>
        /// 停用企业报价信息
        /// </summary>
        /// <param name="entity">修改信息</param>
        /// <returns></returns>
        public int StopPrice(int ID)
        {

            using (SysEntities db = new SysEntities())
            {
                CRM_CompanyPrice model = db.CRM_CompanyPrice.Where(c => c.ID == ID).FirstOrDefault();
                model.Status = 0;//停用

                List<CRM_CompanyLadderPrice> list = new List<CRM_CompanyLadderPrice>();
                list = db.CRM_CompanyLadderPrice.Where(c => c.CRM_Company_ID == model.CRM_Company_ID && c.PRD_Product_ID == model.PRD_Product_ID).ToList();

                foreach (CRM_CompanyLadderPrice l in list)
                {
                    l.Status = 0;//停用
                }
                return db.SaveChanges();
            }

        }

        /// <summary>
        /// 获取企业报价信息列表
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="branchID">分支机构ID</param>
        /// <returns></returns>
        public Common.ClientResult.DataResult GetCompanyPirceList(int companyID, int branchID)
        {
            using (SysEntities db = new SysEntities())
            {
                var queryData = from c in db.CRM_CompanyPrice
                                join p in db.PRD_Product on c.PRD_Product_ID equals p.ID
                                where //c.BranchID == branchID &&
                                      (companyID == null || c.CRM_Company_ID == companyID)
                                select new
                                {
                                    c.ID,
                                    c.CRM_Company_ID,
                                    c.PRD_Product_ID,
                                    c.PriceType,
                                    c.LowestPrice,
                                    c.AddPrice,
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
        /// 获取报价信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public string GetCompanyPrice(int ID)
        {
            using (SysEntities db = new SysEntities())
            {
                var queryData = (from c in db.CRM_CompanyPrice
                                 join p in db.PRD_Product on c.PRD_Product_ID equals p.ID into temp
                                 from tt in temp.DefaultIfEmpty()
                                 where c.ID == ID
                                 select new
                                 {
                                     c.ID,
                                     c.PRD_Product_ID,
                                     tt.ProductName,
                                     c.PriceType,
                                     c.LowestPrice,
                                     c.AddPrice
                                 }).FirstOrDefault();

                return Newtonsoft.Json.JsonConvert.SerializeObject(queryData);
            }
        }

        /// <summary>
        /// 校验报价信息唯一性
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public int CheckPrice(int companyID)
        {
            using (SysEntities db = new SysEntities())
            {
                var q = db.CRM_CompanyPrice.Where(c => c.CRM_Company_ID == companyID && c.Status != 0);
                var q1 = db.CRM_CompanyPrice_Audit.Where(c => c.CRM_Company_ID == companyID && c.CRM_CompanyPrice_ID == null && c.OperateStatus == 1);

                if (q.Count() > 0)
                {
                    return 1;
                }
                if (q1.Count() > 0)
                {
                    return 2;
                }
            }
            return 0;
        }

        /// <summary>
        /// 获取启用中（或者修改中）的报价信息
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public List<CRM_CompanyPrice> GetActiveProduct(int companyID)
        {
            using (SysEntities db = new SysEntities())
            {
                var q = db.CRM_CompanyPrice.Where(c => c.CRM_Company_ID == companyID && c.Status != 0);
                return q.ToList();
            }
        }
    }
}

