using Common;
using Langben.BLL;
using Langben.DAL;
using Langben.DAL.Model;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Langben.App.Areas.CRM.Controllers
{
    public class CRM_Company_SaleManagerApiController : BaseApiController
    {
        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            //根据销售经理ID查询企业列表

            //根据登陆人判断是销售经理，查询销售经理下的所有企业
            int total = 0;
            int branchID = LoginInfo.BranchID;
            //int intBranchID = 1;
            string strCompanyName = "";
            string userID_XS = "";
            int? intUserID_XS = null;
            string menuID = "1006";
            #region 权限
            string departments = "";
            int departmentScope = base.MenuDepartmentScopeAuthority(menuID);
            if (departmentScope == (int)DepartmentScopeAuthority.无限制)//无限制
            {
                //部门业务权限
                departments = MenuDepartmentAuthority(menuID);
            }
            #endregion
            if (!string.IsNullOrEmpty(getParam.search))
            {
                strCompanyName = getParam.search;
            }
            if (!string.IsNullOrEmpty(getParam.search))
            {
                string[] search = getParam.search.Split('^');
                strCompanyName = search[0];
                userID_XS = search[1];
            }
            if (!string.IsNullOrEmpty(userID_XS))
            {
                intUserID_XS = Convert.ToInt32(userID_XS);
            }

            List<CRM_CompanyView> queryData = m_BLL.GetCompanyListForSales(getParam.id, getParam.page, getParam.rows, strCompanyName, intUserID_XS, branchID,menuID,departmentScope,LoginInfo.UserID,LoginInfo.DepartmentID,departments, ref total);

            //List<CRM_CompanyView> queryData = m_BLL.GetCompanyListForSales(getParam.id, getParam.page, getParam.rows, strCompanyName, intUserID_XS, intBranchID, ref total);

            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }


        /// <summary>
        /// 分配企业责任销售
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public Common.ClientResult.Result Sales(string companyId, string searchUser)
        {
            CRM_CompanyToBranch branch = new CRM_CompanyToBranch();
            IBLL.ICRM_CompanyToBranchBLL m_BLL = new CRM_CompanyToBranchBLL();

            Common.ClientResult.Result result = new Common.ClientResult.Result();
            string returnValue = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(companyId))
                {
                    string[] id = companyId.Split(',');
                    for (int i = 0; i < id.Length; i++)
                    {
                        branch = m_BLL.GetAll().Where(p => p.CRM_Company_ID == Convert.ToInt32(id[i])).FirstOrDefault();
                        branch.UserID_XS = Convert.ToInt32(searchUser);
                        if (m_BLL.Edit(ref validationErrors, branch))
                        {
                            LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，企业信息的Id为" + branch.CRM_Company_ID, "企业的责任客服"
                            );//写入日志 
                        }
                    }
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = Suggestion.InsertSucceed;
                    return result; //提示创建成功
                }
            }
            catch (Exception ex)
            {
                if (validationErrors != null && validationErrors.Count > 0)
                {
                    validationErrors.All(a =>
                    {
                        returnValue += a.ErrorMessage;
                        return true;
                    });
                }
                LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，员工银行的信息，" + returnValue, "企业的责任客服"
                    );//写入日志                      
                result.Code = Common.ClientCode.Fail;
                result.Message = Suggestion.InsertFail + returnValue;
                return result; //提示插入失败
            }
            result.Code = Common.ClientCode.FindNull;
            result.Message = Suggestion.InsertFail + "，请核对输入的数据的格式"; //提示输入的数据的格式不对 
            return result;
        }

        IBLL.ICRM_CompanyBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public CRM_Company_SaleManagerApiController()
            : this(new CRM_CompanyBLL()) { }

        public CRM_Company_SaleManagerApiController(CRM_CompanyBLL bll)
        {
            m_BLL = bll;
        }
    }
}
