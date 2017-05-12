using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Common;
using Langben.BLL;
using Langben.DAL;
using Models;

namespace Langben.App.Controllers
{
    public class EmployeeStopPaymentFeedbackApiController : BaseApiController
    {
        #region 待社保专员报减反馈

        /// <summary>
        /// 待社保专员报增反馈
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult FeedbackModifyList([FromBody]GetDataParam getParam)
        {
            int total = 0;
            string search = getParam.search;
            
            string state = Common.EmployeeStopPayment_State.社保专员已提取.ToString();
            search += "State&" + state + "^";
            search += "UserID_SB&" + LoginInfo.UserID + "^";
            List<EmployeeApprove> queryData = m_BLL.GetApproveList(getParam.id, getParam.page, getParam.rows, search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }
        #endregion


        public StopPaymentEmployeeInfo Get(int id)
        {
            string strStatus = EmployeeStopPayment_State.社保专员已提取.ToString();
            StopPaymentEmployeeInfo item = m_BLL.GetStopPaymentEmployeeInfoForStop(id, strStatus);
            return item;
        }

        public Common.ClientResult.Result Post(string parameters)
        {

            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (!string.IsNullOrEmpty(parameters))
            {
                List<EmployeeStopPayment> list = new List<EmployeeStopPayment>();
                string[] kinds = parameters.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string kind in kinds)
                {
                    string[] param = kind.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);//EmployeeStopPaymentID,报减类型

                    EmployeeStopPayment stop = new EmployeeStopPayment()
                    {
                        Id = Convert.ToInt32(param[0]),
                        PoliceOperationId = Convert.ToInt32(param[1])
                    };
                    list.Add(stop);
                }


                string returnValue = string.Empty;
                if (m_BLL.EditStopPaymentOperation(list))
                {
                    //LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，员工停缴的信息的Id为" + entity.Id, "员工停缴");//写入日志 
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = Suggestion.UpdateSucceed;
                    return result; //提示修改成功
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，员工停缴手续的信息，" + returnValue, "员工停缴");//写入日志                      
                    result.Code = Common.ClientCode.Fail;
                    result.Message = Suggestion.UpdateFail + returnValue;
                    return result; //提示修改失败
                }
            }

            result.Code = Common.ClientCode.FindNull;
            result.Message = Suggestion.UpdateFail + "，请核对输入的数据的格式"; //提示输入的数据的格式不对 
            return result;
        }

        public Common.ClientResult.Result SetSuccess(string stopIds)
        {

            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (!string.IsNullOrEmpty(stopIds))
            {
                string returnValue = string.Empty;
                if (m_BLL.SetStopPaymentSuccess(stopIds))
                {
                    //LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，员工停缴的信息的Id为" + entity.Id, "员工停缴");//写入日志 
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，报减设置成功，" + returnValue, "员工停缴");//写入日志                      
                    result.Code = Common.ClientCode.Fail;
                    result.Message = Suggestion.UpdateFail + returnValue;
                    return result; //提示插入失败
                }
            }

            result.Code = Common.ClientCode.FindNull;
            result.Message = Suggestion.UpdateFail + "，请核对输入的数据的格式"; //提示输入的数据的格式不对 
            return result;
        }

        public Common.ClientResult.Result SetFail(string stopIds,string remark)
        {

            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (!string.IsNullOrEmpty(stopIds))
            {
                string returnValue = string.Empty;
                if (m_BLL.SetStopPaymentFail(stopIds,remark))
                {
                    //LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，员工停缴的信息的Id为" + entity.Id, "员工停缴");//写入日志 
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，报减设置失败，" + returnValue, "员工停缴");//写入日志                      
                    result.Code = Common.ClientCode.Fail;
                    result.Message = Suggestion.UpdateFail + returnValue;
                    return result; //提示插入失败
                }
            }

            result.Code = Common.ClientCode.FindNull;
            result.Message = Suggestion.UpdateFail + "，请核对输入的数据的格式"; //提示输入的数据的格式不对 
            return result;
        } 

        IBLL.IEmployeeStopPaymentBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public EmployeeStopPaymentFeedbackApiController()
            : this(new EmployeeStopPaymentBLL()) { }

        public EmployeeStopPaymentFeedbackApiController(EmployeeStopPaymentBLL bll)
        {
            m_BLL = bll;
        }
    }
}
