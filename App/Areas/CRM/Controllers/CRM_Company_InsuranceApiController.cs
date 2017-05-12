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
    /// <summary>
    /// 客户_企业财务信息_待审核
    /// </summary>
    public class CRM_Company_InsuranceApiController : BaseApiController
    {
        #region 根据缴纳地获取社保种类列表
        /// <summary>
        /// 根据缴纳地获取社保种类列表
        /// </summary>
        /// <param name="ID">缴纳地id</param>
        /// <returns></returns>
        public JsonResult getInsuranceKindList(string ID)
        {

            IBLL.IInsuranceKindBLL bll = new BLL.InsuranceKindBLL();
            List<InsuranceKind> list = bll.GetByParam(0, "asc", "InsuranceKindId", "CityDDL_String&" + ID);

            JsonResult data = new JsonResult();
            data.Data = new JsonMessageResult<List<idname__>>("0000", "成功！", list.Select(c => new idname__() { ID = c.InsuranceKindId ?? 0, Name = c.Name }).OrderBy(c => c.ID).ToList());

            return data;
        }
        #endregion

        #region 获取社保信息列表
        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            //int total = 0;

            List<CompanyInsurance> queryData = m_BLL.GetCRM_Company_Insurance(getParam.id);
            var data = new Common.ClientResult.DataResult
            {
                total = queryData.Count,
                rows = queryData
            };
            return data;
        }
        #endregion

        #region 获取社保信息
        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult GetByCompanyCity(int id, string city)
        {
            //int total = 0;

            CompanyInsurance_EditView model = new CompanyInsurance_EditView();

            model = m_BLL.GetByCompanyCity(id, city);

            var data = new Common.ClientResult.DataResult
            {
                total = 1,
                rows = model
            };
            return data;
        }
        #endregion

        #region 获取待审核的新建社保信息
        /// <summary>
        /// 获取待审核的新建社保信息
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostAddData(int id, string city)
        {
            //int total = 0;
            List<CompanyInsurance> List = new List<CompanyInsurance>();
            CompanyInsurance model = new CompanyInsurance();

            model = m_BLL.GetAddData(id, city);

            List.Add(model);

            var data = new Common.ClientResult.DataResult
            {
                total = List.Count,
                rows = List
            };
            return data;
        }
        #endregion

        #region 获取待审核的修改社保信息和原信息
        /// <summary>
        /// 获取待审核的修改社保信息和原信息
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult GetEditData(int id, string city)
        {
            //int total = 0;
            List<CompanyInsurance> List = new List<CompanyInsurance>();
            CompanyInsurance model = new CompanyInsurance();

            model = m_BLL.GetEditData(id, city);

            List = m_BLL.GetCRM_Company_Insurance(id, city);

            List.Insert(0, model);

            var data = new Common.ClientResult.DataResult
            {
                total = List.Count,
                rows = List
            };
            return data;
        }
        #endregion

        #region 创建
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public Common.ClientResult.Result Post([FromBody]CompanyInsurance entity)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {
                if (entity.Account1 == null && entity.Account2 == null && entity.PoliceID1 == null && entity.PoliceID2 == null)
                {
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "请至少填写一项信息";
                    return result; //提示插入失败
                }

                int chk = m_BLL.CheckCompanyCity(entity.CRM_Company_ID ?? 0, entity.CityId);
                if (chk == 1)
                {
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "已经存在当前缴纳地的信息";
                    return result; //提示插入失败
                }
                if (chk == 2)
                {
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "当前缴纳地的信息正在审核中，请不要重复提交";
                    return result; //提示插入失败
                }

                string returnValue = string.Empty;
                if (m_BLL.CreateCRM_Company_Insurance(entity, LoginInfo.UserID, LoginInfo.RealName, LoginInfo.BranchID))
                {
                    LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，客户_企业社保信息_待审核", "客户_企业社保信息_待审核"
                        );//写入日志 
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = Suggestion.InsertSucceed;
                    return result; //提示创建成功
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，客户_企业社保信息_待审核，" + returnValue, "客户_企业社保信息_待审核"
                        );//写入日志                      
                    result.Code = Common.ClientCode.Fail;
                    result.Message = Suggestion.InsertFail + returnValue;
                    return result; //提示插入失败
                }
            }

            result.Code = Common.ClientCode.FindNull;
            result.Message = Suggestion.InsertFail + "，请核对输入的数据的格式"; //提示输入的数据的格式不对 
            return result;
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public Common.ClientResult.Result Put([FromBody]CompanyInsurance entity)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {
                if (entity.Account1 == null && entity.Account2 == null && entity.PoliceID1 == null && entity.PoliceID2 == null)
                {
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "请至少填写一项信息";
                    return result; //提示插入失败
                }

                string returnValue = string.Empty;
                if (m_BLL.UpdateCRM_Company_Insurance(entity, LoginInfo.UserID, LoginInfo.RealName, LoginInfo.BranchID))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，客户_企业社保信息_待审核", "客户_企业社保信息_待审核"
                        );//写入日志 
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = Suggestion.UpdateSucceed;
                    return result; //提示创建成功
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，客户_企业社保信息_待审核，" + returnValue, "客户_企业社保信息_待审核"
                        );//写入日志                      
                    result.Code = Common.ClientCode.Fail;
                    result.Message = Suggestion.UpdateFail + returnValue;
                    return result; //提示插入失败
                }
            }

            result.Code = Common.ClientCode.FindNull;
            result.Message = Suggestion.UpdateFail + "，请核对输入的数据的格式"; //提示输入的数据的格式不对 
            return result;
        }
        #endregion

        #region 停用
        /// <summary>
        /// 停用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Common.ClientResult.Result Stop(int id, string city)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (ModelState.IsValid)
            {   //数据校验

                string returnValue = string.Empty;
                if (m_BLL.ChangeInsuranceState(id, city, Common.Status.停用.ToString(), LoginInfo.RealName))
                {
                    LogClassModels.WriteServiceLog("企业社保信息停用成功", "企业社保信息_停用"
                        );//写入日志                   
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = "停用成功";
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
                    LogClassModels.WriteServiceLog("企业社保信息停用失败", "企业社保信息_停用"
                        );//写入日志   
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "停用失败";
                    return result; //提示更新失败
                }
            }
            result.Code = Common.ClientCode.FindNull;
            result.Message = "停用失败";
            return result; //提示输入的数据的格式不对               
        }
        #endregion

        #region 启用
        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Common.ClientResult.Result Start(int id, string city)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (ModelState.IsValid)
            {   //数据校验

                string returnValue = string.Empty;
                if (m_BLL.ChangeInsuranceState(id, city, Common.Status.启用.ToString(), LoginInfo.RealName))
                {
                    LogClassModels.WriteServiceLog("企业社保信息启用成功", "企业社保信息_启用"
                        );//写入日志                   
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = "启用成功";
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
                    LogClassModels.WriteServiceLog("企业社保信息启用失败", "企业社保信息_启用"
                        );//写入日志   
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "启用失败";
                    return result; //提示更新失败
                }
            }
            result.Code = Common.ClientCode.FindNull;
            result.Message = "启用失败";
            return result; //提示输入的数据的格式不对         
        }
        #endregion

        #region 审核通过新建信息
        /// <summary>
        /// 审核通过新建信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Common.ClientResult.Result PassAdd(int id, string city)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (ModelState.IsValid)
            {   //数据校验

                string returnValue = string.Empty;
                if (m_BLL.PassAdd(id, city))
                {
                    LogClassModels.WriteServiceLog("企业社保信息审核通过成功", "企业社保信息_审核"
                        );//写入日志                   
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = "提交成功";
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
                    LogClassModels.WriteServiceLog("企业社保信息审核通过失败", "企业社保信息_审核"
                        );//写入日志   
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "提交失败";
                    return result; //提示更新失败
                }
            }
            result.Code = Common.ClientCode.FindNull;
            result.Message = "提交失败";
            return result; //提示输入的数据的格式不对         
        }
        #endregion

        #region 审核退回新建信息
        /// <summary>
        /// 审核退回新建信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Common.ClientResult.Result ReturnAdd(int id, string city)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (ModelState.IsValid)
            {   //数据校验

                string returnValue = string.Empty;
                if (m_BLL.ReturnAdd(id, city))
                {
                    LogClassModels.WriteServiceLog("企业社保信息审核退回成功", "企业社保信息_审核"
                        );//写入日志                   
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = "提交成功";
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
                    LogClassModels.WriteServiceLog("企业社保信息审核退回失败", "企业社保信息_审核"
                        );//写入日志   
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "提交失败";
                    return result; //提示更新失败
                }
            }
            result.Code = Common.ClientCode.FindNull;
            result.Message = "提交失败";
            return result; //提示输入的数据的格式不对         
        }
        #endregion

        #region 审核通过新建信息
        /// <summary>
        /// 审核通过新建信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Common.ClientResult.Result PassEdit(int id, string city)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (ModelState.IsValid)
            {   //数据校验

                string returnValue = string.Empty;
                if (m_BLL.PassEdit(id, city))
                {
                    LogClassModels.WriteServiceLog("企业社保信息审核通过成功", "企业社保信息_审核"
                        );//写入日志                   
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = "提交成功";
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
                    LogClassModels.WriteServiceLog("企业社保信息审核通过失败", "企业社保信息_审核"
                        );//写入日志   
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "提交失败";
                    return result; //提示更新失败
                }
            }
            result.Code = Common.ClientCode.FindNull;
            result.Message = "提交失败";
            return result; //提示输入的数据的格式不对         
        }
        #endregion

        #region 审核退回新建信息
        /// <summary>
        /// 审核退回新建信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Common.ClientResult.Result ReturnEdit(int id, string city)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (ModelState.IsValid)
            {   //数据校验

                string returnValue = string.Empty;
                if (m_BLL.ReturnEdit(id, city))
                {
                    LogClassModels.WriteServiceLog("企业社保信息审核退回成功", "企业社保信息_审核"
                        );//写入日志                   
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = "提交成功";
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
                    LogClassModels.WriteServiceLog("企业社保信息审核退回失败", "企业社保信息_审核"
                        );//写入日志   
                    result.Code = Common.ClientCode.Fail;
                    result.Message = "提交失败";
                    return result; //提示更新失败
                }
            }
            result.Code = Common.ClientCode.FindNull;
            result.Message = "提交失败";
            return result; //提示输入的数据的格式不对         
        }
        #endregion

        IBLL.ICRM_Company_InsuranceBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public CRM_Company_InsuranceApiController()
            : this(new CRM_Company_InsuranceBLL()) { }

        public CRM_Company_InsuranceApiController(CRM_Company_InsuranceBLL bll)
        {
            m_BLL = bll;
        }

    }
}


