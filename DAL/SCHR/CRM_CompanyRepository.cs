using Common;
using Langben.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL
{
    public partial class CRM_CompanyRepository
    {
        /// <summary>
        /// 根据责任客服查询企业(ID,CompanyName)
        /// </summary>
        /// <param name="zrUserID">责任客服编号</param>
        /// <returns></returns>
        public List<CRM_Company> GetCompanyDateForZrUser(int zrUserID)
        {
            List<CRM_Company> list = new List<CRM_Company>();
            using (SysEntities db = new SysEntities())
            {
                var query = from a in db.CRM_Company
                            join b in db.CRM_CompanyToBranch on a.ID equals b.CRM_Company_ID
                            where b.UserID_ZR == zrUserID
                            select a;
                list = query.OrderBy(o => o.CompanyName).ToList();
            }

            return list;
        }
        /// <summary>
        /// 根据员工客服查询企业(ID,CompanyName)
        /// </summary>
        /// <param name="zrUserID">员工客服编号</param>
        /// <returns></returns>
        public List<CRM_Company> GetCompanyDateForYGUser(int YGUserID)
        {
            List<CRM_Company> list = new List<CRM_Company>();
            using (SysEntities db = new SysEntities())
            {
                var query = from a in db.CRM_Company
                            join b in db.UserCityCompany on a.ID equals b.CompanyId
                            where b.UserID_YG == YGUserID
                            select a;

                list = query.OrderBy(o => o.CompanyName).ToList();
            }

            return list;
        }

        /// <summary>
        /// 根据企业名称查询企业（销售用）
        /// </summary>
        /// <param name="companyName">企业名称</param>
        /// <param name="branchID">所属分支机构</param>
        /// <returns></returns>
        public IQueryable<CRM_CompanyView> GetCompanyListForSales(SysEntities db, string companyName, int? userID_XS, int branchID)
        {
            var query = from a in db.CRM_Company
                        join b in db.CRM_CompanyToBranch on a.ID equals b.CRM_Company_ID
                        join d in db.ORG_User on b.UserID_ZR equals d.ID into tmpzr
                        from zr in tmpzr.DefaultIfEmpty()
                        join c in db.ORG_User on b.UserID_XS equals c.ID into tmpxs
                        from xs in tmpxs.DefaultIfEmpty()
                        where a.CompanyName.Contains(companyName) &&
                              (userID_XS == null || b.UserID_XS == userID_XS)
                        select new CRM_CompanyView()
                        {
                            ID = b.ID,
                            CRM_Company_ID = a.ID,
                            CompanyCode = a.CompanyCode,
                            CompanyName = a.CompanyName,
                            UserID_XS = b.UserID_XS,
                            OperateStatus = a.OperateStatus,
                            CreateTime = a.CreateTime,
                            UserID_ZR = b.UserID_ZR,
                            UserID_XS_Name = xs.RName,
                            UserID_ZR_Name = zr.RName
                        };
            return query.OrderBy(e => e.ID);
        }

        /// <summary>
        /// 根据企业名称查询企业（销售用）【带权限】
        /// </summary>
        /// <param name="companyName">企业名称</param>
        /// <param name="branchID">所属分支机构</param>
        /// <returns></returns>
        public IQueryable<CRM_CompanyView> GetCompanyListForSales(SysEntities db, string companyName, int? userID_XS, int branchID, string menuID, int departmentScope, int userID, int userDepartmentID, string departments)
        {
            List<CRM_CompanyView> list = new List<CRM_CompanyView>();

            var query = from a in db.CRM_Company
                        join b in db.CRM_CompanyToBranch on a.ID equals b.CRM_Company_ID
                        join d in db.ORG_User on b.UserID_ZR equals d.ID into tmpzr
                        from zr in tmpzr.DefaultIfEmpty()
                        join c in db.ORG_User on b.UserID_XS equals c.ID into tmpxs
                        from xs in tmpxs.DefaultIfEmpty()
                        where a.CompanyName.Contains(companyName) &&
                              (userID_XS == null || b.UserID_XS == userID_XS)
                        select new CRM_CompanyView()
                        {
                            ID = b.ID,
                            CRM_Company_ID = a.ID,
                            CompanyCode = a.CompanyCode,
                            CompanyName = a.CompanyName,
                            UserID_XS = b.UserID_XS,
                            OperateStatus = a.OperateStatus,
                            CreateTime = a.CreateTime,
                            UserID_ZR = b.UserID_ZR,
                            UserID_XS_Name = xs.RName,
                            UserID_ZR_Name = zr.RName
                        };
            if (departmentScope == (int)DepartmentScopeAuthority.无限制)//无限制
            {
                if (!string.IsNullOrEmpty(departments))
                {
                    var people = from a in db.ORG_User
                                 where departments.Split(',').Contains(a.ORG_Department_ID.ToString())
                                 select a;

                    query = from a in query join c in people on a.UserID_XS equals c.ID select a;
                }
            }
            else if (departmentScope == (int)DepartmentScopeAuthority.本机构及下属机构)//本机构及下属机构
            {
                //当前用户直属机构

                //查询本机构及下属机构所有部门数据
                var branch = db.ORG_Department.FirstOrDefault(o => o.ID == branchID);

                var people = from a in db.ORG_User
                             join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                             where a.XYBZ == "Y" && b.XYBZ == "Y" && b.LeftValue >= branch.LeftValue && b.RightValue <= branch.RightValue
                             select a;

                query = from cc in query
                        join f in people on cc.UserID_XS equals f.ID
                        select cc;

            }
            else if (departmentScope == (int)DepartmentScopeAuthority.本机构) //本机构
            {
                //查询本机构所有部门数据

                var people = from a in db.ORG_User
                             join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                             where a.XYBZ == "Y" && b.XYBZ == "Y" && b.BranchID == branchID
                             select a;
                query = (from cc in query
                         join f in people on cc.UserID_XS equals f.ID
                         select cc);

            }
            else if (departmentScope == (int)DepartmentScopeAuthority.本部门及其下属部门)//本部门及其下属部门
            {
                //当前用户所属部门
                ORG_Department department = db.ORG_Department.FirstOrDefault(o => o.ID == userDepartmentID);
                //查询本部门及下属部门所有部门数据
                var people = from a in db.ORG_User
                             join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                             where a.XYBZ == "Y" && b.XYBZ == "Y" && b.LeftValue >= department.LeftValue && b.RightValue <= department.RightValue
                             select a;
                query = (from cc in query
                         join f in people on cc.UserID_XS equals f.ID
                         select cc);
            }
            else if (departmentScope == (int)DepartmentScopeAuthority.本部门) //本部门
            {
                //查询本部门所有用户数据
                var people = from a in db.ORG_User.Where(o => o.XYBZ == "Y")
                             join b in db.ORG_Department.Where(o => o.XYBZ == "Y" && o.BranchID == userDepartmentID) on a.ORG_Department_ID equals b.ID
                             select a;
                query = (from cc in query
                         join f in people on cc.UserID_XS equals f.ID
                         select cc);
            }
            else if (departmentScope == (int)DepartmentScopeAuthority.本人) //本人
            {
                query = (from cc in query
                         where cc.UserID_XS == userID
                         select cc);
            }

            query = query.OrderByDescending(o => o.CreateTime);
            return query;

        }
        // 公司基本信息
        public string GetCompanyBase(int companyID)
        {
            using (SysEntities db = new SysEntities())
            {
                var queryData = (from c in db.CRM_Company
                                 join hy in db.CRM_ZD_HY on c.Dict_HY_Code equals hy.Code into temp
                                 from tt in temp.DefaultIfEmpty()
                                 where c.ID == companyID
                                 select new
                                 {
                                     c.ID,
                                     c.CompanyCode,
                                     c.CompanyName,
                                     c.Dict_HY_Code,
                                     c.OrganizationCode,
                                     c.RegisterAddress,
                                     c.OfficeAddress,
                                     c.OperateStatus,
                                     tt.ParentCode,
                                     tt.HYMC
                                 }).FirstOrDefault();

                return Newtonsoft.Json.JsonConvert.SerializeObject(queryData);
            }
        }
        // 创建新公司
        //public int CreateNewCompany(CRM_Company baseModel, CRM_CompanyContract contractModel, CRM_CompanyToBranch branchModel, List<CRM_CompanyLinkMan> listLink, List<CRM_CompanyBankAccount> listBank, List<CRM_CompanyFinance_Bill> listBill, List<CRM_CompanyFinance_Payment> listPay, List<CRM_CompanyPrice> listPrice, List<CRM_CompanyLadderPrice> listLadderPrice)
        public int CreateNewCompany(CRM_Company baseModel, CRM_CompanyContract contractModel, CRM_CompanyToBranch branchModel, List<CRM_CompanyLinkMan> listLink, List<CRM_CompanyBankAccount> listBank, List<CRM_CompanyFinance_Bill> listBill, List<CRM_CompanyFinance_Payment> listPay, List<CRM_CompanyPrice> listPrice, List<CRM_CompanyLadderPrice> listLadderPrice,List<CRM_Company_PoliceInsurance> CompanyPoliceInsurance, List<CRM_Company_Insurance> CompanyInsurance)
        {
            using (SysEntities db = new SysEntities())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        //创建公司
                        db.CRM_Company.Add(baseModel);
                        db.SaveChanges();
                        baseModel.CompanyCode = baseModel.ID.ToString();//编号等于ID
                        //添加公司分支机构
                        branchModel.CRM_Company_ID = baseModel.ID;
                        db.CRM_CompanyToBranch.Add(branchModel);
                        //添加合同信息
                        contractModel.CRM_Company_ID = baseModel.ID;
                        db.CRM_CompanyContract.Add(contractModel);
                        //联系人
                        if (listLink != null && listLink.Count > 0)
                        {
                            listLink = GetLinkList(listLink, baseModel.ID);
                            db.CRM_CompanyLinkMan.AddRange(listLink);
                        }
                        //银行
                        if (listBank != null && listBank.Count > 0)
                        {
                            listBank = GetBankList(listBank, baseModel.ID);
                            db.CRM_CompanyBankAccount.AddRange(listBank);
                        }
                        //开票
                        if (listBill != null && listBill.Count > 0)
                        {
                            listBill = GetBillList(listBill, baseModel.ID);
                            db.CRM_CompanyFinance_Bill.AddRange(listBill);
                        }
                        //回款
                        if (listPay != null && listPay.Count > 0)
                        {
                            listPay = GetPayList(listPay, baseModel.ID);
                            db.CRM_CompanyFinance_Payment.AddRange(listPay);
                        }
                        //报价
                        if (listPrice != null && listPrice.Count > 0)
                        {
                            listPrice = GetPriceList(listPrice, baseModel.ID);
                            db.CRM_CompanyPrice.AddRange(listPrice);
                        }
                        //阶梯报价
                        if (listLadderPrice != null && listLadderPrice.Count > 0)
                        {
                            listLadderPrice = GetLadderList(listLadderPrice, baseModel.ID);
                            db.CRM_CompanyLadderPrice.AddRange(listLadderPrice);
                        }
                        //企业社保政策
                        if (CompanyPoliceInsurance != null && CompanyPoliceInsurance.Count > 0)
                        {
                            CompanyPoliceInsurance = GetPoliceInsuranceList(CompanyPoliceInsurance, baseModel.ID);
                            db.CRM_Company_PoliceInsurance.AddRange(CompanyPoliceInsurance);
                        }
                        //企业社保信息
                        if (CompanyInsurance != null && CompanyInsurance.Count > 0)
                        {
                            CompanyInsurance = GetInsuranceList(CompanyInsurance, baseModel.ID);
                            db.CRM_Company_Insurance.AddRange(CompanyInsurance);
                        }
                        db.SaveChanges();
                        tran.Commit();
                        return 1;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return 0;
                    }
                }
            }
        }
        /// <summary>
        /// 验证公司名称唯一
        /// </summary>
        /// <param name="companyID"></param>
        /// <param name="CompanyName"></param>
        /// <returns></returns>
        public int CheckCompanyName(string companyID, string companyName)
        {
            using (SysEntities db = new SysEntities())
            {
                var list = from c in db.CRM_Company
                           where c.CompanyName.Equals(companyName)
                           select new
                           {
                               ID=c.ID,
                               CompanyName = c.CompanyName
                           };
                if (!string.IsNullOrEmpty(companyID))
                {
                    int id = int.Parse(companyID);
                    list = list.Where(e => e.ID != id);
                }
                return list.Count();
            }
        }
        /// <summary>
        /// 验证开票名称的唯一性
        /// </summary>
        /// <param name="companyID">公司id</param>
        /// <param name="billName">开票名称</param>
        /// <returns></returns>
        public int CheckTaxNumber(string companyID, string taxNumber)
        {
            using (SysEntities db = new SysEntities())
            {
                //主表
                var list = from c in db.CRM_CompanyFinance_Bill
                           where c.TaxRegistryNumber.Equals(taxNumber) && c.Status != 0
                           select new
                           {
                               ID=c.ID,
                               CRM_Company_ID = c.CRM_Company_ID,
                               TaxRegistryNumber = c.TaxRegistryNumber,
                               BillName = c.BillName
                           };

                if (!string.IsNullOrEmpty(companyID))
                {
                    int id = int.Parse(companyID);
                    list = list.Where(e => e.CRM_Company_ID != id);
                }
                //审核表
                var auditList = from c in db.CRM_CompanyFinance_Bill_Audit
                                where c.TaxRegistryNumber.Equals(taxNumber) && c.OperateStatus == 1
                                select new
                                {
                                    ID = c.ID,
                                    TaxRegistryNumber = c.TaxRegistryNumber,
                                    BillName = c.BillName
                                };
                return list.Count()+auditList.Count();
            }
        }
        #region 信伟青
        /// <summary>
        /// 根据企业名称查询企业（新分配客服用）
        /// </summary>
        /// <param name="companyName">企业名称</param>
        /// <param name="branchID">所属分支机构</param>
        /// <returns></returns>
        public List<CRM_CompanyView> GetCompanyListForNewService(SysEntities db, string companyName, int? userID_ZR, int branchID)
        {
            List<CRM_CompanyView> list = new List<CRM_CompanyView>();
            using (db)
            {
                var query = from a in db.CRM_Company
                            join b in db.CRM_CompanyToBranch on a.ID equals b.CRM_Company_ID
                            //join c in db.ORG_User on b.UserID_ZR equals c.ID
                            //where b.BranchID == branchID &&
                            //      a.CompanyName.Contains(companyName) &&
                            where a.CompanyName.Contains(companyName) &&
                                  (b.UserID_ZR == null || b.UserID_ZR == 0)
                            select new CRM_CompanyView()
                            {
                                ID = b.ID,
                                CRM_Company_ID = a.ID,
                                CompanyCode = a.CompanyCode,
                                CompanyName = a.CompanyName,
                                UserID_XS = b.UserID_XS,
                                //UserID_XS_Name=c.RName,
                                OperateStatus = a.OperateStatus,
                                CreateTime = a.CreateTime,
                                UserID_ZR = b.UserID_ZR
                                //UserID_ZR_Name=c.RName
                            };
                list = query.OrderBy(o => o.CRM_Company_ID).ToList();
            }

            return list;
        }

        /// <summary>
        /// 根据企业名称查询企业（新分配客服用）【带权限】
        /// </summary>
        /// <param name="companyName">企业名称</param>
        /// <param name="branchID">所属分支机构</param>
        /// <returns></returns>
        public List<CRM_CompanyView> GetCompanyListForNewService(SysEntities db, string companyName, int? userID_ZR, int branchID, int departmentScope, int userID, int userDepartmentID, string departments)
        {
            List<CRM_CompanyView> list = new List<CRM_CompanyView>();
            using (db)
            {
                var query = from a in db.CRM_Company
                            join b in db.CRM_CompanyToBranch on a.ID equals b.CRM_Company_ID
                            join c in db.ORG_User on b.UserID_ZR equals c.ID into tmpcc
                            from cc in tmpcc.DefaultIfEmpty()
                            join d in db.ORG_User on b.UserID_XS equals d.ID into tmpdd
                            from dd in tmpdd.DefaultIfEmpty()
                            where a.CompanyName.Contains(companyName) &&
                                  b.UserID_ZR == null
                            select new CRM_CompanyView()
                            {
                                ID = b.ID,
                                CRM_Company_ID = a.ID,
                                CompanyCode = a.CompanyCode,
                                CompanyName = a.CompanyName,
                                UserID_XS = b.UserID_XS,
                                UserID_XS_Name = dd.RName,
                                OperateStatus = a.OperateStatus,
                                CreateTime = a.CreateTime,
                                UserID_ZR = b.UserID_ZR,
                                UserID_ZR_Name = cc.RName
                            };
                if (departmentScope == (int)DepartmentScopeAuthority.无限制)//无限制
                {
                    if (!string.IsNullOrEmpty(departments))
                    {
                        var people = from a in db.ORG_User
                                     where departments.Split(',').Contains(a.ORG_Department_ID.ToString())
                                     select a;

                        query = from a in query join c in people on a.UserID_ZR equals c.ID select a;
                    }
                }
                else if (departmentScope == (int)DepartmentScopeAuthority.本机构及下属机构)//本机构及下属机构
                {
                    //当前用户直属机构                    
                    var branch = db.ORG_Department.FirstOrDefault(o => o.ID == branchID);
                    //查询本机构及下属机构所有部门数据
                    var people = from a in db.ORG_User
                                 join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                                 where a.XYBZ == "Y" && b.XYBZ == "Y" && b.LeftValue >= branch.LeftValue && b.RightValue <= branch.RightValue
                                 select a;

                    query = from cc in query
                            join f in people on cc.UserID_ZR equals f.ID
                            select cc;

                }
                else if (departmentScope == (int)DepartmentScopeAuthority.本机构) //本机构
                {
                    //查询本机构所有部门数据

                    var people = from a in db.ORG_User
                                 join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                                 where a.XYBZ == "Y" && b.XYBZ == "Y" && b.BranchID == branchID
                                 select a;
                    query = (from cc in query
                             join f in people on cc.UserID_ZR equals f.ID
                             select cc);

                }
                else if (departmentScope == (int)DepartmentScopeAuthority.本部门及其下属部门)//本部门及其下属部门
                {
                    //当前用户所属部门
                    ORG_Department department = db.ORG_Department.FirstOrDefault(o => o.ID == userDepartmentID);
                    //查询本部门及下属部门所有部门数据
                    var people = from a in db.ORG_User
                                 join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                                 where a.XYBZ == "Y" && b.XYBZ == "Y" && b.LeftValue >= department.LeftValue && b.RightValue <= department.RightValue
                                 select a;
                    query = (from cc in query
                             join f in people on cc.UserID_ZR equals f.ID
                             select cc);
                }
                else if (departmentScope == (int)DepartmentScopeAuthority.本部门) //本部门
                {
                    //查询本部门所有用户数据
                    var people = from a in db.ORG_User.Where(o => o.XYBZ == "Y")
                                 join b in db.ORG_Department.Where(o => o.XYBZ == "Y" && o.BranchID == userDepartmentID) on a.ORG_Department_ID equals b.ID
                                 select a;
                    query = (from cc in query
                             join f in people on cc.UserID_ZR equals f.ID
                             select cc);
                }
                else if (departmentScope == (int)DepartmentScopeAuthority.本人) //本人
                {
                    query = (from cc in query
                             where cc.UserID_ZR == userID
                             select cc);
                }
                list = query.OrderBy(o => o.CRM_Company_ID).ToList();
            }

            return list;
        }


        /// <summary>
        /// 根据企业名称查询已服务企业（分配客服用）
        /// </summary>
        /// <param name="companyName">企业名称</param>
        /// <param name="branchID">所属分支机构</param>
        /// <returns></returns>
        public List<CRM_CompanyView> GetCompanyListForOldService(SysEntities db, string companyName, int? userID_ZR, int branchID)
        {
            List<CRM_CompanyView> list = new List<CRM_CompanyView>();
            using (db)
            {
                var query = from a in db.CRM_Company
                            join b in db.CRM_CompanyToBranch on a.ID equals b.CRM_Company_ID
                            join c in db.ORG_User on b.UserID_ZR equals c.ID
                            where b.BranchID == branchID &&
                                  a.CompanyName.Contains(companyName) &&
                                  (userID_ZR == null || b.UserID_ZR == userID_ZR)
                            select new CRM_CompanyView()
                            {
                                ID = b.ID,
                                CRM_Company_ID = a.ID,
                                CompanyCode = a.CompanyCode,
                                CompanyName = a.CompanyName,
                                UserID_XS = b.UserID_XS,
                                UserID_XS_Name = c.RName,
                                OperateStatus = a.OperateStatus,
                                CreateTime = a.CreateTime,
                                UserID_ZR = b.UserID_ZR,
                                UserID_ZR_Name = c.RName
                            };
                list = query.OrderBy(o => o.CRM_Company_ID).ToList();
            }

            return list;
        }

        /// <summary>
        /// 根据企业名称查询已服务企业（分配客服用）【带权限】
        /// </summary>
        /// <param name="companyName">企业名称</param>
        /// <param name="branchID">所属分支机构</param>
        /// <returns></returns>
        public List<CRM_CompanyView> GetCompanyListForOldService(SysEntities db, string companyName, int? userID_ZR, int branchID, int departmentScope, int userID, int userDepartmentID, string departments)
        {
            List<CRM_CompanyView> list = new List<CRM_CompanyView>();
            using (db)
            {
                var query = from a in db.CRM_Company
                            join b in db.CRM_CompanyToBranch on a.ID equals b.CRM_Company_ID

                            join d in db.ORG_User on b.UserID_ZR equals d.ID into tmpzr
                            from zr in tmpzr.DefaultIfEmpty()
                            join c in db.ORG_User on b.UserID_XS equals c.ID into tmpxs
                            from xs in tmpxs.DefaultIfEmpty()
                            where a.CompanyName.Contains(companyName) &&
                                  (userID_ZR == null || b.UserID_ZR == userID_ZR) &&
                                  b.UserID_ZR != null
                            select new CRM_CompanyView()
                            {
                                ID = b.ID,
                                CRM_Company_ID = a.ID,
                                CompanyCode = a.CompanyCode,
                                CompanyName = a.CompanyName,
                                UserID_XS = b.UserID_XS,
                                UserID_XS_Name = xs.RName,
                                OperateStatus = a.OperateStatus,
                                CreateTime = a.CreateTime,
                                UserID_ZR = b.UserID_ZR,
                                UserID_ZR_Name = zr.RName
                            };
                if (departmentScope == (int)DepartmentScopeAuthority.无限制)//无限制
                {
                    if (!string.IsNullOrEmpty(departments))
                    {
                        var people = from a in db.ORG_User
                                     where departments.Split(',').Contains(a.ORG_Department_ID.ToString())
                                     select a;

                        query = from a in query join c in people on a.UserID_ZR equals c.ID select a;
                    }
                }
                else if (departmentScope == (int)DepartmentScopeAuthority.本机构及下属机构)//本机构及下属机构
                {
                    //当前用户直属机构

                    //查询本机构及下属机构所有部门数据
                    var branch = db.ORG_Department.FirstOrDefault(o => o.ID == branchID);

                    var people = from a in db.ORG_User
                                 join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                                 where a.XYBZ == "Y" && b.XYBZ == "Y" && b.LeftValue >= branch.LeftValue && b.RightValue <= branch.RightValue
                                 select a;

                    query = from cc in query
                            join f in people on cc.UserID_ZR equals f.ID
                            select cc;

                }
                else if (departmentScope == (int)DepartmentScopeAuthority.本机构) //本机构
                {
                    //查询本机构所有部门数据

                    var people = from a in db.ORG_User
                                 join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                                 where a.XYBZ == "Y" && b.XYBZ == "Y" && b.BranchID == branchID
                                 select a;
                    query = (from cc in query
                             join f in people on cc.UserID_ZR equals f.ID
                             select cc);

                }
                else if (departmentScope == (int)DepartmentScopeAuthority.本部门及其下属部门)//本部门及其下属部门
                {
                    //当前用户所属部门
                    ORG_Department department = db.ORG_Department.FirstOrDefault(o => o.ID == userDepartmentID);
                    //查询本部门及下属部门所有部门数据
                    var people = from a in db.ORG_User
                                 join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                                 where a.XYBZ == "Y" && b.XYBZ == "Y" && b.LeftValue >= department.LeftValue && b.RightValue <= department.RightValue
                                 select a;
                    query = (from cc in query
                             join f in people on cc.UserID_ZR equals f.ID
                             select cc);
                }
                else if (departmentScope == (int)DepartmentScopeAuthority.本部门) //本部门
                {
                    //查询本部门所有用户数据
                    var people = from a in db.ORG_User.Where(o => o.XYBZ == "Y")
                                 join b in db.ORG_Department.Where(o => o.XYBZ == "Y" && o.BranchID == userDepartmentID) on a.ORG_Department_ID equals b.ID
                                 select a;
                    query = (from cc in query
                             join f in people on cc.UserID_ZR equals f.ID
                             select cc);
                }
                else if (departmentScope == (int)DepartmentScopeAuthority.本人) //本人
                {
                    query = (from cc in query
                             where cc.UserID_ZR == userID
                             select cc);
                }
                list = query.OrderBy(o => o.CRM_Company_ID).ToList();
            }

            return list;
        }

        /// <summary>
        /// 根据企业名称责任客服查询企业（客服修改企业信息用）
        /// </summary>
        /// <param name="companyName">企业名称</param>
        /// <param name="branchID">所属分支机构</param>
        /// <returns></returns>
        public List<CRM_CompanyView> GetCompanyListForServiceEdit(SysEntities db, string companyName, int? userID_ZR, int branchID)
        {
            List<CRM_CompanyView> list = new List<CRM_CompanyView>();
            using (db)
            {
                var query = from a in db.CRM_Company
                            join b in db.CRM_CompanyToBranch on a.ID equals b.CRM_Company_ID
                            join c in db.ORG_User on b.UserID_ZR equals c.ID
                            where b.BranchID == branchID &&
                                  a.CompanyName.Contains(companyName) &&
                                  b.UserID_ZR == userID_ZR
                            select new CRM_CompanyView()
                            {
                                ID = b.ID,
                                CRM_Company_ID = a.ID,
                                CompanyCode = a.CompanyCode,
                                CompanyName = a.CompanyName,
                                UserID_XS = b.UserID_XS,
                                UserID_XS_Name = c.RName,
                                OperateStatus = a.OperateStatus,
                                CreateTime = a.CreateTime,
                                UserID_ZR = b.UserID_ZR,
                                UserID_ZR_Name = c.RName
                            };
                list = query.OrderBy(o => o.CRM_Company_ID).ToList();
            }

            return list;
        }

        /// <summary>
        /// 根据企业名称责任客服查询企业（客服修改企业信息用）【带权限】
        /// </summary>
        /// <param name="companyName">企业名称</param>
        /// <param name="branchID">所属分支机构</param>
        /// <returns></returns>
        public List<CRM_CompanyView> GetCompanyListForServiceEdit(SysEntities db, string companyName, int? userID_ZR, int branchID, int departmentScope, int userID, int userDepartmentID, string departments)
        {
            List<CRM_CompanyView> list = new List<CRM_CompanyView>();
            using (db)
            {
                var query = from a in db.CRM_Company
                            join b in db.CRM_CompanyToBranch on a.ID equals b.CRM_Company_ID
                            join d in db.ORG_User on b.UserID_ZR equals d.ID into tmpzr
                            from zr in tmpzr.DefaultIfEmpty()
                            join c in db.ORG_User on b.UserID_XS equals c.ID into tmpxs
                            from xs in tmpxs.DefaultIfEmpty()
                            where a.CompanyName.Contains(companyName) &&
                                 (userID_ZR == null || b.UserID_ZR == userID_ZR)
                            select new CRM_CompanyView()
                            {
                                ID = b.ID,
                                CRM_Company_ID = a.ID,
                                CompanyCode = a.CompanyCode,
                                CompanyName = a.CompanyName,
                                UserID_XS = b.UserID_XS,
                                UserID_XS_Name = xs.RName,
                                OperateStatus = a.OperateStatus,
                                CreateTime = a.CreateTime,
                                UserID_ZR = b.UserID_ZR,
                                UserID_ZR_Name = zr.RName
                            };
                var aa = query.ToList();
                if (departmentScope == (int)DepartmentScopeAuthority.无限制)//无限制
                {
                    if (!string.IsNullOrEmpty(departments))
                    {
                        var people = from a in db.ORG_User
                                     where departments.Split(',').Contains(a.ORG_Department_ID.ToString())
                                     select a;

                        //获取数据源中责任客服为以上取得员工列表的数据
                        query = from a in query
                                join b in people on a.UserID_ZR equals b.ID
                                select a;
                    }
                }
                else if (departmentScope == (int)DepartmentScopeAuthority.本机构及下属机构)//本机构及下属机构
                {
                    //当前用户直属机构
                    var branch = db.ORG_Department.FirstOrDefault(o => o.ID == branchID);

                    //查询本机构及下属机构所有部门数据
                    var people = from a in db.ORG_User
                                 join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                                 where a.XYBZ == "Y" && b.XYBZ == "Y" && b.LeftValue >= branch.LeftValue && b.RightValue <= branch.RightValue
                                 select a;

                    query = from cc in query
                            join f in people on cc.UserID_ZR equals f.ID
                            select cc;

                }
                else if (departmentScope == (int)DepartmentScopeAuthority.本机构) //本机构
                {
                    //查询本机构所有部门数据
                    var people = from a in db.ORG_User
                                 join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                                 where a.XYBZ == "Y" && b.XYBZ == "Y" && b.BranchID == branchID
                                 select a;
                    query = (from cc in query
                             join f in people on cc.UserID_ZR equals f.ID
                             select cc);

                }
                else if (departmentScope == (int)DepartmentScopeAuthority.本部门及其下属部门)//本部门及其下属部门
                {
                    //当前用户所属部门
                    ORG_Department department = db.ORG_Department.FirstOrDefault(o => o.ID == userDepartmentID);
                    //查询本部门及下属部门所有部门数据
                    var people = from a in db.ORG_User
                                 join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                                 where a.XYBZ == "Y" && b.XYBZ == "Y" && b.LeftValue >= department.LeftValue && b.RightValue <= department.RightValue
                                 select a;
                    query = (from cc in query
                             join f in people on cc.UserID_ZR equals f.ID
                             select cc);
                }
                else if (departmentScope == (int)DepartmentScopeAuthority.本部门) //本部门
                {
                    //查询本部门所有用户数据
                    var people = from a in db.ORG_User.Where(o => o.XYBZ == "Y")
                                 join b in db.ORG_Department.Where(o => o.XYBZ == "Y" && o.BranchID == userDepartmentID) on a.ORG_Department_ID equals b.ID
                                 select a;
                    query = (from cc in query
                             join f in people on cc.UserID_ZR equals f.ID
                             select cc);
                }
                else if (departmentScope == (int)DepartmentScopeAuthority.本人) //本人
                {
                    query = (from cc in query
                             where cc.UserID_ZR == userID
                             select cc);
                }
                list = query.OrderBy(o => o.CRM_Company_ID).ToList();
            }
            return list;
        }
        #endregion

        #region zhanghui

        /// <summary>
        /// 根据社保种类ID缴纳地获取政策内容
        /// </summary>
        /// <param name="InsuranceKindId">枚举值</param>
        /// <param name="CityID"></param>
        /// <returns></returns>
        public dynamic GetZCByID(int InsuranceKindId, string CityID)
        {
            using (SysEntities db = new SysEntities())
            {
                string state = Common.Status.启用.ToString();
                var list = (from a in db.InsuranceKind
                            join b in db.PoliceInsurance on a.Id equals b.InsuranceKindId
                            where a.InsuranceKindId == InsuranceKindId && a.City == CityID && b.State == state
                            select new
                            {
                                Sort = a.InsuranceKindId,
                                ID = b.Id,
                                Name = b.Name,
                            }).OrderBy(c => c.Sort).ToList();
                return list;
            }
        }
        #endregion

        #region 内置
        //得到联系人信息
        public List<CRM_CompanyLinkMan> GetLinkList(List<CRM_CompanyLinkMan> listLink, int companyID)
        {
            for (int i = 0; i < listLink.Count(); i++)
            {
                listLink[i].CRM_Company_ID = companyID;
            }
            return listLink;
        }
        //得到银行信息
        public List<CRM_CompanyBankAccount> GetBankList(List<CRM_CompanyBankAccount> listBank, int companyID)
        {
            for (int i = 0; i < listBank.Count(); i++)
            {
                listBank[i].CRM_Company_ID = companyID;
            }
            return listBank;
        }
        //得到开票信息
        public List<CRM_CompanyFinance_Bill> GetBillList(List<CRM_CompanyFinance_Bill> listBill, int companyID)
        {
            for (int i = 0; i < listBill.Count(); i++)
            {
                listBill[i].CRM_Company_ID = companyID;
            }
            return listBill;
        }
        //得到回款信息
        public List<CRM_CompanyFinance_Payment> GetPayList(List<CRM_CompanyFinance_Payment> listPay, int companyID)
        {
            for (int i = 0; i < listPay.Count(); i++)
            {
                listPay[i].CRM_Company_ID = companyID;
            }
            return listPay;
        }
        //得到报价信息
        public List<CRM_CompanyPrice> GetPriceList(List<CRM_CompanyPrice> listPrice, int companyID)
        {
            for (int i = 0; i < listPrice.Count(); i++)
            {
                listPrice[i].CRM_Company_ID = companyID;
            }
            return listPrice;
        }
        //得到阶梯报价信息
        public List<CRM_CompanyLadderPrice> GetLadderList(List<CRM_CompanyLadderPrice> listLadderPrice, int companyID)
        {
            for (int i = 0; i < listLadderPrice.Count(); i++)
            {
                listLadderPrice[i].CRM_Company_ID = companyID;
            }
            return listLadderPrice;
        }
        //得到社保政策
        public List<CRM_Company_Insurance> GetInsuranceList(List<CRM_Company_Insurance> listInsurance, int companyID)
        {
            for (int i = 0; i < listInsurance.Count(); i++)
            {
                listInsurance[i].CRM_Company_ID = companyID;
            }
            return listInsurance;
        }
        //得到社保信息
        public List<CRM_Company_PoliceInsurance> GetPoliceInsuranceList(List<CRM_Company_PoliceInsurance> listPoliceInsurance, int companyID)
        {
            for (int i = 0; i < listPoliceInsurance.Count(); i++)
            {
                listPoliceInsurance[i].CRM_Company_ID = companyID;
            }
            return listPoliceInsurance;
        }
        #endregion

        /// <summary>
        /// 根据企业名称获取企业ID（全字符匹配）
        /// </summary>
        /// <param name="db"></param>
        /// <param name="companyName">企业名称</param>
        /// <returns>返回企业ID，如果没查到返回0</returns>
        public int GetCompanyIdByName(SysEntities db, string companyName)
        {
            int companyId = 0;
            if (!string.IsNullOrEmpty(companyName))
            {
                var model = db.CRM_Company.FirstOrDefault(o => o.CompanyName == companyName);
                if (model != null)
                {
                    companyId = model.ID;
                }
            }
            return companyId;
        }
        /// <summary>
        /// 判断责任客服是否负责该企业
        /// </summary>
        /// <param name="db"></param>
        /// <param name="companyId">企业ID </param>
        /// <param name="userId_ZR">责任客服ID</param>
        /// <returns></returns>
        public bool IsZRUserHaveCompany(SysEntities db, int companyId, int userId_ZR)
        {
            bool result = false;
            var query = db.CRM_CompanyToBranch.Where(o => o.CRM_Company_ID == companyId && o.UserID_ZR == userId_ZR);
            if (query.Any())
            {
                result = query.OrderBy(o => o.ID).First().ID > 0;
            }
            return result;
        }
        #region 获得该企业本月缴纳社保人数
        public int getEmployee(SysEntities db, int? CRM_Company_ID, int? YearMonuth)
        {
            string State = Common.Status.启用.ToString();
            var trueData = from a in db.EmployeeMiddle.Where(em => em.StartDate <= YearMonuth && em.EndedDate >= YearMonuth && em.State == State)
                           join b in db.CompanyEmployeeRelation.Where(x => x.CompanyId == CRM_Company_ID) on a.CompanyEmployeeRelationId equals b.Id
                           where a.PaymentStyle == (int)Common.EmployeeMiddle_PaymentStyle.正常 || a.PaymentStyle == (int)Common.EmployeeMiddle_PaymentStyle.补缴
                           group new { b } by new
                           {
                               EmployeeID = b.EmployeeId
                           }
                               into s

                               select new
                               {
                                   EmpID = s.Key.EmployeeID
                               };

            return trueData.Count();


        }
        public int[] getEmployeeIDs(SysEntities db, int? CRM_Company_ID, int? YearMonuth)
        {

            string State = Common.Status.启用.ToString();
            var trueData = from a in db.EmployeeMiddle.Where(em => em.StartDate <= YearMonuth && em.EndedDate >= YearMonuth && em.State == State)
                           join b in db.CompanyEmployeeRelation.Where(x => x.CompanyId == CRM_Company_ID) on a.CompanyEmployeeRelationId equals b.Id
                           where a.PaymentStyle == (int)Common.EmployeeMiddle_PaymentStyle.正常 || a.PaymentStyle == (int)Common.EmployeeMiddle_PaymentStyle.补缴
                           group new { b } by new
                           {
                               EmployeeID = b.EmployeeId
                           }
                               into s

                               select new
                               {
                                   EmpID = s.Key.EmployeeID
                               };

            int i = 0;
            int[] r = new int[trueData.Count()];
            foreach (var li in trueData)
            {
                r[i] = li.EmpID ?? 0;
                i++;

            }
            return r;

        }

        #endregion

    }

}
