using System;
using System.Linq;

namespace Langben.DAL
{
    public partial class CRM_CompanyFinance_Payment_AuditRepository
    {
        /// <summary>
        /// 提交企业财务信息修改审核
        /// </summary>
        /// <param name="entity">修改信息</param>
        /// <returns></returns>
        public int ModifyFinance_Payment(CRM_CompanyFinance_Payment_Audit entity)
        {
            if (entity != null)
            {
                using (SysEntities db = new SysEntities())
                {
                    CRM_CompanyFinance_Payment linkModel = db.CRM_CompanyFinance_Payment.Where(e => e.ID == entity.CRM_CompanyFinance_Payment_ID).FirstOrDefault();
                    linkModel.Status = 2;//修改中
                    db.CRM_CompanyFinance_Payment_Audit.Add(entity);
                    return db.SaveChanges();
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 退回企业财务信息（付款）修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        public int ReturnEdit(int ID, int mainTableID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_CompanyFinance_Payment comModel = db.CRM_CompanyFinance_Payment.Where(e => e.ID == mainTableID).FirstOrDefault();
                comModel.Status = 1;//启用

                CRM_CompanyFinance_Payment_Audit comAudit = db.CRM_CompanyFinance_Payment_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 0;//未通过

                return db.SaveChanges();
            }

        }

        /// <summary>
        /// 提交企业财务信息（付款）修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        public int PassEdit(int ID, int mainTableID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_CompanyFinance_Payment_Audit comAudit = db.CRM_CompanyFinance_Payment_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 2;//成功

                CRM_CompanyFinance_Payment comModel = db.CRM_CompanyFinance_Payment.Where(e => e.ID == mainTableID).FirstOrDefault();
                comModel.PaymentName = comAudit.PaymentName;
                comModel.Status = 1;//启用
                return db.SaveChanges();
            }

        }
        /// <summary>
        /// 退回企业财务信息（付款）添加审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <returns></returns>
        public int ReturnAdd(int ID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_CompanyFinance_Payment_Audit comAudit = db.CRM_CompanyFinance_Payment_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 0;//未通过

                return db.SaveChanges();
            }

        }

        /// <summary>
        /// 提交企业财务信息（付款）添加审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <returns></returns>
        public int PassAdd(int ID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_CompanyFinance_Payment_Audit comAudit = db.CRM_CompanyFinance_Payment_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 2;//成功

                CRM_CompanyFinance_Payment comModel = new CRM_CompanyFinance_Payment();
                comModel.PaymentName = comAudit.PaymentName;
                comModel.Status = 1;//启用

                comModel.CRM_Company_ID = Convert.ToInt32(comAudit.CRM_Company_ID);
                comModel.CreateTime = comAudit.CreateTime;
                comModel.CreateUserID = comAudit.CreateUserID;
                comModel.CreateUserName = comAudit.CreateUserName;
                comModel.BranchID = comAudit.BranchID;
                db.CRM_CompanyFinance_Payment.Add(comModel);

                return db.SaveChanges();
            }

        }
    }
}
