using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using System.Data;
namespace Langben.DAL
{
    /// <summary>
    /// 客户_企业合同信息_待审核
    /// </summary>
    public partial class CRM_CompanyContract_AuditRepository : BaseRepository<CRM_CompanyContract_Audit>, IDisposable
    {
        /// <summary>
        /// 提交企业合同信息修改审核
        /// </summary>
        /// <param name="entity">修改信息</param>
        /// <returns></returns>
        public int ModifyContract(CRM_CompanyContract_Audit entity)
        {
            if (entity != null)
            {
                using (SysEntities db = new SysEntities())
                {
                    CRM_CompanyContract linkModel = db.CRM_CompanyContract.Where(e => e.ID == entity.CRM_CompanyContract_ID).FirstOrDefault();
                    linkModel.Status = 2;//修改中
                    db.CRM_CompanyContract_Audit.Add(entity);
                    return db.SaveChanges();
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 退回企业合同信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        public int ReturnEdit(int ID, int mainTableID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_CompanyContract comModel = db.CRM_CompanyContract.Where(e => e.ID == mainTableID).FirstOrDefault();
                comModel.Status = 1;//启用

                CRM_CompanyContract_Audit comAudit = db.CRM_CompanyContract_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 0;//未通过

                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 提交企业合同信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        public int PassEdit(int ID, int mainTableID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_CompanyContract_Audit comAudit = db.CRM_CompanyContract_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 2;//成功

                CRM_CompanyContract comModel = db.CRM_CompanyContract.Where(e => e.ID == mainTableID).FirstOrDefault();
                comModel.BillDay = comAudit.BillDay;
                comModel.ChangeDay = comAudit.ChangeDay;
                comModel.DatumDay = comAudit.DatumDay;
                comModel.FeesCycle = comAudit.FeesCycle;
                comModel.ReceivedDay = comAudit.ReceivedDay;
                comModel.SendBillDay = comAudit.SendBillDay;
                comModel.ServceEndDay = comAudit.ServceEndDay;
                comModel.ServiceBeginDay = comAudit.ServceEndDay;
                comModel.Status = 1;//启用
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 退回企业合同信息添加审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <returns></returns>
        public int ReturnAdd(int ID)
        {
            using (SysEntities db = new SysEntities())
            {

                CRM_CompanyContract_Audit comAudit = db.CRM_CompanyContract_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 0;//未通过

                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 提交企业合同信息添加审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <returns></returns>
        public int PassAdd(int ID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_CompanyContract_Audit comAudit = db.CRM_CompanyContract_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 2;//成功

                CRM_CompanyContract comModel = new CRM_CompanyContract();
                comModel.BillDay = comAudit.BillDay;
                comModel.ChangeDay = comAudit.ChangeDay;
                comModel.DatumDay = comAudit.DatumDay;
                comModel.FeesCycle = comAudit.FeesCycle;
                comModel.ReceivedDay = comAudit.ReceivedDay;
                comModel.SendBillDay = comAudit.SendBillDay;
                comModel.ServceEndDay = comAudit.ServceEndDay;
                comModel.ServiceBeginDay = comAudit.ServceEndDay;
                comModel.Status = 1;//启用

                comModel.CRM_Company_ID = Convert.ToInt32(comAudit.CRM_Company_ID);
                comModel.CreateTime = comAudit.CreateTime;
                comModel.CreateUserID = comAudit.CreateUserID;
                comModel.CreateUserName = comAudit.CreateUserName;
                comModel.BranchID = comAudit.BranchID;
                db.CRM_CompanyContract.Add(comModel);

                return db.SaveChanges();
            }
        }
    }
}

