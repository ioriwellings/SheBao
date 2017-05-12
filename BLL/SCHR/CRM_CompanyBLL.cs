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
    /// <summary>
    /// 客户_企业信息 
    /// </summary>
    public partial class CRM_CompanyBLL : IBLL.ICRM_CompanyBLL, IDisposable
    {
        #region 马英杰
        /// <summary>
        /// 根据企业名称查询企业（销售用）
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="companyName">企业名称</param>
        /// <param name="userID_XS">销售人员ID</param>
        /// <param name="branchID">分支机构ID</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        public List<CRM_CompanyView> GetCompanyListForSales(int? id, int page, int rows, string companyName, int? userID_XS, int branchID, ref int total)
        {
            var queryData = repository.GetCompanyListForSales(db, companyName, userID_XS, branchID);
            List<CRM_CompanyView> queryList = new List<CRM_CompanyView>();
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
        /// 根据企业名称查询企业（销售用）【带权限】
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="companyName">企业名称</param>
        /// <param name="userID_XS">销售人员ID</param>
        /// <param name="branchID">分支机构ID</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        public List<CRM_CompanyView> GetCompanyListForSales(int? id, int page, int rows, string companyName, int? userID_XS, int branchID, string menuID, int departmentScope, int userID, int userDepartmentID,string departments, ref int total)
        {
                var queryData = repository.GetCompanyListForSales(db, companyName, userID_XS, branchID, menuID, departmentScope, userID, userDepartmentID, departments);
            List<CRM_CompanyView> queryList = new List<CRM_CompanyView>();
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
        #endregion

        #region 王骁健
        /// <summary>
        /// 得到企业基本信息
        /// </summary>
        /// <param name="companyID">公司id</param>
        /// <returns></returns>
       public string GetCompanyBase(int companyID)
        {
            string jsonData = repository.GetCompanyBase(companyID);
            return jsonData;
        }
        
        /// <summary>
       ///  创建新公司
        /// </summary>
        /// <param name="validationErrors"></param>
        /// <param name="baseModel">基本信息表</param>
        /// <param name="contractModel">合同信息表</param>
        /// <param name="branchModel">公司分支关系</param>
        /// <param name="listLink">联系人信息</param>
        /// <param name="listBank">银行信息</param>
        /// <param name="listBill">开票信息</param>
        /// <param name="listPay">回款信息</param>
        /// <param name="listPrice">报价</param>
        /// <param name="listLadderPrice">阶梯报价</param>
        /// <returns></returns>
       public bool CreateNewCompany(ref ValidationErrors validationErrors, CRM_Company baseModel, CRM_CompanyContract contractModel, CRM_CompanyToBranch branchModel, List<CRM_CompanyLinkMan> listLink, List<CRM_CompanyBankAccount> listBank, List<CRM_CompanyFinance_Bill> listBill, List<CRM_CompanyFinance_Payment> listPay, List<CRM_CompanyPrice> listPrice, List<CRM_CompanyLadderPrice> listLadderPrice, List<CRM_Company_PoliceInsurance> CompanyPoliceInsurance, List<CRM_Company_Insurance> CompanyInsurance)
       {
           try
           {
               int result = repository.CreateNewCompany(baseModel, contractModel, branchModel, listLink, listBank, listBill, listPay, listPrice, listLadderPrice, CompanyPoliceInsurance, CompanyInsurance);
               if (result==1)
                   return true;
               else
                   return false;
           }
           catch (Exception ex)
           {
               validationErrors.Add(ex.Message);
               ExceptionsHander.WriteExceptions(ex);
               return false;
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
            int count=repository.CheckCompanyName(companyID, companyName);
            return count;
       }
        /// <summary>
        /// 验证开票名称唯一
        /// </summary>
        /// <param name="companyID"></param>
        /// <param name="billName"></param>
        /// <returns></returns>
       public int CheckTaxNumber(string companyID, string taxNumber)
       {
           int count = repository.CheckTaxNumber(companyID, taxNumber);
           return count;
       }
        #endregion

        #region ainan

        /// <summary>
        /// 根据责任客服查询企业(ID,CompanyName)
        /// </summary>
        /// <param name="zrUserID">责任客服编号</param>
        /// <returns></returns>
        public List<CRM_Company> GetCompanyDateForZrUser(int zrUserID)
        {
            List<CRM_Company> queryData = repository.GetCompanyDateForZrUser(zrUserID);
            return queryData;
        }
        #endregion

        #region qiaoweilin

        /// <summary>
        /// 根据员工客服查询企业(ID,CompanyName)
        /// </summary>
        /// <param name="zrUserID">员工客服编号</param>
        /// <returns></returns>
        public List<CRM_Company> GetCompanyDateForYGUser(int zrUserID)
        {
            List<CRM_Company> queryData = repository.GetCompanyDateForYGUser(zrUserID);
            return queryData;
        }
        #endregion

        #region 信伟青
        /// <summary>
        /// 根据企业名称查询企业（新分配客服用）
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="companyName">企业名称</param>
        /// <param name="userID_ZR">责任客服ID</param>
        /// <param name="branchID">分支机构ID</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        public List<CRM_CompanyView> GetCompanyListForNewService(int? id, int page, int rows, string companyName, int? userID_ZR, int branchID, ref int total)
        {
            List<CRM_CompanyView> queryData = repository.GetCompanyListForNewService(db, companyName, userID_ZR, branchID);
            List<CRM_CompanyView> queryList = new List<CRM_CompanyView>();
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
        /// 根据企业名称查询企业（新分配客服用）【带权限】
        /// </summary>
        /// <param name="companyName">企业名称</param>
        /// <param name="branchID">所属分支机构</param>
        /// <returns></returns>
        public List<CRM_CompanyView> GetCompanyListForNewService(int? id, int page, int rows, string companyName, int? userID_ZR, int branchID, int departmentScope, int userID, int userDepartmentID, string departments, ref int total)
        {
            List<CRM_CompanyView> queryData = repository.GetCompanyListForNewService(db, companyName, userID_ZR, branchID, departmentScope, userID, userDepartmentID, departments);
            List<CRM_CompanyView> queryList = new List<CRM_CompanyView>();
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
        /// 根据企业名称查询已服务企业（分配客服用）
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="companyName">企业名称</param>
        /// <param name="userID_ZR">责任客服ID</param>
        /// <param name="branchID">分支机构ID</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        public List<CRM_CompanyView> GetCompanyListForOldService(int? id, int page, int rows, string companyName, int? userID_ZR, int branchID, ref int total)
        {
            List<CRM_CompanyView> queryData = repository.GetCompanyListForOldService(db, companyName, userID_ZR, branchID);
            List<CRM_CompanyView> queryList = new List<CRM_CompanyView>();
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
        /// 根据企业名称查询已服务企业（分配客服用）【带权限】
        /// </summary>
        /// <param name="companyName">企业名称</param>
        /// <param name="branchID">所属分支机构</param>
        /// <returns></returns>
        public List<CRM_CompanyView> GetCompanyListForOldService(int? id, int page, int rows, string companyName, int? userID_ZR, int branchID, int departmentScope, int userID, int userDepartmentID, string departments, ref int total)
        {
            List<CRM_CompanyView> queryData = repository.GetCompanyListForOldService(db, companyName, userID_ZR, branchID, departmentScope, userID, userDepartmentID, departments);
            List<CRM_CompanyView> queryList = new List<CRM_CompanyView>();
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
        /// 根据企业名称责任客服查询企业（客服修改企业信息用）
        /// </summary>
        /// <param name="companyName">企业名称</param>
        /// <param name="branchID">所属分支机构</param>
        /// <returns></returns>
        public List<CRM_CompanyView> GetCompanyListForServiceEdit(int? id, int page, int rows, string companyName, int? userID_ZR, int branchID, ref int total)
        {
            List<CRM_CompanyView> queryData = repository.GetCompanyListForServiceEdit(db, companyName, userID_ZR, branchID);
            List<CRM_CompanyView> queryList = new List<CRM_CompanyView>();
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
        /// 根据企业名称责任客服查询企业（客服修改企业信息用）【带权限】
        /// </summary>
        /// <param name="companyName">企业名称</param>
        /// <param name="branchID">所属分支机构</param>
        /// <returns></returns>
        public List<CRM_CompanyView> GetCompanyListForServiceEdit(int? id, int page, int rows, string companyName, int? userID_ZR, int branchID, int departmentScope, int userID, int userDepartmentID, string departments, ref int total)
        {
            List<CRM_CompanyView> queryData = repository.GetCompanyListForServiceEdit(db, companyName, userID_ZR, branchID,departmentScope, userID, userDepartmentID, departments);
            List<CRM_CompanyView> queryList = new List<CRM_CompanyView>();
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
        /// 获取所有的公司
        /// </summary>
        /// <returns></returns>
        public List<CRM_Company> GetCompanyList()
        {
            var queryData = repository.GetCompanyList(db);
            List<CRM_Company> queryList = new List<CRM_Company>();
            queryList = queryData.ToList();
            return queryList;       
        }
        #endregion
        #region zhanghui
        public dynamic GetZCByID(int InsuranceKindId, string CityID)
        {
            return repository.GetZCByID(InsuranceKindId, CityID);
        }
        #endregion
    }



}

