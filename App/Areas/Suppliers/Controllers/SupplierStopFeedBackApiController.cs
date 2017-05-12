using Common;
using Langben.DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Langben.App.Areas.Suppliers.Controllers
{
    public class SupplierStopFeedBackApiController : BaseApiController
    {
        IBLL.IEmployeeStopPaymentBLL m_BLL = new BLL.EmployeeStopPaymentBLL();
        ValidationErrors validationErrors = new ValidationErrors();

        public Common.ClientResult.Result SetSuccess(string stopIds, string alltype)
        {

            alltype = HttpUtility.HtmlDecode(alltype);

            IBLL.IEmployeeStopPaymentBLL m_BLL = new BLL.EmployeeStopPaymentBLL();
            ValidationErrors validationErrors = new ValidationErrors();

            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (!string.IsNullOrEmpty(stopIds))
            {
                string returnValue = string.Empty;
                SysEntities db = new SysEntities();
                List<int> IdList = new List<int>();
                string[] strArrayid = stopIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] strArrayType = alltype.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                List<int?> InsuranceKindList = new List<int?>();//险种

                foreach (string id in strArrayid)
                {
                    int idd= int.Parse(id);
                    IdList.Add(idd);
                }

                foreach (var a in strArrayType)
                {
                    int InsuranceKindId = (int)(Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), a);
                    InsuranceKindList.Add(InsuranceKindId);
                }

                if (SetStopPaymentSuccess(db, IdList, InsuranceKindList))
                {
                    //LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，员工停缴的信息的Id为" + entity.Id, "员工停缴");//写入日志 
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = "保存成功！";
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


        #region 设置报减成功

        public bool SetStopPaymentSuccess(SysEntities db, List<int> stopIds, List<int?> alltypes)
        {
            bool result = false;
            try
            {

                var stopPayment = db.EmployeeStopPayment.Where(o => stopIds.Contains(o.Id)&&alltypes.Contains(o.EmployeeAdd.InsuranceKindId));
                foreach (EmployeeStopPayment stop in stopPayment)
                {
                    stop.State = EmployeeStopPayment_State.申报成功.ToString();
                    stop.EmployeeAdd.State = EmployeeAdd_State.已报减.ToString();
                }
                db.SaveChanges();
                //修改企业员工关系表 的状态
                var lstRelationId = stopPayment.Select(o => o.EmployeeAdd).GroupBy(g => g.CompanyEmployeeRelationId).Select(x => x.Key);
                foreach (int relationId in lstRelationId)
                {
                    string strState = EmployeeAdd_State.已报减.ToString();

                    if (!db.EmployeeAdd.Any(o => o.CompanyEmployeeRelationId == relationId && o.State != strState))
                    {
                        int id = relationId;
                        var companyEmployeeRelation = db.CompanyEmployeeRelation.Single(o => o.Id == id);
                        companyEmployeeRelation.State = "离职";
                    }
                }


                db.SaveChanges();
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }
        #endregion

        #region 供应商客服反馈报减信息列表

        /// <summary>
        /// 供应商客服反馈报减信息列表
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult FeedbackModifyList([FromBody]GetDataParam getParam)
        {
            int total = 0;
            string search = getParam.search;

            string state = Common.EmployeeStopPayment_State.已发送供应商.ToString();
            search += "State&" + state + "^";
            search += "UserID_Supplier&" + LoginInfo.UserID + "^";
            List<EmployeeApprove> queryData = m_BLL.GetApproveList(getParam.id, getParam.page, getParam.rows, search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }
        #endregion

        #region 设置报减失败
        /// <summary>
        /// 设置报减失败操作
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="reason">失败原因</param>
        /// <param name="alltype">报减险种</param>
        /// <returns></returns>
        public Common.ClientResult.Result SetFail(string ids, string reason, string alltype)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            reason = HttpUtility.HtmlDecode(reason);  // 失败原因
            alltype = HttpUtility.HtmlDecode(alltype);

            string[] strArrayall = alltype.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<int?> insuranceKindTypes = new List<int?>();  // 失败的险种

            foreach (var a in strArrayall)
            {
                int insuranceKindId = (int)(Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), a);
                insuranceKindTypes.Add(insuranceKindId);
            }

            if (!string.IsNullOrEmpty(ids))
            {
                string returnValue = string.Empty;
                if (m_BLL.SetStopPaymentFailByInsuranceKind(ids, reason, insuranceKindTypes))
                {
                    LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，报减设置成功" + returnValue, "员工停缴");//写入日志 
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
        #endregion
    }
}
