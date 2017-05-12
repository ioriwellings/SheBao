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
    /// 客户_企业阶梯报价
    /// </summary>
    public class CRM_CompanyLadderPriceApiController : BaseApiController
    {
        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            return m_BLL.GetCompanyLadderPirceList(Convert.ToInt32(getParam.search), LoginInfo.BranchID);
        }

        /// <summary>
        /// 根据ID获取数据模型
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public CRM_CompanyLadderPrice Get(int id)
        {
            CRM_CompanyLadderPrice item = m_BLL.GetById(id);
            return item;
        }

        /// <summary>
        /// 根据ID获取数据
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public string GetLadderPrice(int id)
        {
            string item = m_BLL.GetCompanyLadderPrice(id);
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

                CRM_CompanyLadderPrice item = m_BLL.GetById(id);

                item.Status = 0;//停用

                string returnValue = string.Empty;
                if (m_BLL.Edit(ref validationErrors, item))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，客户_企业阶梯报价信息的Id为" + id, "客户_企业阶梯报价信息_停用"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业阶梯报价信息的Id为" + id + "," + returnValue, "客户_企业阶梯报价信息_停用"
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

                CRM_CompanyLadderPrice item = m_BLL.GetById(id);
                IBLL.ICRM_CompanyPriceBLL bll = new CRM_CompanyPriceBLL();
                List<CRM_CompanyPrice> chk = bll.GetActiveProduct(item.CRM_Company_ID);
                int chkid = 0;
                if (chk.Count != 0)
                {
                    chkid = chk.FirstOrDefault().PRD_Product_ID;
                }

                if (item.PRD_Product_ID != chkid)
                {
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "阶梯报价信息必须与报价信息产品一致！";
                    return result;
                }

                item.Status = 1;//启用

                if (!m_BLL.CheckRange(item.CRM_Company_ID, item.PRD_Product_ID, Convert.ToInt32(item.BeginLadder), Convert.ToInt32(item.EndLadder), item.BranchID, item.ID))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业阶梯报价信息的Id为" + id, "客户_企业阶梯报价信息_启用"
                      );//写入日志   
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "请检查阶梯报价范围！";
                    return result; //提示更新失败
                }

                string returnValue = string.Empty;
                if (m_BLL.Edit(ref validationErrors, item))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，客户_企业阶梯报价信息的Id为" + id, "客户_企业阶梯报价信息_启用"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业阶梯报价信息的Id为" + id + "," + returnValue, "客户_企业阶梯报价信息_启用"
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

        IBLL.ICRM_CompanyLadderPriceBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public CRM_CompanyLadderPriceApiController()
            : this(new CRM_CompanyLadderPriceBLL()) { }

        public CRM_CompanyLadderPriceApiController(CRM_CompanyLadderPriceBLL bll)
        {
            m_BLL = bll;
        }

    }
}


