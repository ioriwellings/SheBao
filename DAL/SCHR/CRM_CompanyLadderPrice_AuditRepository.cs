using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using System.Data;
namespace Langben.DAL
{
    /// <summary>
    /// 客户_企业阶梯报价_待审核
    /// </summary>
    public partial class CRM_CompanyLadderPrice_AuditRepository : BaseRepository<CRM_CompanyLadderPrice_Audit>, IDisposable
    {
        /// <summary>
        /// 提交企业阶梯报价信息修改审核
        /// </summary>
        /// <param name="entity">修改信息</param>
        /// <returns></returns>
        public int ModifyLadderPrice(CRM_CompanyLadderPrice_Audit entity)
        {
            if (entity != null)
            {
                using (SysEntities db = new SysEntities())
                {
                    CRM_CompanyLadderPrice linkModel = db.CRM_CompanyLadderPrice.Where(e => e.ID == entity.CRM_CompanyLadderPrice_ID).FirstOrDefault();
                    linkModel.Status = 2;//修改中
                    db.CRM_CompanyLadderPrice_Audit.Add(entity);
                    return db.SaveChanges();
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取阶梯报价审核信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public string GetCompanyLadderPrice_Audit(int ID)
        {
            using (SysEntities db = new SysEntities())
            {
                var queryData = (from c in db.CRM_CompanyLadderPrice_Audit
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

        /// <summary>
        /// 退回企业阶梯报价信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        public int ReturnEdit(int ID, int mainTableID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_CompanyLadderPrice comModel = db.CRM_CompanyLadderPrice.Where(e => e.ID == mainTableID).FirstOrDefault();
                comModel.Status = 1;//启用

                CRM_CompanyLadderPrice_Audit comAudit = db.CRM_CompanyLadderPrice_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 0;//未通过

                return db.SaveChanges();
            }

        }

        /// <summary>
        /// 提交企业阶梯报价信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        public int PassEdit(int ID, int mainTableID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_CompanyLadderPrice_Audit comAudit = db.CRM_CompanyLadderPrice_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 2;//成功

                CRM_CompanyLadderPrice comModel = db.CRM_CompanyLadderPrice.Where(e => e.ID == mainTableID).FirstOrDefault();
                comModel.BeginLadder = comAudit.BeginLadder;
                comModel.EndLadder = comAudit.EndLadder;
                comModel.PRD_Product_ID = comAudit.PRD_Product_ID;
                comModel.SinglePrice = comAudit.SinglePrice;
                comModel.Status = 1;//启用
                return db.SaveChanges();
            }

        }

        /// <summary>
        /// 退回企业阶梯报价信息添加审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <returns></returns>
        public int ReturnAdd(int ID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_CompanyLadderPrice_Audit comAudit = db.CRM_CompanyLadderPrice_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 0;//未通过

                return db.SaveChanges();
            }

        }

        /// <summary>
        /// 提交企业阶梯报价信息添加审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <returns></returns>
        public int PassAdd(int ID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_CompanyLadderPrice_Audit comAudit = db.CRM_CompanyLadderPrice_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 2;//成功

                CRM_CompanyLadderPrice comModel = new CRM_CompanyLadderPrice();
                comModel.BeginLadder = comAudit.BeginLadder;
                comModel.EndLadder = comAudit.EndLadder;
                comModel.PRD_Product_ID = comAudit.PRD_Product_ID;
                comModel.SinglePrice = comAudit.SinglePrice;
                comModel.Status = 1;//启用

                comModel.CRM_Company_ID = Convert.ToInt32(comAudit.CRM_Company_ID);
                comModel.CreateTime = comAudit.CreateTime;
                comModel.CreateUserID = comAudit.CreateUserID;
                comModel.CreateUserName = comAudit.CreateUserName;
                comModel.BranchID = comAudit.BranchID;
                db.CRM_CompanyLadderPrice.Add(comModel);

                return db.SaveChanges();
            }

        }
    }
}

