using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using System.Data;
namespace Langben.DAL
{
    /// <summary>
    /// 客户_企业报价_待审批
    /// </summary>
    public partial class CRM_CompanyPrice_AuditRepository : BaseRepository<CRM_CompanyPrice_Audit>, IDisposable
    {
        /// <summary>
        /// 提交企业报价信息修改审核
        /// </summary>
        /// <param name="entity">修改信息</param>
        /// <returns></returns>
        public int ModifyPrice(CRM_CompanyPrice_Audit entity)
        {
            if (entity != null)
            {
                using (SysEntities db = new SysEntities())
                {
                    CRM_CompanyPrice linkModel = db.CRM_CompanyPrice.Where(e => e.ID == entity.CRM_CompanyPrice_ID).FirstOrDefault();
                    linkModel.Status = 2;//修改中
                    db.CRM_CompanyPrice_Audit.Add(entity);
                    return db.SaveChanges();
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取报价审核信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public string GetCompanyPrice_Audit(int ID)
        {
            using (SysEntities db = new SysEntities())
            {
                var queryData = (from c in db.CRM_CompanyPrice_Audit
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
        /// 退回企业报价信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        public int ReturnEdit(int ID, int mainTableID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_CompanyPrice comModel = db.CRM_CompanyPrice.Where(e => e.ID == mainTableID).FirstOrDefault();
                comModel.Status = 1;//启用

                CRM_CompanyPrice_Audit comAudit = db.CRM_CompanyPrice_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 0;//未通过

                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 提交企业报价信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        public int PassEdit(int ID, int mainTableID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_CompanyPrice_Audit comAudit = db.CRM_CompanyPrice_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 2;//成功

                CRM_CompanyPrice comModel = db.CRM_CompanyPrice.Where(e => e.ID == mainTableID).FirstOrDefault();
                comModel.AddPrice = comAudit.AddPrice;
                comModel.LowestPrice = comAudit.LowestPrice;
                comModel.PRD_Product_ID = comAudit.PRD_Product_ID;
                comModel.Status = 1;//启用
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 退回企业报价信息添加审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <returns></returns>
        public int ReturnAdd(int ID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_CompanyPrice_Audit comAudit = db.CRM_CompanyPrice_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 0;//未通过

                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 提交企业报价信息添加审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <returns></returns>
        public int PassAdd(int ID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_CompanyPrice_Audit comAudit = db.CRM_CompanyPrice_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 2;//成功

                CRM_CompanyPrice comModel = new CRM_CompanyPrice();
                comModel.AddPrice = comAudit.AddPrice;
                comModel.LowestPrice = comAudit.LowestPrice;
                comModel.PRD_Product_ID = comAudit.PRD_Product_ID;
                comModel.Status = 1;//启用

                comModel.CRM_Company_ID = Convert.ToInt32(comAudit.CRM_Company_ID);
                comModel.CreateTime = comAudit.CreateTime;
                comModel.CreateUserID = comAudit.CreateUserID;
                comModel.CreateUserName = comAudit.CreateUserName;
                comModel.BranchID = comAudit.BranchID;
                db.CRM_CompanyPrice.Add(comModel);

                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 添加企业报价信息到审核表
        /// </summary>
        /// <param name="listPrice">报价信息</param>
        /// <param name="listLadderPrice">阶梯报价</param>
        /// <returns></returns>
        public bool CreatePrice_Audit( List<CRM_CompanyPrice_Audit> listPrice, List<CRM_CompanyLadderPrice_Audit> listLadderPrice)
        {
            try
            {
                using (SysEntities db = new SysEntities())
                {
                    //报价
                    if (listPrice != null && listPrice.Count > 0)
                    {
                        db.CRM_CompanyPrice_Audit.AddRange(listPrice);
                    }
                    //阶梯报价
                    if (listLadderPrice != null && listLadderPrice.Count > 0)
                    {
                        db.CRM_CompanyLadderPrice_Audit.AddRange(listLadderPrice);
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

