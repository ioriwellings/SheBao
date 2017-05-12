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
    /// 客户_社保政策 
    /// </summary>
    public partial class CRM_Company_InsuranceBLL : IBLL.ICRM_Company_InsuranceBLL, IDisposable
    {
        /// <summary>
        /// 根据企业ID获取企业社保信息
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地（可为空，为空时表示查询该企业所有缴纳的的社保信息）</param>
        /// <returns></returns>
        public List<CompanyInsurance> GetCRM_Company_Insurance(int companyID, string cityID)
        {
            var queryData = repository.GetCRM_Company_Insurance(db, companyID, cityID);

            return queryData;
        }

        /// <summary>
        /// 创建企业社保信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="model">社保信息实体</param>
        /// <param name="userID">操作人ID</param>
        /// <param name="userName">操作人姓名</param>
        /// <param name="branchID">所属分支机构</param>
        /// <returns></returns>
        public bool CreateCRM_Company_Insurance(CompanyInsurance model, int userID, string userName, int branchID)
        {
            return repository.CreateCRM_Company_Insurance(db, model, userID, userName, branchID);
        }

        /// <summary>
        /// 检查当前企业在当前缴纳地是否存在社保信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地ID</param>
        /// <returns></returns>
        public int CheckCompanyCity(int companyID, string cityID)
        {
            return repository.CheckCompanyCity(db, companyID, cityID);
        }

        /// <summary>
        /// 根据企业和缴纳地获取企业社保信息
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <returns></returns>
        public CompanyInsurance_EditView GetByCompanyCity(int companyID, string cityID)
        {
            return repository.GetByCompanyCity(db, companyID, cityID);
        }

        /// <summary>
        /// 修改企业社保信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="model">社保信息实体</param>
        /// <param name="userID">操作人ID</param>
        /// <param name="userName">操作人姓名</param>
        /// <param name="branchID">所属分支机构</param>
        /// <returns></returns>
        public bool UpdateCRM_Company_Insurance(CompanyInsurance model, int userID, string userName, int branchID)
        {
            return repository.UpdateCRM_Company_Insurance(db, model, userID, userName, branchID);
        }

        /// <summary>
        /// 获取待审核的新建社保信息
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地</param>
        /// <returns></returns>
        public CompanyInsurance GetAddData(int companyID, string cityID)
        {
            return repository.GetAddData(db, companyID, cityID);
        }

        /// <summary>
        /// 获取待审核的修改社保信息
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地</param>
        /// <returns></returns>
        public CompanyInsurance GetEditData(int companyID, string cityID)
        {
            return repository.GetEditData(db, companyID, cityID);
        }

        /// <summary>
        /// 停用企业社保信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地</param>
        /// <param name="state">启用状态</param>
        /// <param name="userName">操作人姓名</param>
        /// <returns></returns>
        public bool ChangeInsuranceState(int companyID, string cityID, string state, string userName)
        {
            return repository.ChangeInsuranceState(db, companyID, cityID, state, userName);
        }

        /// <summary>
        /// 审核通过新建数据
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地ID</param>
        /// <returns></returns>
        public bool PassAdd(int companyID, string cityID)
        {
            return repository.PassAdd(db, companyID, cityID);
        }
        /// <summary>
        /// 审核退回新建数据
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地ID</param>
        /// <returns></returns>
        public bool ReturnAdd(int companyID, string cityID)
        {
            return repository.ReturnAdd(db, companyID, cityID);
        }
        /// <summary>
        /// 审核通过修改数据
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地ID</param>
        /// <returns></returns>
        public bool PassEdit(int companyID, string cityID)
        {
            return repository.PassEdit(db, companyID, cityID);
        }
        /// <summary>
        /// 审核退回修改数据
        /// </summary>
        /// <param name="companyID">企业ID</param>
        /// <param name="cityID">缴纳地ID</param>
        /// <returns></returns>
        public bool ReturnEdit(int companyID, string cityID)
        {
            return repository.ReturnEdit(db, companyID, cityID);
        }
    }
}

