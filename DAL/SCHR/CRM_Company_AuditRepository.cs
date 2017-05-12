using Langben.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Langben.DAL
{
    public partial class CRM_Company_AuditRepository
    {
        /// <summary>
        /// 提交企业基本信息修改审核
        /// </summary>
        /// <param name="entity">修改信息</param>
        /// <returns></returns>
        public int ModifyBaseInfo(CRM_Company_Audit entity)
        {
            if (entity != null)
            {
                using (SysEntities db = new SysEntities())
                {
                    CRM_Company comModel = db.CRM_Company.Where(e => e.ID == entity.CRM_Company_ID).FirstOrDefault();
                    comModel.OperateStatus = 2;//修改中
                    db.CRM_Company_Audit.Add(entity);
                    return db.SaveChanges();
                }
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 待审核企业列表
        /// </summary>
        /// <param name="companyName">公司名称</param>
        /// <returns></returns>
        public Common.ClientResult.DataResult GetAuditCompanyList(int page, int rows, string companyName, string operateStatus)
        {
            using (SysEntities db = new SysEntities())
            {
                var queryData = from c in db.CRM_Company_Audit
                                join hy in db.CRM_ZD_HY on c.Dict_HY_Code equals hy.Code into temp
                                from tt in temp.DefaultIfEmpty()
                                where c.OperateNode == 1
                                select new
                                {
                                    c.ID,
                                    c.CompanyCode,
                                    c.CompanyName,
                                    c.Dict_HY_Code,
                                    HYName = tt.HYMC,
                                    c.OrganizationCode,
                                    c.RegisterAddress,
                                    c.OfficeAddress,
                                    c.Source,
                                    c.CreateTime,
                                    c.CreateUserID,
                                    c.CreateUserName,
                                    c.OperateStatus,
                                    c.OperateNode
                                };
                if (!string.IsNullOrEmpty(companyName))
                {
                    queryData = queryData.Where(e => e.CompanyName.Contains(companyName));
                }
                if (!string.IsNullOrEmpty(operateStatus))
                {
                    int status = int.Parse(operateStatus);
                    queryData = queryData.Where(e => e.OperateStatus == status);
                }
                int total = queryData.Count();
                var queryList = queryData.ToList();
                if (total > 0)
                {
                    if (page <= 1)
                        queryList = queryList.Take(rows).ToList();
                    else
                        queryList = queryList.Skip((page - 1) * rows).Take(rows).ToList();
                }
                var data = new Common.ClientResult.DataResult
                {
                    total = total,
                    rows = queryList
                };
                return data;
            }

        }

        /// <summary>
        /// 需重新提交的企业列表（销售用）
        /// </summary>
        /// <param name="companyName">企业名称</param>
        /// <param name="userID_XS">销售人员</param>
        /// <param name="branchID">所属分支机构</param>
        /// <returns></returns>
        public List<CRM_Company_Audit> GetCompany_AuditListForReSubmit(SysEntities db, string companyName, int userID_XS, int branchID)
        {
            List<CRM_Company_Audit> list = new List<CRM_Company_Audit>();
            using (db)
            {
                list = db.CRM_Company_Audit.Where(c => c.BranchID.Equals(branchID) &&
                                                  c.OperateStatus.Equals(0) &&//审核失败
                                                  c.OperateNode.Equals(1) &&//销售经理
                                                  c.CompanyName.Contains(companyName) &&
                                                  c.CreateUserID.Equals(userID_XS))
                                           .OrderBy(c => c.ID).ToList();
            }

            return list;
        }

        /// <summary>
        /// 企业基本信息待审核信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public string GetCompanyBaseAudit(int ID)
        {
            using (SysEntities db = new SysEntities())
            {
                var queryData = (from c in db.CRM_Company_Audit
                                 join hy in db.CRM_ZD_HY on c.Dict_HY_Code equals hy.Code into temp
                                 from tt in temp.DefaultIfEmpty()
                                 where c.ID == ID
                                 select new
                                 {
                                     c.ID,
                                     c.CRM_Company_ID,
                                     c.CompanyCode,
                                     c.CompanyName,
                                     c.Dict_HY_Code,
                                     //c.TaxRegistryNumber,
                                     c.OrganizationCode,
                                     c.RegisterAddress,
                                     c.OfficeAddress,
                                     c.Source,
                                     c.CreateTime,
                                     c.CreateUserID,
                                     c.CreateUserName,
                                     c.BranchID,
                                     c.OperateStatus,
                                     c.OperateNode,
                                     tt.ParentCode,
                                     tt.HYMC
                                 }).FirstOrDefault();

                return Newtonsoft.Json.JsonConvert.SerializeObject(queryData);
            }
        }

        /// <summary>
        /// 退回基本信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        public int ReturnBaseEdit(int ID, int mainTableID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_Company comModel = db.CRM_Company.Where(e => e.ID == mainTableID).FirstOrDefault();
                comModel.OperateStatus = 1;//启用

                CRM_Company_Audit comAudit = db.CRM_Company_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 0;//未通过

                return db.SaveChanges();
            }

        }

        /// <summary>
        /// 提交企业基本信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        public int PassBaseEdit(int ID, int mainTableID)
        {
            using (SysEntities db = new SysEntities())
            {
                CRM_Company_Audit comAudit = db.CRM_Company_Audit.Where(c => c.ID == ID).FirstOrDefault();
                comAudit.OperateStatus = 2;//成功

                CRM_Company comModel = db.CRM_Company.Where(e => e.ID == mainTableID).FirstOrDefault();
                comModel.CompanyCode = comAudit.CompanyCode;
                comModel.CompanyName = comAudit.CompanyName;
                comModel.Dict_HY_Code = comAudit.Dict_HY_Code;
                comModel.OfficeAddress = comAudit.OfficeAddress;
                comModel.OrganizationCode = comAudit.OrganizationCode;
                comModel.RegisterAddress = comAudit.RegisterAddress;
                //comModel.TaxRegistryNumber = comAudit.TaxRegistryNumber;
                comModel.OperateStatus = 1;//启用

                return db.SaveChanges();
            }

        }


        #region 质检用企业信息修改审核列表

        /// <summary>
        /// 待审核企业列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="companyName"></param>
        /// <param name="UserID_ZR"></param>
        /// <param name="UserID_XS"></param>
        /// <param name="auditType"></param>
        /// <param name="operateStatus"></param>
        /// <returns></returns>
        public IQueryable<CRM_CompanyAuditView> GetAuditCompanyListForQuality(SysEntities db, string companyName, int? UserID_ZR, int? UserID_XS, int? auditType, int? operateStatus, string companyCode)
        {
            IQueryable<CRM_CompanyAuditView> list = null;
            switch (auditType)
            {
                case 1:
                    list = GetAuditCompanyBasicList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    break;
                case 2:
                    list = GetAuditCompanyAddLinkManList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    break;
                case 3:
                    list = GetAuditCompanyEditLinkManList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    break;
                case 4:
                    list = GetAuditCompanyAddBankList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    break;
                case 5:
                    list = GetAuditCompanyEditBankList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    break;
                case 6:
                    list = GetAuditCompanyAddFinanceList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    break;
                case 7:
                    list = GetAuditCompanyEditFinanceList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    break;
                case 8:
                    list = GetAuditCompanyAddPriceList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    break;
                case 9:
                    list = GetAuditCompanyEditPriceList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    break;
                case 10:
                    list = GetAuditCompanyAddContractList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    break;
                case 11:
                    list = GetAuditCompanyEditContractList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    break;
                case 12:
                    list = GetAuditCompanyAddInsuranceList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    break;
                case 13:
                    list = GetAuditCompanyEditInsuranceList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    break;
                case null:
                    var list1 = GetAuditCompanyBasicList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    var list2 = GetAuditCompanyAddLinkManList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    var list3 = GetAuditCompanyEditLinkManList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    var list4 = GetAuditCompanyAddBankList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    var list5 = GetAuditCompanyEditBankList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    var list6 = GetAuditCompanyAddFinanceList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    var list7 = GetAuditCompanyEditFinanceList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    var list8 = GetAuditCompanyAddPriceList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    var list9 = GetAuditCompanyEditPriceList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    var list10 = GetAuditCompanyAddContractList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    var list11 = GetAuditCompanyEditContractList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    var list12 = GetAuditCompanyAddInsuranceList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);
                    var list13 = GetAuditCompanyEditInsuranceList(db, companyName, UserID_ZR, UserID_XS, operateStatus, companyCode);

                    list = list1.Concat(list2).Concat(list3).Concat(list4).Concat(list5)
                           .Concat(list6).Concat(list7).Concat(list8).Concat(list9)
                           .Concat(list10).Concat(list11).Concat(list12).Concat(list13);
                    break;
            }



            return list;
        }

        /// <summary>
        /// 修改基本信息
        /// </summary>
        /// <param name="db">DbContext</param>
        /// <param name="companyName">企业名称</param>
        /// <param name="UserID_ZR">责任客服ID</param>
        /// <param name="UserID_XS">销售ID</param>
        /// <param name="operateStatus">操作状态</param>
        /// <returns></returns>
        public IQueryable<CRM_CompanyAuditView> GetAuditCompanyBasicList(SysEntities db, string companyName, int? UserID_ZR, int? UserID_XS, int? operateStatus, string companyCode)
        {
            IQueryable<CRM_CompanyAuditView> queryData = null;

            queryData = from a in db.CRM_Company_Audit
                        join b in db.CRM_CompanyToBranch on a.CRM_Company_ID equals b.CRM_Company_ID
                        join c in db.CRM_Company on a.CRM_Company_ID equals c.ID
                        where (companyCode == "" || c.CompanyCode.Equals(companyCode)) &&
                              c.CompanyName.Contains(companyName) &&
                              (UserID_ZR == null || b.UserID_ZR == UserID_ZR) &&
                              (UserID_XS == null || b.UserID_XS == UserID_XS) &&
                              (operateStatus == null || a.OperateStatus == operateStatus)
                        select new CRM_CompanyAuditView
                        {
                            AuditType = 1,
                            AuditTypeName = "修改企业基本信息",
                            ID = a.ID,
                            MainTableID = c.ID,
                            CityID = "",
                            CompanyID = a.CRM_Company_ID,
                            CompanyCode = c.CompanyCode,
                            CompanyName = c.CompanyName,
                            UserID_ZR = b.UserID_ZR,
                            UserID_XS = b.UserID_XS,
                            CreateUserID = a.CreateUserID,
                            CreateUserName = a.CreateUserName,
                            CreateTime = a.CreateTime,
                            OperateStatus = a.OperateStatus,
                            BranchID = b.BranchID,
                            UserID_XS_Name = "",
                            UserID_ZR_Name = ""
                        };

            return queryData;
        }

        /// <summary>
        /// 新建联系人信息
        /// </summary>
        /// <param name="db">DbContext</param>
        /// <param name="companyName">企业名称</param>
        /// <param name="UserID_ZR">责任客服ID</param>
        /// <param name="UserID_XS">销售ID</param>
        /// <param name="operateStatus">操作状态</param>
        /// <returns></returns>
        public IQueryable<CRM_CompanyAuditView> GetAuditCompanyAddLinkManList(SysEntities db, string companyName, int? UserID_ZR, int? UserID_XS, int? operateStatus, string companyCode)
        {
            IQueryable<CRM_CompanyAuditView> queryData = null;

            queryData = from a in db.CRM_CompanyLinkMan_Audit
                        join c in db.CRM_Company on a.CRM_Company_ID equals c.ID
                        join b in db.CRM_CompanyToBranch on a.CRM_Company_ID equals b.CRM_Company_ID
                        where (companyCode == "" || c.CompanyCode.Equals(companyCode)) &&
                              c.CompanyName.Contains(companyName) &&
                              (UserID_ZR == null || b.UserID_ZR == UserID_ZR) &&
                              (UserID_XS == null || b.UserID_XS == UserID_XS) &&
                              (operateStatus == null || a.OperateStatus == operateStatus) &&
                              a.CRM_CompanyLinkMan_ID == null
                        select new CRM_CompanyAuditView
                        {
                            AuditType = 2,
                            AuditTypeName = "新建联系人信息",
                            ID = a.ID,
                            MainTableID = a.CRM_CompanyLinkMan_ID,
                            CityID = "",
                            CompanyID = a.CRM_Company_ID,
                            CompanyCode = c.CompanyCode,
                            CompanyName = c.CompanyName,
                            UserID_ZR = b.UserID_ZR,
                            UserID_XS = b.UserID_XS,
                            CreateUserID = a.CreateUserID,
                            CreateUserName = a.CreateUserName,
                            CreateTime = a.CreateTime,
                            OperateStatus = a.OperateStatus,
                            BranchID = b.BranchID,
                            UserID_XS_Name = "",
                            UserID_ZR_Name = ""
                        };

            return queryData;
        }

        /// <summary>
        /// 修改联系人信息
        /// </summary>
        /// <param name="db">DbContext</param>
        /// <param name="companyName">企业名称</param>
        /// <param name="UserID_ZR">责任客服ID</param>
        /// <param name="UserID_XS">销售ID</param>
        /// <param name="operateStatus">操作状态</param>
        /// <returns></returns>
        public IQueryable<CRM_CompanyAuditView> GetAuditCompanyEditLinkManList(SysEntities db, string companyName, int? UserID_ZR, int? UserID_XS, int? operateStatus, string companyCode)
        {
            IQueryable<CRM_CompanyAuditView> queryData = null;

            queryData = from a in db.CRM_CompanyLinkMan_Audit
                        join c in db.CRM_Company on a.CRM_Company_ID equals c.ID
                        join b in db.CRM_CompanyToBranch on a.CRM_Company_ID equals b.CRM_Company_ID
                        join d in db.CRM_CompanyLinkMan on a.CRM_CompanyLinkMan_ID equals d.ID
                        where (companyCode == "" || c.CompanyCode.Equals(companyCode)) &&
                              c.CompanyName.Contains(companyName) &&
                              (UserID_ZR == null || b.UserID_ZR == UserID_ZR) &&
                              (UserID_XS == null || b.UserID_XS == UserID_XS) &&
                              (operateStatus == null || a.OperateStatus == operateStatus)
                        select new CRM_CompanyAuditView
                        {
                            AuditType = 3,
                            AuditTypeName = "修改联系人信息",
                            ID = a.ID,
                            MainTableID = a.CRM_CompanyLinkMan_ID,
                            CityID = "",
                            CompanyID = a.CRM_Company_ID,
                            CompanyCode = c.CompanyCode,
                            CompanyName = c.CompanyName,
                            UserID_ZR = b.UserID_ZR,
                            UserID_XS = b.UserID_XS,
                            CreateUserID = a.CreateUserID,
                            CreateUserName = a.CreateUserName,
                            CreateTime = a.CreateTime,
                            OperateStatus = a.OperateStatus,
                            BranchID = b.BranchID,
                            UserID_XS_Name = "",
                            UserID_ZR_Name = ""
                        };

            return queryData;
        }

        /// <summary>
        /// 新建银行信息
        /// </summary>
        /// <param name="db">DbContext</param>
        /// <param name="companyName">企业名称</param>
        /// <param name="UserID_ZR">责任客服ID</param>
        /// <param name="UserID_XS">销售ID</param>
        /// <param name="operateStatus">操作状态</param>
        /// <returns></returns>
        public IQueryable<CRM_CompanyAuditView> GetAuditCompanyAddBankList(SysEntities db, string companyName, int? UserID_ZR, int? UserID_XS, int? operateStatus, string companyCode)
        {
            IQueryable<CRM_CompanyAuditView> queryData = null;

            queryData = from a in db.CRM_CompanyBankAccount_Audit
                        join c in db.CRM_Company on a.CRM_Company_ID equals c.ID
                        join b in db.CRM_CompanyToBranch on a.CRM_Company_ID equals b.CRM_Company_ID
                        where (companyCode == "" || c.CompanyCode.Equals(companyCode)) &&
                              c.CompanyName.Contains(companyName) &&
                              (UserID_ZR == null || b.UserID_ZR == UserID_ZR) &&
                              (UserID_XS == null || b.UserID_XS == UserID_XS) &&
                              (operateStatus == null || a.OperateStatus == operateStatus) &&
                              a.CRM_CompanyBankAccount_ID == null
                        select new CRM_CompanyAuditView
                        {
                            AuditType = 4,
                            AuditTypeName = "新建企业银行信息",
                            ID = a.ID,
                            MainTableID = a.CRM_CompanyBankAccount_ID,
                            CityID = "",
                            CompanyID = a.CRM_Company_ID,
                            CompanyCode = c.CompanyCode,
                            CompanyName = c.CompanyName,
                            UserID_ZR = b.UserID_ZR,
                            UserID_XS = b.UserID_XS,
                            CreateUserID = a.CreateUserID,
                            CreateUserName = a.CreateUserName,
                            CreateTime = a.CreateTime,
                            OperateStatus = a.OperateStatus,
                            BranchID = b.BranchID,
                            UserID_XS_Name = "",
                            UserID_ZR_Name = ""
                        };

            return queryData;
        }

        /// <summary>
        /// 修改银行信息
        /// </summary>
        /// <param name="db">DbContext</param>
        /// <param name="companyName">企业名称</param>
        /// <param name="UserID_ZR">责任客服ID</param>
        /// <param name="UserID_XS">销售ID</param>
        /// <param name="operateStatus">操作状态</param>
        /// <returns></returns>
        public IQueryable<CRM_CompanyAuditView> GetAuditCompanyEditBankList(SysEntities db, string companyName, int? UserID_ZR, int? UserID_XS, int? operateStatus, string companyCode)
        {
            IQueryable<CRM_CompanyAuditView> queryData = null;

            queryData = from a in db.CRM_CompanyBankAccount_Audit
                        join c in db.CRM_Company on a.CRM_Company_ID equals c.ID
                        join b in db.CRM_CompanyToBranch on a.CRM_Company_ID equals b.CRM_Company_ID
                        join d in db.CRM_CompanyBankAccount on a.CRM_CompanyBankAccount_ID equals d.ID
                        where (companyCode == "" || c.CompanyCode.Equals(companyCode)) &&
                              c.CompanyName.Contains(companyName) &&
                              (UserID_ZR == null || b.UserID_ZR == UserID_ZR) &&
                              (UserID_XS == null || b.UserID_XS == UserID_XS) &&
                              (operateStatus == null || a.OperateStatus == operateStatus)
                        select new CRM_CompanyAuditView
                        {
                            AuditType = 5,
                            AuditTypeName = "修改企业银行信息",
                            ID = a.ID,
                            MainTableID = a.CRM_CompanyBankAccount_ID,
                            CityID = "",
                            CompanyID = a.CRM_Company_ID,
                            CompanyCode = c.CompanyCode,
                            CompanyName = c.CompanyName,
                            UserID_ZR = b.UserID_ZR,
                            UserID_XS = b.UserID_XS,
                            CreateUserID = a.CreateUserID,
                            CreateUserName = a.CreateUserName,
                            CreateTime = a.CreateTime,
                            OperateStatus = a.OperateStatus,
                            BranchID = b.BranchID,
                            UserID_XS_Name = "",
                            UserID_ZR_Name = ""
                        };

            return queryData;
        }

        /// <summary>
        /// 新建财务信息
        /// </summary>
        /// <param name="db">DbContext</param>
        /// <param name="companyName">企业名称</param>
        /// <param name="UserID_ZR">责任客服ID</param>
        /// <param name="UserID_XS">销售ID</param>
        /// <param name="operateStatus">操作状态</param>
        /// <returns></returns>
        public IQueryable<CRM_CompanyAuditView> GetAuditCompanyAddFinanceList(SysEntities db, string companyName, int? UserID_ZR, int? UserID_XS, int? operateStatus, string companyCode)
        {
            IQueryable<CRM_CompanyAuditView> queryData = null;

            queryData = (from a in db.CRM_CompanyFinance_Bill_Audit
                         join c in db.CRM_Company on a.CRM_Company_ID equals c.ID
                         join b in db.CRM_CompanyToBranch on a.CRM_Company_ID equals b.CRM_Company_ID
                         where (companyCode == "" || c.CompanyCode.Equals(companyCode)) &&
                               c.CompanyName.Contains(companyName) &&
                               (UserID_ZR == null || b.UserID_ZR == UserID_ZR) &&
                               (UserID_XS == null || b.UserID_XS == UserID_XS) &&
                               (operateStatus == null || a.OperateStatus == operateStatus) &&
                               a.CRM_CompanyFinance_Bill_ID == null
                         select new CRM_CompanyAuditView
                         {
                             AuditType = 61,
                             AuditTypeName = "新建企业财务信息",
                             ID = a.ID,
                             MainTableID = a.CRM_CompanyFinance_Bill_ID,
                             CityID = "",
                             CompanyID = a.CRM_Company_ID,
                             CompanyCode = c.CompanyCode,
                             CompanyName = c.CompanyName,
                             UserID_ZR = b.UserID_ZR,
                             UserID_XS = b.UserID_XS,
                             CreateUserID = a.CreateUserID,
                             CreateUserName = a.CreateUserName,
                             CreateTime = a.CreateTime,
                             OperateStatus = a.OperateStatus,
                             BranchID = b.BranchID,
                             UserID_XS_Name = "",
                             UserID_ZR_Name = ""
                         }).Concat(
                                    from a in db.CRM_CompanyFinance_Payment_Audit
                                    join c in db.CRM_Company on a.CRM_Company_ID equals c.ID
                                    join b in db.CRM_CompanyToBranch on a.CRM_Company_ID equals b.CRM_Company_ID
                                    where (companyCode == "" || c.CompanyCode.Equals(companyCode)) &&
                                          c.CompanyName.Contains(companyName) &&
                                          (UserID_ZR == null || b.UserID_ZR == UserID_ZR) &&
                                          (UserID_XS == null || b.UserID_XS == UserID_XS) &&
                                          (operateStatus == null || a.OperateStatus == operateStatus) &&
                                          a.CRM_CompanyFinance_Payment_ID == null
                                    select new CRM_CompanyAuditView
                                    {
                                        AuditType = 62,
                                        AuditTypeName = "新建企业财务信息",
                                        ID = a.ID,
                                        MainTableID = a.CRM_CompanyFinance_Payment_ID,
                                        CityID = "",
                                        CompanyID = a.CRM_Company_ID,
                                        CompanyCode = c.CompanyCode,
                                        CompanyName = c.CompanyName,
                                        UserID_ZR = b.UserID_ZR,
                                        UserID_XS = b.UserID_XS,
                                        CreateUserID = a.CreateUserID,
                                        CreateUserName = a.CreateUserName,
                                        CreateTime = a.CreateTime,
                                        OperateStatus = a.OperateStatus,
                                        BranchID = b.BranchID,
                                        UserID_XS_Name = "",
                                        UserID_ZR_Name = ""
                                    }
                                    );

            return queryData;
        }

        /// <summary>
        /// 修改财务信息
        /// </summary>
        /// <param name="db">DbContext</param>
        /// <param name="companyName">企业名称</param>
        /// <param name="UserID_ZR">责任客服ID</param>
        /// <param name="UserID_XS">销售ID</param>
        /// <param name="operateStatus">操作状态</param>
        /// <returns></returns>
        public IQueryable<CRM_CompanyAuditView> GetAuditCompanyEditFinanceList(SysEntities db, string companyName, int? UserID_ZR, int? UserID_XS, int? operateStatus, string companyCode)
        {
            IQueryable<CRM_CompanyAuditView> queryData = null;

            queryData = (from a in db.CRM_CompanyFinance_Bill_Audit
                         join c in db.CRM_Company on a.CRM_Company_ID equals c.ID
                         join b in db.CRM_CompanyToBranch on a.CRM_Company_ID equals b.CRM_Company_ID
                         join d in db.CRM_CompanyFinance_Bill on a.CRM_CompanyFinance_Bill_ID equals d.ID
                         where (companyCode == "" || c.CompanyCode.Equals(companyCode)) &&
                               c.CompanyName.Contains(companyName) &&
                               (UserID_ZR == null || b.UserID_ZR == UserID_ZR) &&
                               (UserID_XS == null || b.UserID_XS == UserID_XS) &&
                               (operateStatus == null || a.OperateStatus == operateStatus)
                         select new CRM_CompanyAuditView
                         {
                             AuditType = 71,
                             AuditTypeName = "修改企业财务信息",
                             ID = a.ID,
                             MainTableID = a.CRM_CompanyFinance_Bill_ID,
                             CityID = "",
                             CompanyID = a.CRM_Company_ID,
                             CompanyCode = c.CompanyCode,
                             CompanyName = c.CompanyName,
                             UserID_ZR = b.UserID_ZR,
                             UserID_XS = b.UserID_XS,
                             CreateUserID = a.CreateUserID,
                             CreateUserName = a.CreateUserName,
                             CreateTime = a.CreateTime,
                             OperateStatus = a.OperateStatus,
                             BranchID = b.BranchID,
                             UserID_XS_Name = "",
                             UserID_ZR_Name = ""
                         }).Concat(
                                    from a in db.CRM_CompanyFinance_Payment_Audit
                                    join c in db.CRM_Company on a.CRM_Company_ID equals c.ID
                                    join b in db.CRM_CompanyToBranch on a.CRM_Company_ID equals b.CRM_Company_ID
                                    join d in db.CRM_CompanyFinance_Payment on a.CRM_CompanyFinance_Payment_ID equals d.ID
                                    where (companyCode == "" || c.CompanyCode.Equals(companyCode)) &&
                                          c.CompanyName.Contains(companyName) &&
                                          (UserID_ZR == null || b.UserID_ZR == UserID_ZR) &&
                                          (UserID_XS == null || b.UserID_XS == UserID_XS) &&
                                          (operateStatus == null || a.OperateStatus == operateStatus)
                                    select new CRM_CompanyAuditView
                                    {
                                        AuditType = 72,
                                        AuditTypeName = "修改企业财务信息",
                                        ID = a.ID,
                                        MainTableID = a.CRM_CompanyFinance_Payment_ID,
                                        CityID = "",
                                        CompanyID = a.CRM_Company_ID,
                                        CompanyCode = c.CompanyCode,
                                        CompanyName = c.CompanyName,
                                        UserID_ZR = b.UserID_ZR,
                                        UserID_XS = b.UserID_XS,
                                        CreateUserID = a.CreateUserID,
                                        CreateUserName = a.CreateUserName,
                                        CreateTime = a.CreateTime,
                                        OperateStatus = a.OperateStatus,
                                        BranchID = b.BranchID,
                                        UserID_XS_Name = "",
                                        UserID_ZR_Name = ""
                                    }
                                    );

            return queryData;
        }

        /// <summary>
        /// 新建报价信息
        /// </summary>
        /// <param name="db">DbContext</param>
        /// <param name="companyName">企业名称</param>
        /// <param name="UserID_ZR">责任客服ID</param>
        /// <param name="UserID_XS">销售ID</param>
        /// <param name="operateStatus">操作状态</param>
        /// <returns></returns>
        public IQueryable<CRM_CompanyAuditView> GetAuditCompanyAddPriceList(SysEntities db, string companyName, int? UserID_ZR, int? UserID_XS, int? operateStatus, string companyCode)
        {
            IQueryable<CRM_CompanyAuditView> queryData = null;

            queryData = (from a in db.CRM_CompanyPrice_Audit
                         join c in db.CRM_Company on a.CRM_Company_ID equals c.ID
                         join b in db.CRM_CompanyToBranch on a.CRM_Company_ID equals b.CRM_Company_ID
                         where (companyCode == "" || c.CompanyCode.Equals(companyCode)) &&
                               c.CompanyName.Contains(companyName) &&
                               (UserID_ZR == null || b.UserID_ZR == UserID_ZR) &&
                               (UserID_XS == null || b.UserID_XS == UserID_XS) &&
                               (operateStatus == null || a.OperateStatus == operateStatus) &&
                               a.CRM_CompanyPrice_ID == null
                         select new CRM_CompanyAuditView
                         {
                             AuditType = 81,
                             AuditTypeName = "新建企业报价信息",
                             ID = a.ID,
                             MainTableID = a.CRM_CompanyPrice_ID,
                             CityID = "",
                             CompanyID = a.CRM_Company_ID,
                             CompanyCode = c.CompanyCode,
                             CompanyName = c.CompanyName,
                             UserID_ZR = b.UserID_ZR,
                             UserID_XS = b.UserID_XS,
                             CreateUserID = a.CreateUserID,
                             CreateUserName = a.CreateUserName,
                             CreateTime = a.CreateTime,
                             OperateStatus = a.OperateStatus,
                             BranchID = b.BranchID,
                             UserID_XS_Name = "",
                             UserID_ZR_Name = ""
                         }).Concat(
                                    from a in db.CRM_CompanyLadderPrice_Audit
                                    join c in db.CRM_Company on a.CRM_Company_ID equals c.ID
                                    join b in db.CRM_CompanyToBranch on a.CRM_Company_ID equals b.CRM_Company_ID
                                    where (companyCode == "" || c.CompanyCode.Equals(companyCode)) &&
                                          c.CompanyName.Contains(companyName) &&
                                          (UserID_ZR == null || b.UserID_ZR == UserID_ZR) &&
                                          (UserID_XS == null || b.UserID_XS == UserID_XS) &&
                                          (operateStatus == null || a.OperateStatus == operateStatus) &&
                                          a.CRM_CompanyLadderPrice_ID == null
                                    select new CRM_CompanyAuditView
                                    {
                                        AuditType = 82,
                                        AuditTypeName = "新建企业报价信息",
                                        ID = a.ID,
                                        MainTableID = a.CRM_CompanyLadderPrice_ID,
                                        CityID = "",
                                        CompanyID = a.CRM_Company_ID,
                                        CompanyCode = c.CompanyCode,
                                        CompanyName = c.CompanyName,
                                        UserID_ZR = b.UserID_ZR,
                                        UserID_XS = b.UserID_XS,
                                        CreateUserID = a.CreateUserID,
                                        CreateUserName = a.CreateUserName,
                                        CreateTime = a.CreateTime,
                                        OperateStatus = a.OperateStatus,
                                        BranchID = b.BranchID,
                                        UserID_XS_Name = "",
                                        UserID_ZR_Name = ""
                                    }
                                    );

            return queryData;
        }

        /// <summary>
        /// 修改报价信息
        /// </summary>
        /// <param name="db">DbContext</param>
        /// <param name="companyName">企业名称</param>
        /// <param name="UserID_ZR">责任客服ID</param>
        /// <param name="UserID_XS">销售ID</param>
        /// <param name="operateStatus">操作状态</param>
        /// <returns></returns>
        public IQueryable<CRM_CompanyAuditView> GetAuditCompanyEditPriceList(SysEntities db, string companyName, int? UserID_ZR, int? UserID_XS, int? operateStatus, string companyCode)
        {
            IQueryable<CRM_CompanyAuditView> queryData = null;

            queryData = (from a in db.CRM_CompanyPrice_Audit
                         join c in db.CRM_Company on a.CRM_Company_ID equals c.ID
                         join b in db.CRM_CompanyToBranch on a.CRM_Company_ID equals b.CRM_Company_ID
                         join d in db.CRM_CompanyPrice on a.CRM_CompanyPrice_ID equals d.ID
                         where (companyCode == "" || c.CompanyCode.Equals(companyCode)) &&
                               c.CompanyName.Contains(companyName) &&
                               (UserID_ZR == null || b.UserID_ZR == UserID_ZR) &&
                               (UserID_XS == null || b.UserID_XS == UserID_XS) &&
                               (operateStatus == null || a.OperateStatus == operateStatus)
                         select new CRM_CompanyAuditView
                         {
                             AuditType = 91,
                             AuditTypeName = "修改企业报价信息",
                             ID = a.ID,
                             MainTableID = a.CRM_CompanyPrice_ID,
                             CityID = "",
                             CompanyID = a.CRM_Company_ID,
                             CompanyCode = c.CompanyCode,
                             CompanyName = c.CompanyName,
                             UserID_ZR = b.UserID_ZR,
                             UserID_XS = b.UserID_XS,
                             CreateUserID = a.CreateUserID,
                             CreateUserName = a.CreateUserName,
                             CreateTime = a.CreateTime,
                             OperateStatus = a.OperateStatus,
                             BranchID = b.BranchID,
                             UserID_XS_Name = "",
                             UserID_ZR_Name = ""
                         }).Concat(
                                    from a in db.CRM_CompanyLadderPrice_Audit
                                    join c in db.CRM_Company on a.CRM_Company_ID equals c.ID
                                    join b in db.CRM_CompanyToBranch on a.CRM_Company_ID equals b.CRM_Company_ID
                                    join d in db.CRM_CompanyLadderPrice on a.CRM_CompanyLadderPrice_ID equals d.ID
                                    where (companyCode == "" || c.CompanyCode.Equals(companyCode)) &&
                                          c.CompanyName.Contains(companyName) &&
                                          (UserID_ZR == null || b.UserID_ZR == UserID_ZR) &&
                                          (UserID_XS == null || b.UserID_XS == UserID_XS) &&
                                          (operateStatus == null || a.OperateStatus == operateStatus)
                                    select new CRM_CompanyAuditView
                                    {
                                        AuditType = 92,
                                        AuditTypeName = "修改企业报价信息",
                                        ID = a.ID,
                                        MainTableID = a.CRM_CompanyLadderPrice_ID,
                                        CityID = "",
                                        CompanyID = a.CRM_Company_ID,
                                        CompanyCode = c.CompanyCode,
                                        CompanyName = c.CompanyName,
                                        UserID_ZR = b.UserID_ZR,
                                        UserID_XS = b.UserID_XS,
                                        CreateUserID = a.CreateUserID,
                                        CreateUserName = a.CreateUserName,
                                        CreateTime = a.CreateTime,
                                        OperateStatus = a.OperateStatus,
                                        BranchID = b.BranchID,
                                        UserID_XS_Name = "",
                                        UserID_ZR_Name = ""
                                    }
                                    );

            return queryData;
        }

        /// <summary>
        /// 新建合同信息
        /// </summary>
        /// <param name="db">DbContext</param>
        /// <param name="companyName">企业名称</param>
        /// <param name="UserID_ZR">责任客服ID</param>
        /// <param name="UserID_XS">销售ID</param>
        /// <param name="operateStatus">操作状态</param>
        /// <returns></returns>
        public IQueryable<CRM_CompanyAuditView> GetAuditCompanyAddContractList(SysEntities db, string companyName, int? UserID_ZR, int? UserID_XS, int? operateStatus, string companyCode)
        {
            IQueryable<CRM_CompanyAuditView> queryData = null;

            queryData = from a in db.CRM_CompanyContract_Audit
                        join c in db.CRM_Company on a.CRM_Company_ID equals c.ID
                        join b in db.CRM_CompanyToBranch on a.CRM_Company_ID equals b.CRM_Company_ID
                        where (companyCode == "" || c.CompanyCode.Equals(companyCode)) &&
                              c.CompanyName.Contains(companyName) &&
                              (UserID_ZR == null || b.UserID_ZR == UserID_ZR) &&
                              (UserID_XS == null || b.UserID_XS == UserID_XS) &&
                              (operateStatus == null || a.OperateStatus == operateStatus) &&
                              a.CRM_CompanyContract_ID == null
                        select new CRM_CompanyAuditView
                        {
                            AuditType = 10,
                            AuditTypeName = "新建企业合同信息",
                            ID = a.ID,
                            MainTableID = a.CRM_CompanyContract_ID,
                            CityID = "",
                            CompanyID = a.CRM_Company_ID,
                            CompanyCode = c.CompanyCode,
                            CompanyName = c.CompanyName,
                            UserID_ZR = b.UserID_ZR,
                            UserID_XS = b.UserID_XS,
                            CreateUserID = a.CreateUserID,
                            CreateUserName = a.CreateUserName,
                            CreateTime = a.CreateTime,
                            OperateStatus = a.OperateStatus,
                            BranchID = b.BranchID,
                            UserID_XS_Name = "",
                            UserID_ZR_Name = ""
                        };

            return queryData;
        }

        /// <summary>
        /// 修改合同信息
        /// </summary>
        /// <param name="db">DbContext</param>
        /// <param name="companyName">企业名称</param>
        /// <param name="UserID_ZR">责任客服ID</param>
        /// <param name="UserID_XS">销售ID</param>
        /// <param name="operateStatus">操作状态</param>
        /// <returns></returns>
        public IQueryable<CRM_CompanyAuditView> GetAuditCompanyEditContractList(SysEntities db, string companyName, int? UserID_ZR, int? UserID_XS, int? operateStatus, string companyCode)
        {
            IQueryable<CRM_CompanyAuditView> queryData = null;

            queryData = from a in db.CRM_CompanyContract_Audit
                        join c in db.CRM_Company on a.CRM_Company_ID equals c.ID
                        join b in db.CRM_CompanyToBranch on a.CRM_Company_ID equals b.CRM_Company_ID
                        join d in db.CRM_CompanyContract on a.CRM_CompanyContract_ID equals d.ID
                        where (companyCode == "" || c.CompanyCode.Equals(companyCode)) &&
                              c.CompanyName.Contains(companyName) &&
                              (UserID_ZR == null || b.UserID_ZR == UserID_ZR) &&
                              (UserID_XS == null || b.UserID_XS == UserID_XS) &&
                              (operateStatus == null || a.OperateStatus == operateStatus)
                        select new CRM_CompanyAuditView
                        {
                            AuditType = 11,
                            AuditTypeName = "修改企业合同信息",
                            ID = a.ID,
                            MainTableID = a.CRM_CompanyContract_ID,
                            CityID = "",
                            CompanyID = a.CRM_Company_ID,
                            CompanyCode = c.CompanyCode,
                            CompanyName = c.CompanyName,
                            UserID_ZR = b.UserID_ZR,
                            UserID_XS = b.UserID_XS,
                            CreateUserID = a.CreateUserID,
                            CreateUserName = a.CreateUserName,
                            CreateTime = a.CreateTime,
                            OperateStatus = a.OperateStatus,
                            BranchID = b.BranchID,
                            UserID_XS_Name = "",
                            UserID_ZR_Name = ""
                        };

            return queryData;
        }

        /// <summary>
        /// 新建社保信息
        /// </summary>
        /// <param name="db">DbContext</param>
        /// <param name="companyName">企业名称</param>
        /// <param name="UserID_ZR">责任客服ID</param>
        /// <param name="UserID_XS">销售ID</param>
        /// <param name="operateStatus">操作状态</param>
        /// <returns></returns>
        public IQueryable<CRM_CompanyAuditView> GetAuditCompanyAddInsuranceList(SysEntities db, string companyName, int? UserID_ZR, int? UserID_XS, int? operateStatus, string companyCode)
        {
            IQueryable<CRM_CompanyAuditView> queryData = null;

            int addType = (int)Common.OperateType.添加;

            queryData = (from a in db.CRM_Company_Insurance_Audit
                         join c in db.CRM_Company on a.CRM_Company_ID equals c.ID
                         join b in db.CRM_CompanyToBranch on a.CRM_Company_ID equals b.CRM_Company_ID
                         where (companyCode == "" || c.CompanyCode.Equals(companyCode)) &&
                               c.CompanyName.Contains(companyName) &&
                               (UserID_ZR == null || b.UserID_ZR == UserID_ZR) &&
                               (UserID_XS == null || b.UserID_XS == UserID_XS) &&
                               (operateStatus == null || a.OperateStatus == operateStatus) &&
                               a.OperateType == addType
                         select new CRM_CompanyAuditView
                         {
                             AuditType = 12,
                             AuditTypeName = "新建企业社保信息",
                             ID = 0,
                             MainTableID = 0,
                             CityID = a.City,
                             CompanyID = a.CRM_Company_ID,
                             CompanyCode = c.CompanyCode,
                             CompanyName = c.CompanyName,
                             UserID_ZR = b.UserID_ZR,
                             UserID_XS = b.UserID_XS,
                             CreateUserID = a.CreateUserID,
                             CreateUserName = a.CreatePerson,
                             CreateTime = a.CreateTime,
                             OperateStatus = a.OperateStatus,
                             BranchID = b.BranchID,
                             UserID_XS_Name = "",
                             UserID_ZR_Name = ""
                         }).Distinct().Union((from a in db.CRM_Company_PoliceInsurance_Audit
                                              join c in db.CRM_Company on a.CRM_Company_ID equals c.ID
                                              join b in db.CRM_CompanyToBranch on a.CRM_Company_ID equals b.CRM_Company_ID
                                              where (companyCode == "" || c.CompanyCode.Equals(companyCode)) &&
                                                    c.CompanyName.Contains(companyName) &&
                                                    (UserID_ZR == null || b.UserID_ZR == UserID_ZR) &&
                                                    (UserID_XS == null || b.UserID_XS == UserID_XS) &&
                                                    (operateStatus == null || a.OperateStatus == operateStatus) &&
                                                    a.OperateType == addType
                                              select new CRM_CompanyAuditView
                                              {
                                                  AuditType = 12,
                                                  AuditTypeName = "新建企业社保信息",
                                                  ID = 0,
                                                  MainTableID = 0,
                                                  CityID = a.City,
                                                  CompanyID = a.CRM_Company_ID,
                                                  CompanyCode = c.CompanyCode,
                                                  CompanyName = c.CompanyName,
                                                  UserID_ZR = b.UserID_ZR,
                                                  UserID_XS = b.UserID_XS,
                                                  CreateUserID = a.CreateUserID,
                                                  CreateUserName = a.CreatePerson,
                                                  CreateTime = a.CreateTime,
                                                  OperateStatus = a.OperateStatus,
                                                  BranchID = b.BranchID,
                                                  UserID_XS_Name = "",
                                                  UserID_ZR_Name = ""
                                              }

                            ).Distinct());

            return queryData;
        }

        /// <summary>
        /// 修改社保信息
        /// </summary>
        /// <param name="db">DbContext</param>
        /// <param name="companyName">企业名称</param>
        /// <param name="UserID_ZR">责任客服ID</param>
        /// <param name="UserID_XS">销售ID</param>
        /// <param name="operateStatus">操作状态</param>
        /// <returns></returns>
        public IQueryable<CRM_CompanyAuditView> GetAuditCompanyEditInsuranceList(SysEntities db, string companyName, int? UserID_ZR, int? UserID_XS, int? operateStatus, string companyCode)
        {
            IQueryable<CRM_CompanyAuditView> queryData = null;
            int editType = (int)Common.OperateType.修改;
            queryData = (from a in db.CRM_Company_Insurance_Audit
                         join c in db.CRM_Company on a.CRM_Company_ID equals c.ID
                         join b in db.CRM_CompanyToBranch on a.CRM_Company_ID equals b.CRM_Company_ID
                         //join d in db.CRM_Company_Insurance on a.CRM_Company_Insurance_ID equals d.ID
                         where (companyCode == "" || c.CompanyCode.Equals(companyCode)) &&
                               c.CompanyName.Contains(companyName) &&
                               (UserID_ZR == null || b.UserID_ZR == UserID_ZR) &&
                               (UserID_XS == null || b.UserID_XS == UserID_XS) &&
                               (operateStatus == null || a.OperateStatus == operateStatus) &&
                               a.OperateType == editType
                         select new CRM_CompanyAuditView
                         {
                             AuditType = 13,
                             AuditTypeName = "修改企业社保信息",
                             ID = 0,
                             MainTableID = 0,
                             CityID = a.City,
                             CompanyID = a.CRM_Company_ID,
                             CompanyCode = c.CompanyCode,
                             CompanyName = c.CompanyName,
                             UserID_ZR = b.UserID_ZR,
                             UserID_XS = b.UserID_XS,
                             CreateUserID = a.CreateUserID,
                             CreateUserName = a.CreatePerson,
                             CreateTime = a.CreateTime,
                             OperateStatus = a.OperateStatus,
                             BranchID = b.BranchID,
                             UserID_XS_Name = "",
                             UserID_ZR_Name = ""
                         }).Distinct().Union((from a in db.CRM_Company_PoliceInsurance_Audit
                                              join c in db.CRM_Company on a.CRM_Company_ID equals c.ID
                                              join b in db.CRM_CompanyToBranch on a.CRM_Company_ID equals b.CRM_Company_ID
                                              //join d in db.CRM_Company_Insurance on a.CRM_Company_PoliceInsurance_ID equals d.ID
                                              where (companyCode == "" || c.CompanyCode.Equals(companyCode)) &&
                                                    c.CompanyName.Contains(companyName) &&
                                                    (UserID_ZR == null || b.UserID_ZR == UserID_ZR) &&
                                                    (UserID_XS == null || b.UserID_XS == UserID_XS) &&
                                                    (operateStatus == null || a.OperateStatus == operateStatus) &&
                                                    a.OperateType == editType
                                              select new CRM_CompanyAuditView
                                              {
                                                  AuditType = 13,
                                                  AuditTypeName = "修改企业社保信息",
                                                  ID = 0,
                                                  MainTableID = 0,
                                                  CityID = a.City,
                                                  CompanyID = a.CRM_Company_ID,
                                                  CompanyCode = c.CompanyCode,
                                                  CompanyName = c.CompanyName,
                                                  UserID_ZR = b.UserID_ZR,
                                                  UserID_XS = b.UserID_XS,
                                                  CreateUserID = a.CreateUserID,
                                                  CreateUserName = a.CreatePerson,
                                                  CreateTime = a.CreateTime,
                                                  OperateStatus = a.OperateStatus,
                                                  BranchID = b.BranchID,
                                                  UserID_XS_Name = "",
                                                  UserID_ZR_Name = ""
                                              }

                  ).Distinct());

            return queryData;
        }

        #endregion
    }
}

