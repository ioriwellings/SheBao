using Common;
using Langben.DAL;
using Langben.DAL.Model;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Langben.App.Areas.Suppliers.Controllers
{
    public class SupplierAddFeedBackApiController : BaseApiController
    {
        JsonResult jr = new JsonResult();
        IBLL.IEmployeeAddBLL eadd_BLL = new BLL.EmployeeAddBLL();
        SysEntities SysEntitiesO2O = new SysEntities();

        #region 待待供应商客服反馈报增信息列表

        /// <summary>
        /// 待待供应商客服反馈报增信息列表
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult FeedbackModifyList([FromBody]GetDataParam getParam)
        {
            int total = 0;
            string search = getParam.search;
            if (string.IsNullOrWhiteSpace(search))
            {
                search = "State&" + Common.EmployeeAdd_State.已发送供应商.ToString() + "^";
            }
            else
            {
                search += "State&" + Common.EmployeeAdd_State.已发送供应商.ToString() + "^";
            }
            search += "UserID_Supplier&" + LoginInfo.UserID + "^";
            List<EmployeeApprove> queryData = eadd_BLL.GetApproveListByParam(getParam.id, getParam.page, getParam.rows, search, ref total);

            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }
        #endregion

        #region 社保种类 王帅
        /// <summary>
        /// 社保种类
        /// </summary>
        /// <param name="ID">缴纳地id</param>
        /// <returns></returns>
        public ActionResult getInsuranceKindList(string ID)
        {
            ID = ID.Trim();
            List<idname__> list = eadd_BLL.getInsuranceKindid(ID);
            jr.Data = new JsonMessageResult<List<idname__>>("0000", "成功！", list);
            return jr;
        }
        #endregion

        #region 供应商客服报增反馈 成功 动作
             /// <summary>
                /// 供应商专员报增反馈 成功
            /// </summary>
            /// <param name="query">报增成功id集合</param>
            /// <param name="CompanyEmployeeRelationId">人员企业关系</param>
            /// <param name="alltype">险种</param>
            /// <returns></returns>
        public string FeedbackIndexPass1(string query, string CompanyEmployeeRelationId, string alltype)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    alltype = HttpUtility.HtmlDecode(alltype);
                    string result = "";
                    string prompt = "";
                    int[] intArray;
                    string[] strArray = query.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    intArray = Array.ConvertAll<string, int>(strArray, s => int.Parse(s));
                    int?[] intArrayall1 = new int?[10];
                    List<int?> InsuranceKindTypes = new List<int?>();
                    string[] strArrayall = alltype.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var a in strArrayall)
                    {
                        int InsuranceKindId = (int)(Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), a);
                        InsuranceKindTypes.Add(InsuranceKindId);
                    }

                    int[] fuwuintArray;
                    string[] fuwuArray = CompanyEmployeeRelationId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    fuwuintArray = Array.ConvertAll<string, int>(fuwuArray, s => int.Parse(s));
                    using (var ent = new SysEntities())
                    {

                        var empadd = ent.EmployeeAdd.Where(a => true);//报增表
                        var updateEmpAdd = ent.EmployeeAdd.Where(a => intArray.Contains(a.Id) && InsuranceKindTypes.Contains(a.InsuranceKindId));//获取险种要报增的险种
                        if (updateEmpAdd != null && updateEmpAdd.Count() >= 1)
                        {
                            foreach (var item in updateEmpAdd)
                            {
                                string statesuccess = EmployeeAdd_State.申报成功.ToString();//申报成功 && o.State == statesuccess
                                var EmployeeAdd = ent.EmployeeAdd.FirstOrDefault(o => o.Id == item.Id );
                                var CompanyEmployeeRelation = ent.CompanyEmployeeRelation.FirstOrDefault(o => o.Id == EmployeeAdd.CompanyEmployeeRelationId);
                                var CompanyEmployeeRelationList = ent.CompanyEmployeeRelation.FirstOrDefault(o => o.EmployeeId == CompanyEmployeeRelation.EmployeeId && o.CompanyId == CompanyEmployeeRelation.CompanyId && o.Id == EmployeeAdd.CompanyEmployeeRelationId&&o.State=="在职");

                                var EmployeeAdd_Seccess = ent.EmployeeAdd.FirstOrDefault(o => o.CompanyEmployeeRelationId == CompanyEmployeeRelationList.Id && o.State == statesuccess && o.InsuranceKindId == EmployeeAdd.InsuranceKindId);

                                if (EmployeeAdd_Seccess != null)
                                {
                                    var KindName = Enum.GetName(typeof(Common.EmployeeAdd_InsuranceKindId), EmployeeAdd.InsuranceKindId);
                                    var Employee = ent.Employee.FirstOrDefault(o => o.Id == CompanyEmployeeRelationList.EmployeeId);
                                    result = result + Employee.Name + "在此公司下以存在" + KindName + "的报增成功记录";
                                }
                                else
                                {
                                    item.State = EmployeeAdd_State.申报成功.ToString();
                                    item.UpdateTime = DateTime.Now;
                                    item.UpdatePerson = LoginInfo.LoginName;
                                    ent.SaveChanges();
                                    #region 添加服务费
                                    #region 得出服务费

                                    decimal payService_One = 0;
                                    decimal payService_OneBJ = 0;
                                    int count = 0;
                                    CRM_CompanyRepository api1 = new CRM_CompanyRepository();
                                    count = api1.getEmployee(SysEntitiesO2O, item.CompanyEmployeeRelation.CompanyId, item.YearMonth);

                                    int[] cclpStatus = new int[]{
                                       (int)Common.Status.启用, (int)Common.Status.修改中 
                                        };
                                    //找出单人服务费
                                    var cRM_CompanyLadderPrice = SysEntitiesO2O.CRM_CompanyLadderPrice.Where(cclp => cclp.CRM_Company_ID == item.CompanyEmployeeRelation.CompanyId && cclpStatus.Contains(cclp.Status) && cclp.BeginLadder <= count && cclp.EndLadder >= count);
                                    if (cRM_CompanyLadderPrice.Count() > 0)
                                    {
                                        payService_One = cRM_CompanyLadderPrice.OrderByDescending(o => o.EndLadder).FirstOrDefault().SinglePrice;

                                    }
                                    decimal payService_All = 0;
                                    bool zhengHu = false;
                                    //找出整户服务费
                                    var cRM_CompanyPrice = SysEntitiesO2O.CRM_CompanyPrice.Where(ccp => (new[] { (int)Common.Status.启用, (int)Common.Status.修改中 }).Contains(ccp.Status) && ccp.CRM_Company_ID == item.CompanyEmployeeRelation.CompanyId);
                                    if (cRM_CompanyPrice.Count() > 0)
                                    {
                                        payService_All = cRM_CompanyPrice.FirstOrDefault().LowestPrice.Value;
                                        payService_OneBJ = cRM_CompanyPrice.FirstOrDefault().AddPrice.Value;
                                    }
                                    //判断取单人服务费还是整户服务费
                                    if (payService_One * count > payService_All)
                                    {
                                        zhengHu = false;
                                        payService_All = 0;
                                    }
                                    else
                                    {
                                        zhengHu = true;
                                        payService_One = 0;
                                    }
                                    #endregion
                                    #region 得出需要修改的人和公司全部人
                                    string[] ZENG_STATUS = new string[]{
                        Common.EmployeeAdd_State.待责任客服确认.ToString(),   Common.EmployeeAdd_State.待员工客服经理分配.ToString(),
                        Common.EmployeeAdd_State.待员工客服确认.ToString(), Common.EmployeeAdd_State.员工客服已确认.ToString(),  Common.EmployeeAdd_State.社保专员已提取.ToString() ,
                         Common.EmployeeAdd_State.申报成功.ToString() 
                        };

                                    int[] emploees = api1.getEmployeeIDs(SysEntitiesO2O, item.CompanyEmployeeRelation.CompanyId, item.YearMonth);
                                    var CompanyEmployeeRelationlist = (from ce in SysEntitiesO2O.CompanyEmployeeRelation.Where(ce => ce.State == "在职")
                                                                       join e in SysEntitiesO2O.EmployeeAdd on ce.Id equals e.CompanyEmployeeRelationId
                                                                       where ce.CompanyId == item.CompanyEmployeeRelation.CompanyId && ZENG_STATUS.Contains(e.State) && emploees.Contains(ce.EmployeeId ?? 0)
                                                                       select new
                                                                       {
                                                                           CompanyId = (int)ce.CompanyId,
                                                                           Employee_ID = ce.EmployeeId,
                                                                           YearMonth = e.YearMonth
                                                                       }).Distinct().ToList();
                                    #endregion
                                    #endregion
                                    #region 修改费用服务费表中的正常服务费
                                    int PaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.正常;
                                    int[] FEIYONG_STATUS = new int[] { (int)Common.COST_Table_Status.财务作废, (int)Common.COST_Table_Status.客户作废, (int)Common.COST_Table_Status.责任客服作废 };
                                    var COST_CostTable = ent.COST_CostTable.Where(a => true);
                                    foreach (var item3 in CompanyEmployeeRelationlist)
                                    {
                                        var list1 = (from a in COST_CostTable
                                                     join b in ent.COST_CostTableService on a.ID equals b.COST_CostTable_ID
                                                     where !FEIYONG_STATUS.Contains(a.Status) && b.PaymentStyle == PaymentStyle && b.CRM_Company_ID == item3.CompanyId && a.YearMonth == item3.YearMonth && b.Employee_ID == item.CompanyEmployeeRelation.EmployeeId
                                                     select b).FirstOrDefault();//费用表
                                        if (list1 != null)
                                        {
                                            if (zhengHu == true)//整户服务费
                                            {
                                                list1.ServiceCoset = payService_All;

                                                var list2 = from a in COST_CostTable
                                                            join b in ent.COST_CostTableService on a.ID equals b.COST_CostTable_ID
                                                            where !FEIYONG_STATUS.Contains(a.Status) && b.PaymentStyle == PaymentStyle && b.CRM_Company_ID == item3.CompanyId && a.YearMonth == item3.YearMonth && b.Employee_ID != item.CompanyEmployeeRelation.EmployeeId
                                                            select b;//
                                                if (list2.Count() > 0)
                                                {
                                                    foreach (var item4 in list2)
                                                    {
                                                        item4.ServiceCoset = 0;
                                                    }
                                                }
                                            }
                                            else//单人服务费
                                            {
                                                list1.ServiceCoset = payService_One;
                                                var list2 = from a in COST_CostTable
                                                            join b in ent.COST_CostTableService on a.ID equals b.COST_CostTable_ID
                                                            where !FEIYONG_STATUS.Contains(a.Status) && b.PaymentStyle == PaymentStyle && b.CRM_Company_ID == item3.CompanyId && a.YearMonth == item3.YearMonth && b.Employee_ID != item.CompanyEmployeeRelation.EmployeeId
                                                            select b;//
                                                if (list2.Count() > 0)
                                                {
                                                    foreach (var item4 in list2)
                                                    {


                                                        item4.ServiceCoset = payService_One;


                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                    #region  修改补缴服务费
                                    //取中间表的补缴数据
                                    string Middlestate = Common.Status.启用.ToString();
                                    var CompanyEmployeeRelationlistMiddle =
                                        (from em in ent.EmployeeMiddle.Where(em => em.PaymentStyle == (int)Common.EmployeeMiddle_PaymentStyle.补缴)
                                         join cer in ent.CompanyEmployeeRelation on em.CompanyEmployeeRelationId equals cer.Id
                                         join e in ent.Employee on cer.EmployeeId equals e.Id
                                         where cer.CompanyId == item.CompanyEmployeeRelation.CompanyId && em.State == Middlestate
                                         select new
                                         {
                                             StartDate = em.StartDate,
                                             Employee_ID = e.Id,
                                             CompanyId = cer.CompanyId,
                                             PaymentMonth = em.PaymentMonth,

                                         }).GroupBy(o => new { o.Employee_ID, o.CompanyId, o.PaymentMonth, o.StartDate }).Select(o => new
                                         {
                                             StartDate = o.Key.StartDate,
                                             ServiceCoset = (decimal)o.Max(m => m.PaymentMonth) * payService_OneBJ,
                                             Employee_ID = o.Key.Employee_ID,
                                             CompanyId = item.CompanyEmployeeRelation.CompanyId,
                                             PaymentMonth = o.Key.PaymentMonth,
                                         }).ToList();
                                    #region 修改费用服务费表中的补缴服务费
                                    int bujiaoPaymentStyle = (int)Common.EmployeeMiddle_PaymentStyle.补缴;


                                    foreach (var item3 in CompanyEmployeeRelationlistMiddle)
                                    {
                                        var listooo = (from a in COST_CostTable
                                                       join b in ent.COST_CostTableService on a.ID equals b.COST_CostTable_ID
                                                       where !FEIYONG_STATUS.Contains(a.Status) && b.PaymentStyle == bujiaoPaymentStyle && b.CRM_Company_ID == item3.CompanyId && a.YearMonth == item3.StartDate && b.Employee_ID == item3.Employee_ID
                                                       select b).FirstOrDefault();//费用表
                                        if (listooo != null)
                                        {
                                            listooo.ServiceCoset = item3.ServiceCoset;

                                        }
                                    }
                                    #endregion
                                    #endregion
                                    ent.SaveChanges();
                                    result = "报增成功！"; 
                                }
                            }
                            scope.Complete();
                            prompt = result;


                        }
                        else
                            prompt = "不存在此险种！"; 
                        return prompt;
                    }
                }

            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }

        #endregion
        #region 供应商客服报增反馈 失败 动作
          /// <summary>
          /// 供应商客服报增反馈
          /// </summary>
          /// <param name="ids">失败险id</param>
          /// <param name="message">失败原因</param>
          /// <param name="alltype">所有险种</param>
          /// <param name="CompanyEmployeeRelationId">企业员工关系</param>
          /// <returns></returns>
        public string FeedBackAction1(string ids, string message, string alltype, string CompanyEmployeeRelationId)
        {
            try
            {
                message = HttpUtility.HtmlDecode(message);
                string enable = Status.启用.ToString();
                alltype = HttpUtility.HtmlDecode(alltype);


                string[] strArrayall = alltype.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                // intArrayall = Array.ConvertAll<string, int>(strArrayall, s => int.Parse(s));
                List<int?> InsuranceKindTypes = new List<int?>();

                foreach (var a in strArrayall)
                {
                    int InsuranceKindId = (int)(Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), a);
                    InsuranceKindTypes.Add(InsuranceKindId);
                }

                string result = "";//返回的结果
                int[] intArray;
                string[] strArray = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                intArray = Array.ConvertAll<string, int>(strArray, s => int.Parse(s));
                using (TransactionScope scope = new TransactionScope())
                {
                    using (var ent = new SysEntities())
                    {
                        var empadd = ent.EmployeeAdd.Where(a => true);
                        var updateEmpAdd = ent.EmployeeAdd.Where(a => intArray.Contains(a.Id) && InsuranceKindTypes.Contains(a.InsuranceKindId));
                        if (updateEmpAdd != null && updateEmpAdd.Count() >= 1)
                        {
                            foreach (var item in updateEmpAdd)
                            {
                                result = eadd_BLL.ChangeServicecharge(item, message, LoginInfo.LoginName);                              

                            }
                            scope.Complete();
                        }
                        if (result != "")
                        {
                            return result;
                        }
                        else
                        {
                            return result = "操作成功";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }

        #endregion
    }
}
