using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using System.EnterpriseServices;
using System.Configuration;
using Models;
using Common;
using Langben.DAL;
using Langben.BLL;
using System.Web.Http;
using Langben.App.Models;
using System.Reflection;
using Langben.DAL.Model;

namespace Langben.App.Areas.CRM.Controllers
{
    public class CRM_Company_ServiceManagerApiController : BaseApiController
    {
        private string menuID = "1003";
        private string OmenuID = "1005";
        private string NmenuID = "1004";
        /// <summary>
        /// 异步加载数据(根据责任客服ID获取该客服的所有企业)
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            int total = 0;
            int intBranchID = LoginInfo.BranchID;
            string strCompanyName = "";
            string userID_ZRKF = "";
            int? intUserID_ZR = null;
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
                string[] search = getParam.search.Split('^');
                strCompanyName = search[0];
                if (search.Length > 1)
                {
                    userID_ZRKF = search[1];
                }
                //strCompanyName = getParam.search;
            }
            if (!string.IsNullOrEmpty(userID_ZRKF))
            {
                intUserID_ZR = Convert.ToInt32(userID_ZRKF);
            }
            List<CRM_CompanyView> queryData = m_BLL.GetCompanyListForServiceEdit(getParam.id, getParam.page, getParam.rows, strCompanyName, intUserID_ZR, intBranchID, departmentScope, LoginInfo.UserID, LoginInfo.DepartmentID, departments, ref total);

            //List<CRM_CompanyView> queryData = m_BLL.GetCompanyListForServiceEdit(getParam.id, getParam.page, getParam.rows, strCompanyName, intUserID_ZR, intBranchID, ref total);

            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }

        /// <summary>
        /// 异步加载数据(已服务企业)
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult YPostData([FromBody]GetDataParam getParam)
        {
            //根据责任客服经理ID查询企业责任客服不为空的企业列表

            //根据登陆人判断是责任客服经理，查询归属责任客服经理下的所有已服务的企业
            int total = 0;
            int branchID = LoginInfo.BranchID;
            string strCompanyName = "";
            string userID_ZRKF = "";
            int? intUserID_ZR = null;
            #region 权限
            string departments = "";
            int departmentScope = base.MenuDepartmentScopeAuthority(OmenuID);
            if (departmentScope == (int)DepartmentScopeAuthority.无限制)//无限制
            {
                //部门业务权限
                departments = MenuDepartmentAuthority(menuID);
            }
            #endregion
            if (!string.IsNullOrEmpty(getParam.search))
            {
                string[] search = getParam.search.Split('^');

                strCompanyName = search[0];
                userID_ZRKF = search[1];
            }
            if (!string.IsNullOrEmpty(userID_ZRKF))
            {
                intUserID_ZR = Convert.ToInt32(userID_ZRKF);
            }
            List<CRM_CompanyView> queryData = m_BLL.GetCompanyListForOldService(getParam.id, getParam.page, getParam.rows, strCompanyName, intUserID_ZR, branchID, departmentScope, LoginInfo.UserID, LoginInfo.DepartmentID, departments, ref total);

            //List<CRM_CompanyView> queryData = m_BLL.GetCompanyListForOldService(getParam.id, getParam.page, getParam.rows, strCompanyName, intUserID_ZR, intBranchID, ref total);

            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }

        /// <summary>
        /// 异步加载数据(新企业分配客服)
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult KFPostData([FromBody]GetDataParam getParam)
        {
            //根据责任客服经理ID查询企业责任客服为空的企业列表

            //根据登陆人判断是责任客服经理，查询归属责任客服经理下的所有企业
            int total = 0;
            int branchID = LoginInfo.BranchID;
            string strCompanyName = "";
            int? intUserID_ZR = null;
            #region 权限
            string departments = "";
            int departmentScope = base.MenuDepartmentScopeAuthority(NmenuID);
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
            List<CRM_CompanyView> queryData = m_BLL.GetCompanyListForNewService(getParam.id, getParam.page, getParam.rows, strCompanyName, intUserID_ZR, branchID, departmentScope, LoginInfo.UserID, LoginInfo.DepartmentID, departments, ref total);

            //List<CRM_CompanyView> queryData = m_BLL.GetCompanyListForNewService(getParam.id, getParam.page, getParam.rows, strCompanyName, intUserID_ZR, branchID, ref total);

            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }


        /// <summary>
        /// 分配企业责任客服
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public Common.ClientResult.Result Services(string companyId, string searchUser)
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
                        branch.UserID_ZR = Convert.ToInt32(searchUser);
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

        public CRM_Company_ServiceManagerApiController()
            : this(new CRM_CompanyBLL()) { }

        public CRM_Company_ServiceManagerApiController(CRM_CompanyBLL bll)
        {
            m_BLL = bll;
        }
    }
}