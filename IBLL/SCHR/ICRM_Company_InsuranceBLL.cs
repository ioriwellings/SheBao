using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using Langben.DAL;
using System.ServiceModel;
using Langben.DAL.Model;

namespace Langben.IBLL
{
    /// <summary>
    /// 客户_社保信息 接口
    /// </summary>
    public partial interface ICRM_Company_InsuranceBLL
    {
        /// <summary>
        /// 根据企业ID获取企业社保信息
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地（可为空，为空时表示查询该企业所有缴纳的的社保信息）</param>
        /// <returns></returns>
        List<CompanyInsurance> GetCRM_Company_Insurance(int companyID, string cityID = "");

        /// <summary>
        /// 创建企业社保信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="model">社保信息实体</param>
        /// <param name="userID">操作人ID</param>
        /// <param name="userName">操作人姓名</param>
        /// <param name="branchID">所属分支机构</param>
        /// <returns></returns>
        bool CreateCRM_Company_Insurance(CompanyInsurance model, int userID, string userName, int branchID);


        /// <summary>
        /// 检查当前企业在当前缴纳地是否存在社保信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地ID</param>
        /// <returns></returns>
        int CheckCompanyCity(int companyID, string cityID);

        /// <summary>
        /// 根据企业和缴纳地获取企业社保信息
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <returns></returns>
        CompanyInsurance_EditView GetByCompanyCity(int companyID, string cityID);

        /// <summary>
        /// 修改企业社保信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="model">社保信息实体</param>
        /// <param name="userID">操作人ID</param>
        /// <param name="userName">操作人姓名</param>
        /// <param name="branchID">所属分支机构</param>
        /// <returns></returns>
        bool UpdateCRM_Company_Insurance(CompanyInsurance model, int userID, string userName, int branchID);

        /// <summary>
        /// 获取待审核的新建社保信息
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地</param>
        /// <returns></returns>
        CompanyInsurance GetAddData(int companyID, string cityID);

        /// <summary>
        /// 获取待审核的修改社保信息
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地</param>
        /// <returns></returns>
        CompanyInsurance GetEditData(int companyID, string cityID);

        /// <summary>
        /// 停用企业社保信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地</param>
        /// <param name="state">启用状态</param>
        /// <param name="userName">操作人姓名</param>
        /// <returns></returns>
        bool ChangeInsuranceState(int companyID, string cityID, string state, string userName);

        /// <summary>
        /// 审核通过新建数据
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地ID</param>
        /// <returns></returns>
        bool PassAdd(int companyID, string cityID);
        /// <summary>
        /// 审核退回新建数据
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地ID</param>
        /// <returns></returns>
        bool ReturnAdd(int companyID, string cityID);
        /// <summary>
        /// 审核通过修改数据
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地ID</param>
        /// <returns></returns>
        bool PassEdit(int companyID, string cityID);
        /// <summary>
        /// 审核退回修改数据
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地ID</param>
        /// <returns></returns>
        bool ReturnEdit(int companyID, string cityID);
    }
}

