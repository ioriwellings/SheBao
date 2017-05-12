using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Langben.DAL;
using Common;
using Langben.DAL.Model;

namespace Langben.BLL
{
    public partial class CRM_Company_AuditBLL : IBLL.ICRM_Company_AuditBLL, IDisposable
    {
        #region 企业基本信息
        /// <summary>
        /// 修改企业基本信息
        /// </summary>
        /// <param name="entity">企业基本信息</param>
        /// <returns></returns>
        public bool ModifyBaseInfo(ref ValidationErrors validationErrors, CRM_Company_Audit entity)
        {
            try
            {
                repository.ModifyBaseInfo(entity);
                return true;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
                return false;
            }
        }
        #endregion

        #region 待审核企业列表
        public Common.ClientResult.DataResult GetAuditCompanyList(int page, int rows, string companyName, string operateStatus)
        {
            var queryData = repository.GetAuditCompanyList(page, rows, companyName, operateStatus);

            return queryData;
        }
        #endregion

        /// <summary>
        /// 需重新提交的企业列表
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="companyName">企业名称</param>
        /// <param name="userID_XS">销售人员ID</param>
        /// <param name="branchID">分支机构ID</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        public List<CRM_Company_Audit> GetCompany_AuditListForReSubmit(int? id, int page, int rows, string companyName, int userID_XS, int branchID, ref int total)
        {
            List<CRM_Company_Audit> queryData = repository.GetCompany_AuditListForReSubmit(db, companyName, userID_XS, branchID);
            List<CRM_Company_Audit> queryList = new List<CRM_Company_Audit>();
            total = queryData.Count();
            if (total > 0)
            {
                if (page <= 1)
                {
                    queryList = queryData.Take(rows).ToList();
                }
                else
                {
                    queryList = queryData.Skip((page - 1) * rows).Take(rows).ToList();
                }

            }
            return queryList;
        }

        /// <summary>
        /// 企业基本信息待审核信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public string GetCompanyBaseAudit(int ID)
        {
            string jsonData = repository.GetCompanyBaseAudit(ID);
            return jsonData;
        }


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
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult GetAuditCompanyListForQuality(int page, int rows, string companyName, int? UserID_ZR, int? UserID_XS, int? auditType, int? operateStatus, string companyCode, int departmentScope, string departments, int branchID, int departmentID, int userID)
        {
            using (db)
            {
                List<CRM_CompanyAuditView> queryList = new List<CRM_CompanyAuditView>();
                var query = repository.GetAuditCompanyListForQuality(db, companyName, UserID_ZR, UserID_XS, auditType, operateStatus, companyCode);

                if (departmentScope == (int)DepartmentScopeAuthority.无限制)//无限制
                {
                    if (!string.IsNullOrEmpty(departments))
                    {
                        var people = from a in db.ORG_User
                                     where departments.Split(',').Contains(a.ORG_Department_ID.ToString())
                                     select a;

                        query = (from a in query join b in people on a.CreateUserID equals b.ID select a).Union(
                                   from a in query join c in people on a.UserID_XS equals c.ID select a).Union(
                                   from a in query join c in people on a.UserID_ZR equals c.ID select a);
                    }
                }
                else if (departmentScope == (int)DepartmentScopeAuthority.本机构及下属机构)//本机构及下属机构
                {
                    //查询本机构及下属机构所有部门数据
                    var branch = db.ORG_Department.FirstOrDefault(o => o.ID == branchID);

                    query = from a in query
                            join b in db.ORG_Department on a.BranchID equals b.ID
                            where b.XYBZ == "Y" && b.LeftValue >= branch.LeftValue && b.RightValue <= branch.RightValue
                            select a;
                }
                else if (departmentScope == (int)DepartmentScopeAuthority.本机构) //本机构
                {
                    query = from a in query
                            where a.BranchID == branchID
                            select a;
                }
                else if (departmentScope == (int)DepartmentScopeAuthority.本部门及其下属部门)//本部门及其下属部门
                {
                    //当前用户所属部门
                    ORG_Department department = db.ORG_Department.FirstOrDefault(o => o.ID == departmentID);

                    //查询本部门及下属部门所有部门数据
                    var people = from a in db.ORG_User
                                 join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                                 where a.XYBZ == "Y" && b.XYBZ == "Y" && b.LeftValue >= department.LeftValue && b.RightValue <= department.RightValue
                                 select a;

                    query = (from a in query join b in people on a.CreateUserID equals b.ID select a).Union(
                             from a in query join c in people on a.UserID_XS equals c.ID select a).Union(
                             from a in query join c in people on a.UserID_ZR equals c.ID select a);

                }
                else if (departmentScope == (int)DepartmentScopeAuthority.本部门) //本部门
                {
                    //查询本部门所有用户数据
                    var people = from a in db.ORG_User.Where(o => o.XYBZ == "Y")
                                 join b in db.ORG_Department.Where(o => o.XYBZ == "Y" && o.ID == departmentID) on a.ORG_Department_ID equals b.ID
                                 select a;

                    query = (from a in query join b in people on a.CreateUserID equals b.ID select a).Union(
                             from a in query join c in people on a.UserID_XS equals c.ID select a).Union(
                             from a in query join c in people on a.UserID_ZR equals c.ID select a);

                }
                else if (departmentScope == (int)DepartmentScopeAuthority.本人) //本人
                {
                    query = query.Where(c => c.CreateUserID == userID || c.UserID_XS == userID || c.UserID_ZR == userID);
                }

                if (query.Count() > 0)
                {
                    query = from a in query
                            join b in db.ORG_User on a.UserID_ZR equals b.ID into tmpzr
                            from zr in tmpzr.DefaultIfEmpty()
                            join c in db.ORG_User on a.UserID_XS equals c.ID into tmpxs
                            from xs in tmpxs.DefaultIfEmpty()
                            orderby a.CreateTime descending
                            select new CRM_CompanyAuditView
                            {
                                AuditType = a.AuditType,
                                AuditTypeName = a.AuditTypeName,
                                ID = a.ID,
                                MainTableID = a.MainTableID,
                                CityID = a.CityID,
                                CompanyID = a.CompanyID,
                                CompanyCode = a.CompanyCode,
                                CompanyName = a.CompanyName,
                                UserID_ZR = a.UserID_ZR,
                                UserID_XS = a.UserID_XS,
                                CreateUserID = a.CreateUserID,
                                CreateUserName = a.CreateUserName,
                                CreateTime = a.CreateTime,
                                OperateStatus = a.OperateStatus,
                                BranchID = a.BranchID,
                                UserID_XS_Name = xs.RName ?? "",
                                UserID_ZR_Name = zr.RName ?? ""
                            };


                    if (page <= 1)
                    {
                        queryList = query.Take(rows).ToList();
                    }
                    else
                    {
                        queryList = query.Skip((page - 1) * rows).Take(rows).ToList();
                    }

                }

                var data = new Common.ClientResult.DataResult
                {
                    total = query.Count(),
                    rows = queryList
                };

                return data;
            }

        }


        /// <summary>
        /// 退回基本信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        public bool ReturnBaseEdit(ValidationErrors validationErrors, int ID, int mainTableID)
        {
            try
            {
                repository.ReturnBaseEdit(ID, mainTableID);
                return true;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
                return false;
            }
        }

        /// <summary>
        /// 提交企业基本信息修改审核
        /// </summary>
        /// <param name="ID">审核表ID</param>
        /// <param name="mainTableID">原表ID</param>
        /// <returns></returns>
        public bool PassBaseEdit(ValidationErrors validationErrors, int ID, int mainTableID)
        {
            try
            {
                repository.PassBaseEdit(ID, mainTableID);
                return true;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
                return false;
            }
        }
    }
}
