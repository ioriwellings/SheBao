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

namespace Langben.App.Areas.CRM.Controllers
{
    /// <summary>
    /// 客户_企业联系人信息
    /// </summary>
    public class CRM_CompanyLinkManApiController : BaseApiController
    {
        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            int total = 0;
            List<CRM_CompanyLinkMan> queryData = m_BLL.GetByParam(getParam.id, getParam.order, getParam.sort, getParam.search);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData.Select(s => new
                {
                    ID = s.ID,
                    CRM_Company_ID = s.CRM_Company_ID,
                    LinkManName = s.LinkManName,
                    Position = s.Position,
                    Address = s.Address,
                    Mobile = s.Mobile,
                    Telephone = s.Telephone,
                    Email = s.Email,
                    Remark = s.Remark,
                    IsDefault=s.IsDefault=="Y"?"是":"否",
                    CreateTime = s.CreateTime ,
                    CreateUserID = s.CreateUserID,
                    CreateUserName = s.CreateUserName,
                    Status = s.Status,
                    BranchID = s.BranchID
                })
            };
            return data;
        }

        /// <summary>
        /// 根据ID获取数据模型
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public CRM_CompanyLinkMan Get(int id)
        {
            CRM_CompanyLinkMan item = m_BLL.GetById(id);
            return item;
        }
        /// <summary>
        /// 停用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Common.ClientResult.Result Stop(int id)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (ModelState.IsValid)
            {   //数据校验

                CRM_CompanyLinkMan item = m_BLL.GetById(id);

                item.Status = 0;//停用

                string returnValue = string.Empty;
                if (m_BLL.Edit(ref validationErrors, item))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，客户_企业联系人的Id为" + id, "客户_企业联系人_停用"
                        );//写入日志                   
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = Suggestion.UpdateSucceed;
                    return result; //提示更新成功 
                }
                else
                {
                    if (validationErrors != null && validationErrors.Count > 0)
                    {
                        validationErrors.All(a =>
                        {
                            returnValue += a.ErrorMessage;
                            return true;
                        });
                    }
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业联系人的Id为" + id + "," + returnValue, "客户_企业联系人_停用"
                        );//写入日志   
                    result.Code = Common.ClientCode.Fail;
                    result.Message = Suggestion.UpdateFail + returnValue;
                    return result; //提示更新失败
                }
            }
            result.Code = Common.ClientCode.FindNull;
            result.Message = Suggestion.UpdateFail + "请核对输入的数据的格式";
            return result; //提示输入的数据的格式不对         
        }

        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Common.ClientResult.Result Start(int id)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (ModelState.IsValid)
            {   //数据校验

                CRM_CompanyLinkMan item = m_BLL.GetById(id);

                item.Status = 1;//启用

                string returnValue = string.Empty;
                if (m_BLL.Edit(ref validationErrors, item))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，客户_企业联系人的Id为" + id, "客户_企业联系人_启用"
                        );//写入日志                   
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = Suggestion.UpdateSucceed;
                    return result; //提示更新成功 
                }
                else
                {
                    if (validationErrors != null && validationErrors.Count > 0)
                    {
                        validationErrors.All(a =>
                        {
                            returnValue += a.ErrorMessage;
                            return true;
                        });
                    }
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业联系人的Id为" + id + "," + returnValue, "客户_企业联系人_启用"
                        );//写入日志   
                    result.Code = Common.ClientCode.Fail;
                    result.Message = Suggestion.UpdateFail + returnValue;
                    return result; //提示更新失败
                }
            }
            result.Code = Common.ClientCode.FindNull;
            result.Message = Suggestion.UpdateFail + "请核对输入的数据的格式";
            return result; //提示输入的数据的格式不对         
        }
        /// <summary>
        /// 设置默认联系人
        /// </summary>
        /// <param name="id">联系人id</param>
        /// <returns></returns>
        public Common.ClientResult.Result SetDefault(int id,int companyID)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
                string returnValue = string.Empty;
                if (m_BLL.SetDefault(ref validationErrors, id,companyID))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，客户_企业联系人的Id为" + id, "客户_企业联系人_设置默认联系人"
                        );//写入日志                   
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = Suggestion.UpdateSucceed;
                    return result; //提示更新成功 
                }
                else
                {
                    if (validationErrors != null && validationErrors.Count > 0)
                    {
                        validationErrors.All(a =>
                        {
                            returnValue += a.ErrorMessage;
                            return true;
                        });
                    }
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业联系人的Id为" + id + "," + returnValue, "客户_企业联系人_设置默认联系人"
                        );//写入日志   
                    result.Code = Common.ClientCode.Fail;
                    result.Message = Suggestion.UpdateFail + returnValue;
                    return result; //提示更新失败
                }
                
        }

        IBLL.ICRM_CompanyLinkManBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public CRM_CompanyLinkManApiController()
            : this(new CRM_CompanyLinkManBLL()) { }

        public CRM_CompanyLinkManApiController(CRM_CompanyLinkManBLL bll)
        {
            m_BLL = bll;
        }

    }
}


