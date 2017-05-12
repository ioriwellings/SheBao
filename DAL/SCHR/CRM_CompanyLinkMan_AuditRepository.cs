using System;
using System.Linq;

namespace Langben.DAL
{
    public partial class CRM_CompanyLinkMan_AuditRepository
    {
        /// <summary>
        /// 提交企业联系人信息修改审核
        /// </summary>
        /// <param name="entity">修改信息</param>
        /// <returns></returns>
        public int ModifyContact(CRM_CompanyLinkMan_Audit entity)
        {
            if (entity != null)
            {
                using (SysEntities db = new SysEntities())
                {
                    CRM_CompanyLinkMan linkModel = db.CRM_CompanyLinkMan.Where(e => e.ID == entity.CRM_CompanyLinkMan_ID).FirstOrDefault();
                    linkModel.Status = 2;//修改中
                    db.CRM_CompanyLinkMan_Audit.Add(entity);
                    return db.SaveChanges();
                }
            }
            else
            {
                return 0;
            }
        }


        /// <summary>
        /// 退回联系人信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        public int ReturnEdit(int ID, int mainTableID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_CompanyLinkMan comModel = db.CRM_CompanyLinkMan.Where(e => e.ID == mainTableID).FirstOrDefault();
                comModel.Status = 1;//启用

                CRM_CompanyLinkMan_Audit comAudit = db.CRM_CompanyLinkMan_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 0;//未通过

                return db.SaveChanges();
            }

        }

        /// <summary>
        /// 提交企业联系人信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        public int PassEdit(int ID, int mainTableID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_CompanyLinkMan_Audit comAudit = db.CRM_CompanyLinkMan_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 2;//成功

                CRM_CompanyLinkMan comModel = db.CRM_CompanyLinkMan.Where(e => e.ID == mainTableID).FirstOrDefault();
                comModel.LinkManName = comAudit.LinkManName;
                comModel.Mobile = comAudit.Mobile;
                comModel.Position = comAudit.Position;
                comModel.Remark = comAudit.Remark;
                comModel.Telephone = comAudit.Telephone;
                comModel.Email = comAudit.Email;
                comModel.Address = comAudit.Address;
                comModel.Status = 1;//启用
                return db.SaveChanges();
            }

        }

        /// <summary>
        /// 退回联系人信息添加审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <returns></returns>
        public int ReturnAdd(int ID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_CompanyLinkMan_Audit comAudit = db.CRM_CompanyLinkMan_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 0;//未通过

                return db.SaveChanges();
            }

        }

        /// <summary>
        /// 提交企业联系人信息添加审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <returns></returns>
        public int PassAdd(int ID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_CompanyLinkMan_Audit comAudit = db.CRM_CompanyLinkMan_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 2;//成功

                CRM_CompanyLinkMan comModel = new CRM_CompanyLinkMan();
                comModel.LinkManName = comAudit.LinkManName;
                comModel.Mobile = comAudit.Mobile;
                comModel.Position = comAudit.Position;
                comModel.Remark = comAudit.Remark;
                comModel.Telephone = comAudit.Telephone;
                comModel.Email = comAudit.Email;
                comModel.Address = comAudit.Address;
                comModel.Status = 1;//启用

                comModel.CRM_Company_ID = Convert.ToInt32(comAudit.CRM_Company_ID);
                comModel.CreateTime = comAudit.CreateTime;
                comModel.CreateUserID = comAudit.CreateUserID;
                comModel.CreateUserName = comAudit.CreateUserName;
                comModel.BranchID = comAudit.BranchID;
                db.CRM_CompanyLinkMan.Add(comModel);

                return db.SaveChanges();
            }

        }
   
    }
}
