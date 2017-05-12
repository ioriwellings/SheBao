using System;
using System.Linq;

namespace Langben.DAL
{
    public partial class CRM_CompanyBankAccount_AuditRepository
    {
        /// <summary>
        /// 提交企业银行信息修改审核
        /// </summary>
        /// <param name="entity">修改信息</param>
        /// <returns></returns>
        public int ModifyBank(CRM_CompanyBankAccount_Audit entity)
        {
            if (entity != null)
            {
                using (SysEntities db = new SysEntities())
                {
                    CRM_CompanyBankAccount bankModel = db.CRM_CompanyBankAccount.Where(e => e.ID == entity.CRM_CompanyBankAccount_ID).FirstOrDefault();
                    bankModel.Status = 2;//修改中
                    db.CRM_CompanyBankAccount_Audit.Add(entity);
                    return db.SaveChanges();
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 退回银行信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        public int ReturnEdit(int ID, int mainTableID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_CompanyBankAccount comModel = db.CRM_CompanyBankAccount.Where(e => e.ID == mainTableID).FirstOrDefault();
                comModel.Status = 1;//启用

                CRM_CompanyBankAccount_Audit comAudit = db.CRM_CompanyBankAccount_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 0;//未通过

                return db.SaveChanges();
            }

        }

        /// <summary>
        /// 提交企业银行信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        public int PassEdit(int ID, int mainTableID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_CompanyBankAccount_Audit comAudit = db.CRM_CompanyBankAccount_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 2;//成功

                CRM_CompanyBankAccount comModel = db.CRM_CompanyBankAccount.Where(e => e.ID == mainTableID).FirstOrDefault();
                comModel.Account = comAudit.Account;
                comModel.Bank = comAudit.Bank;
                comModel.Status = 1;//启用
                return db.SaveChanges();
            }

        }

        /// <summary>
        /// 退回银行信息添加审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <returns></returns>
        public int ReturnAdd(int ID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_CompanyBankAccount_Audit comAudit = db.CRM_CompanyBankAccount_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 0;//未通过

                return db.SaveChanges();
            }

        }

        /// <summary>
        /// 提交企业银行信息添加审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <returns></returns>
        public int PassAdd(int ID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_CompanyBankAccount_Audit comAudit = db.CRM_CompanyBankAccount_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 2;//成功

                CRM_CompanyBankAccount comModel = new CRM_CompanyBankAccount();
                comModel.CRM_Company_ID = Convert.ToInt32(comAudit.CRM_Company_ID);

                comModel.Account = comAudit.Account;
                comModel.Bank = comAudit.Bank;
                comModel.Status = 1;//启用

                comModel.CreateTime = comAudit.CreateTime;
                comModel.CreateUserID = comAudit.CreateUserID;
                comModel.CreateUserName = comAudit.CreateUserName;
                comModel.BranchID = comAudit.BranchID;
                db.CRM_CompanyBankAccount.Add(comModel);

                return db.SaveChanges();
            }

        }

    }
}
