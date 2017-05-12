using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using Langben.DAL;
using System.ServiceModel;
using Langben.DAL.Model;

namespace Langben.IBLL
{
    public partial interface ICRM_CompanyBLL
    {
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
        List<CRM_CompanyView> GetCompanyListForSales(int? id, int page, int rows, string companyName, int? userID_XS, int branchID, ref int total);

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
        List<CRM_CompanyView> GetCompanyListForSales(int? id, int page, int rows, string companyName, int? userID_XS, int branchID, string menuID, int departmentScope, int userID, int userDepartmentID, string departments, ref int total);
        
        /// <summary>
        /// 根据责任客服查询企业(ID,CompanyName)
        /// </summary>
        /// <param name="zrUserID">责任客服编号</param>
        /// <returns></returns>
        List<CRM_Company> GetCompanyDateForZrUser(int zrUserID);
     /// <summary>
        /// 根据员工客服查询企业(ID,CompanyName)
     /// </summary>
     /// <param name="YGUserID">员工客服编号</param>
     /// <returns></returns>
        List<CRM_Company> GetCompanyDateForYGUser(int YGUserID);
        // 企业基本信息
         string GetCompanyBase(int companyID);
         //创建新公司
         //bool CreateNewCompany(ref Common.ValidationErrors validationErrors, CRM_Company baseModel, CRM_CompanyContract contractModel, CRM_CompanyToBranch branchModel, List<CRM_CompanyLinkMan> listLink, List<CRM_CompanyBankAccount> listBank, List<CRM_CompanyFinance_Bill> listBill, List<CRM_CompanyFinance_Payment> listPay, List<CRM_CompanyPrice> listPrice, List<CRM_CompanyLadderPrice> listLadderPrice);
         //创建新公司
         bool CreateNewCompany(ref Common.ValidationErrors validationErrors, CRM_Company baseModel, CRM_CompanyContract contractModel, CRM_CompanyToBranch branchModel, List<CRM_CompanyLinkMan> listLink, List<CRM_CompanyBankAccount> listBank, List<CRM_CompanyFinance_Bill> listBill, List<CRM_CompanyFinance_Payment> listPay, List<CRM_CompanyPrice> listPrice, List<CRM_CompanyLadderPrice> listLadderPrice, List<CRM_Company_PoliceInsurance> CompanyPoliceInsurance, List<CRM_Company_Insurance> CompanyInsurance);
         //验证公司名称唯一
         int CheckCompanyName(string companyID, string CompanyName);
         //验证开票名称名称唯一
         int CheckTaxNumber(string companyID, string taxName);
         dynamic GetZCByID(int InsuranceKindId, string CityID);
   

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
         List<CRM_CompanyView> GetCompanyListForNewService(int? id, int page, int rows, string companyName, int? userID_ZR, int branchID, ref int total);

                /// <summary>
        /// 根据企业名称查询企业（新分配客服用）【带权限】
        /// </summary>
        /// <param name="companyName">企业名称</param>
        /// <param name="branchID">所属分支机构</param>
        /// <returns></returns>
         List<CRM_CompanyView> GetCompanyListForNewService(int? id, int page, int rows, string companyName, int? userID_ZR, int branchID, int departmentScope, int userID, int userDepartmentID, string departments, ref int total);


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
        List<CRM_CompanyView> GetCompanyListForOldService(int? id, int page, int rows, string companyName, int? userID_ZR, int branchID, ref int total);

        /// <summary>
        /// 根据企业名称查询已服务企业（分配客服用）【带权限】
        /// </summary>
        /// <param name="companyName">企业名称</param>
        /// <param name="branchID">所属分支机构</param>
        /// <returns></returns>
        List<CRM_CompanyView> GetCompanyListForOldService(int? id, int page, int rows, string companyName, int? userID_ZR, int branchID,int departmentScope, int userID, int userDepartmentID, string departments, ref int total);

        /// <summary>
        /// 根据企业名称责任客服查询企业（客服修改企业信息用）
        /// </summary>
        /// <param name="companyName">企业名称</param>
        /// <param name="branchID">所属分支机构</param>
        /// <returns></returns>
        List<CRM_CompanyView> GetCompanyListForServiceEdit(int? id, int page, int rows, string companyName, int? userID_ZR, int branchID, ref int total);

        /// <summary>
        /// 根据企业名称责任客服查询企业（客服修改企业信息用）【带权限】
        /// </summary>
        /// <param name="companyName">企业名称</param>
        /// <param name="branchID">所属分支机构</param>
        /// <returns></returns>
        List<CRM_CompanyView> GetCompanyListForServiceEdit(int? id, int page, int rows, string companyName, int? userID_ZR, int branchID, int departmentScope, int userID, int userDepartmentID, string departments, ref int total);
        
        /// <summary>
        /// 获取所有的公司
        /// </summary>
        /// <returns></returns>
        List<CRM_Company> GetCompanyList();
        #endregion
    }
}

