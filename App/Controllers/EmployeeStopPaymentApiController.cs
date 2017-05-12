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
using NPOI.SS.UserModel;
using System.IO;
using NPOI.HSSF.UserModel;
using System.Web;
using Langben.DAL.Model;
using System.Transactions;
using Langben.IBLL;

namespace Langben.App.Controllers
{
    /// <summary>
    /// 员工停缴
    /// </summary>
    public class EmployeeStopPaymentApiController : BaseApiController
    {
        IORG_UserBLL userBLL = new ORG_UserBLL();
        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            int total = 0;
            List<EmployeeStopPayment> queryData = m_BLL.GetByParam(getParam.id, getParam.page, getParam.rows, getParam.order, getParam.sort, getParam.search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData.Select(s => new
                {
                    Id = s.Id
                    ,
                    EmployeeAddId = s.EmployeeAddIdOld
                    ,
                    InsuranceMonth = s.InsuranceMonth
                    ,
                    PoliceOperationId = s.PoliceOperationIdOld
                    ,
                    Remark = s.Remark
                    ,
                    State = s.State
                    ,
                    CreateTime = s.CreateTime
                    ,
                    CreatePerson = s.CreatePerson
                    ,
                    UpdateTime = s.UpdateTime
                    ,
                    UpdatePerson = s.UpdatePerson


                })
            };
            return data;
        }

        #region 停缴员工列表 王帅
        /// <summary>
        /// 停缴员工列表
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult EmployeeStopModifyList([FromBody]GetDataParam getParam)
        {
            int total = 0;

            List<SingleStopPaymentViewDuty> queryData = m_BLL.GetEmployeeStopForDutyList(LoginInfo.UserID, getParam.page, getParam.rows, getParam.search, ref total);
            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData.Select(s => new SingleStopPaymentViewDuty()
                {

                    ID = s.ID,
                    CompanyID = s.CompanyID,
                    CompanyName = s.CompanyName,
                    EmployeeID = s.EmployeeID,
                    EmployeeName = s.EmployeeName,
                    CertificateNumber = s.CertificateNumber,
                    CanSotpInsuranceKindName = s.CanSotpInsuranceKindName,
                    EmployeeAddId = s.EmployeeAddId,
                    CompanyEmployeeRelationId = s.CompanyEmployeeRelationId,
                    CityName = s.CityName,
                    YearMonth = s.YearMonth,

                })
            };
            return data;
        }
        #endregion

        #region 责任客服修改 社保时间  获得社保类的方法 王帅
        /// <summary>
        /// 责任客服修改 社保时间  获得社保类的方法 王帅
        /// </summary>
        /// <param name="CompanyEmployeeRelationId">员工关系id</param>    
        /// <returns></returns>
        public List<StoppaymentTypeYearmonth> EmploeeStopInfo(int companyEmployeeRelationId)
        {
            SysEntities db = new SysEntities();
            var query2 = from d in db.EmployeeStopPayment
                         where d.EmployeeAdd.CompanyEmployeeRelationId == companyEmployeeRelationId && d.State == "待员工客服确认" && d.EmployeeAdd.State == "申报成功"
                         select new StoppaymentTypeYearmonth
                         {
                             ID = d.Id,
                             InsuranceKindId = d.EmployeeAdd.InsuranceKindId,
                             InsuranceMonth = d.InsuranceMonth,
                             PoliceOperationId = d.PoliceOperationId,
                             InsuranceKindName = d.EmployeeAdd.PoliceInsurance.InsuranceKind.Name
                         };


            List<StoppaymentTypeYearmonth> info = query2.ToList();

            return info;

        }

        #endregion

        #region 获取报减险种
        /// <summary>
        /// 获取报减险种
        /// </summary>
        /// <param name="CompanyEmployeeRelationId">员工关系id</param>    
        /// <returns></returns>
        public List<StoppaymentTypeYearmonth> EmploeeStop_Info(int companyEmployeeRelationId, string State)
        {
            SysEntities db = new SysEntities();
            var query2 = from d in db.EmployeeStopPayment
                         where d.EmployeeAdd.CompanyEmployeeRelationId == companyEmployeeRelationId
                         select new StoppaymentTypeYearmonth
                         {
                             ID = d.Id,
                             InsuranceKindId = d.EmployeeAdd.InsuranceKindId,
                             InsuranceMonth = d.InsuranceMonth,
                             PoliceOperationId = d.PoliceOperationId,
                             InsuranceKindName = d.EmployeeAdd.PoliceInsurance.InsuranceKind.Name
                         };


            List<StoppaymentTypeYearmonth> info = query2.ToList();

            return info;

        }

        #endregion


        #region 报减政策手续 王帅
        /// <summary>
        /// 报减政策手续
        /// </summary>
        /// <param name="ID">社保种类id</param>
        /// <returns></returns>
        public ActionResult getPoliceOperationList(int ID)
        {
            JsonResult jr = new JsonResult();
            IBLL.IEmployeeStopPaymentBLL eadd_BLL = new BLL.EmployeeStopPaymentBLL();
            List<idname__> list = eadd_BLL.getPoliceOperationid(ID);
            jr.Data = new JsonMessageResult<List<idname__>>("0000", "成功！", list);
            return jr;

        }
        #endregion



        #region 社保专员修改员工信息   王帅
        public Common.ClientResult.Result FeedbackModify([FromBody]PostInfo postinfos, string yanglao_id, string yiliao_id, string gongshang_id, string shiye_id, string gongjijin_id, string shengyu_id)
        {

            SysEntities SysEntitiesO2O = new SysEntities();
            try
            {


                int yanglao_id1 = 0;
                int yiliao_id1 = 0;
                int gongshang_id1 = 0;
                int shiye_id1 = 0;
                int gongjijin_id1 = 0;
                int shengyu_id1 = 0;
                int.TryParse(yanglao_id, out yanglao_id1);
                int.TryParse(yiliao_id, out yiliao_id1);
                int.TryParse(gongshang_id, out gongshang_id1);
                int.TryParse(shiye_id, out shiye_id1);
                int.TryParse(gongjijin_id, out gongjijin_id1);
                int.TryParse(shengyu_id, out shengyu_id1);

                using (TransactionScope scope = new TransactionScope())
                {
                    var EmployeeMiddle_BLL = new BLL.EmployeeMiddleBLL();
                    DateTime _NowDate = DateTime.Now.AddDays(-DateTime.Now.Day + 1);
                    DateTime QJ_Yanglao = DateTime.MinValue;
                    DateTime QJ_Yiliao = DateTime.MinValue;
                    DateTime QJ_Gongshang = DateTime.MinValue;
                    DateTime QJ_Shiye = DateTime.MinValue;
                    DateTime QJ_Gongjijin = DateTime.MinValue;
                    DateTime QJ_Shengyu = DateTime.MinValue;
                    DateTime.TryParse(postinfos.Pension_PaymentTime, out QJ_Yanglao);
                    DateTime.TryParse(postinfos.Medical_PaymentTime, out QJ_Yiliao);
                    DateTime.TryParse(postinfos.WorkInjury_PaymentTime, out QJ_Gongshang);
                    DateTime.TryParse(postinfos.Unemployment_PaymentTime, out QJ_Shiye);
                    DateTime.TryParse(postinfos.HousingFund_PaymentTime, out QJ_Gongjijin);
                    DateTime.TryParse(postinfos.Maternity_PaymentTime, out QJ_Shengyu);

                    #region 养老修改
                    if (QJ_Yanglao != DateTime.MinValue)
                    {


                        // var yanglao_EmployeeStopPayment = SysEntitiesO2O.EmployeeStopPayment.FirstOrDefault(p => p.Id == yanglao_id);

                        var yanglao_EmployeeStopPayment = SysEntitiesO2O.EmployeeStopPayment.FirstOrDefault(p => p.Id == yanglao_id1);
                        if (yanglao_EmployeeStopPayment != null)
                        {
                            string Enable = Status.启用.ToString();
                            int InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.养老;
                            if (DateTime.Compare((DateTime)yanglao_EmployeeStopPayment.InsuranceMonth, QJ_Yanglao) != 0)
                            {
                                yanglao_EmployeeStopPayment.InsuranceMonth = QJ_Yanglao;
                                yanglao_EmployeeStopPayment.UpdateTime = DateTime.Now;
                                yanglao_EmployeeStopPayment.UpdatePerson = LoginInfo.LoginName;

                                var EmployeeMiddle = SysEntitiesO2O.EmployeeMiddle.Where(o => o.CompanyEmployeeRelationId == yanglao_EmployeeStopPayment.EmployeeAdd.CompanyEmployeeRelationId && o.InsuranceKindId == InsuranceKindId && o.State == Enable);
                                var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(o => o.Id == yanglao_EmployeeStopPayment.EmployeeAdd.PoliceInsurance.Id);
                                if (PoliceInsurance != null)
                                {
                                    int a = (int)PoliceInsurance.InsuranceReduce;
                                    QJ_Yanglao = QJ_Yanglao.AddMonths(-a - 1);
                                }

                                string SBY = QJ_Yanglao.ToString("yyyyMM");
                                if (EmployeeMiddle.Count() > 0)
                                {
                                    foreach (var order in EmployeeMiddle)
                                    {

                                        order.EndedDate = int.Parse(SBY);
                                        order.UpdateTime = DateTime.Now;
                                        order.UpdatePerson = LoginInfo.LoginName;


                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region 医疗修改
                    if (QJ_Yiliao != DateTime.MinValue)
                    {

                        var EmployeeStopPayment = SysEntitiesO2O.EmployeeStopPayment.FirstOrDefault(p => p.Id == yiliao_id1);
                        if (EmployeeStopPayment != null)
                        {
                            string Enable = Status.启用.ToString();
                            int InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.医疗;
                            if (DateTime.Compare((DateTime)EmployeeStopPayment.InsuranceMonth, QJ_Yiliao) != 0)
                            {
                                EmployeeStopPayment.InsuranceMonth = QJ_Yiliao;
                                EmployeeStopPayment.UpdateTime = DateTime.Now;
                                EmployeeStopPayment.UpdatePerson = LoginInfo.LoginName;

                                var EmployeeMiddle = SysEntitiesO2O.EmployeeMiddle.Where(o => o.CompanyEmployeeRelationId == EmployeeStopPayment.EmployeeAdd.CompanyEmployeeRelationId && o.InsuranceKindId == InsuranceKindId && o.State == Enable);
                                var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(o => o.Id == EmployeeStopPayment.EmployeeAdd.PoliceInsurance.Id);
                                if (PoliceInsurance != null)
                                {
                                    int a = (int)PoliceInsurance.InsuranceReduce;
                                    QJ_Yiliao = QJ_Yiliao.AddMonths(-a - 1);
                                }

                                string SBY = QJ_Yiliao.ToString("yyyyMM");
                                if (EmployeeMiddle.Count() > 0)
                                {
                                    foreach (var order in EmployeeMiddle)
                                    {

                                        order.EndedDate = int.Parse(SBY);
                                        order.UpdateTime = DateTime.Now;
                                        order.UpdatePerson = LoginInfo.LoginName;


                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region 工伤修改
                    if (QJ_Gongshang != DateTime.MinValue)
                    {

                        var EmployeeStopPayment = SysEntitiesO2O.EmployeeStopPayment.FirstOrDefault(p => p.Id == gongshang_id1);
                        if (EmployeeStopPayment != null)
                        {
                            string Enable = Status.启用.ToString();
                            int InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.工伤;
                            if (DateTime.Compare((DateTime)EmployeeStopPayment.InsuranceMonth, QJ_Gongshang) != 0)
                            {
                                EmployeeStopPayment.InsuranceMonth = QJ_Gongshang;
                                EmployeeStopPayment.UpdateTime = DateTime.Now;
                                EmployeeStopPayment.UpdatePerson = LoginInfo.LoginName;

                                var EmployeeMiddle = SysEntitiesO2O.EmployeeMiddle.Where(o => o.CompanyEmployeeRelationId == EmployeeStopPayment.EmployeeAdd.CompanyEmployeeRelationId && o.InsuranceKindId == InsuranceKindId && o.State == Enable);
                                var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(o => o.Id == EmployeeStopPayment.EmployeeAdd.PoliceInsurance.Id);
                                if (PoliceInsurance != null)
                                {
                                    int a = (int)PoliceInsurance.InsuranceReduce;
                                    QJ_Gongshang = QJ_Gongshang.AddMonths(-a - 1);
                                }

                                string SBY = QJ_Gongshang.ToString("yyyyMM");
                                if (EmployeeMiddle.Count() > 0)
                                {
                                    foreach (var order in EmployeeMiddle)
                                    {

                                        order.EndedDate = int.Parse(SBY);
                                        order.UpdateTime = DateTime.Now;
                                        order.UpdatePerson = LoginInfo.LoginName;


                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region 失业修改
                    if (QJ_Shiye != DateTime.MinValue)
                    {

                        var EmployeeStopPayment = SysEntitiesO2O.EmployeeStopPayment.FirstOrDefault(p => p.Id == shiye_id1);
                        if (EmployeeStopPayment != null)
                        {
                            string Enable = Status.启用.ToString();
                            int InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.失业;
                            if (DateTime.Compare((DateTime)EmployeeStopPayment.InsuranceMonth, QJ_Shiye) != 0)
                            {
                                EmployeeStopPayment.InsuranceMonth = QJ_Shiye;
                                EmployeeStopPayment.UpdateTime = DateTime.Now;
                                EmployeeStopPayment.UpdatePerson = LoginInfo.LoginName;

                                var EmployeeMiddle = SysEntitiesO2O.EmployeeMiddle.Where(o => o.CompanyEmployeeRelationId == EmployeeStopPayment.EmployeeAdd.CompanyEmployeeRelationId && o.InsuranceKindId == InsuranceKindId && o.State == Enable);
                                var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(o => o.Id == EmployeeStopPayment.EmployeeAdd.PoliceInsurance.Id);
                                if (PoliceInsurance != null)
                                {
                                    int a = (int)PoliceInsurance.InsuranceReduce;
                                    QJ_Shiye = QJ_Shiye.AddMonths(-a - 1);
                                }

                                string SBY = QJ_Shiye.ToString("yyyyMM");
                                if (EmployeeMiddle.Count() > 0)
                                {
                                    foreach (var order in EmployeeMiddle)
                                    {

                                        order.EndedDate = int.Parse(SBY);
                                        order.UpdateTime = DateTime.Now;
                                        order.UpdatePerson = LoginInfo.LoginName;


                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region 公积金修改
                    if (QJ_Gongjijin != DateTime.MinValue)
                    {

                        var EmployeeStopPayment = SysEntitiesO2O.EmployeeStopPayment.FirstOrDefault(p => p.Id == gongjijin_id1);
                        if (EmployeeStopPayment != null)
                        {
                            string Enable = Status.启用.ToString();
                            int InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.公积金;
                            if (DateTime.Compare((DateTime)EmployeeStopPayment.InsuranceMonth, QJ_Gongjijin) != 0)
                            {
                                EmployeeStopPayment.InsuranceMonth = QJ_Gongjijin;
                                EmployeeStopPayment.UpdateTime = DateTime.Now;
                                EmployeeStopPayment.UpdatePerson = LoginInfo.LoginName;

                                var EmployeeMiddle = SysEntitiesO2O.EmployeeMiddle.Where(o => o.CompanyEmployeeRelationId == EmployeeStopPayment.EmployeeAdd.CompanyEmployeeRelationId && o.InsuranceKindId == InsuranceKindId && o.State == Enable);
                                var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(o => o.Id == EmployeeStopPayment.EmployeeAdd.PoliceInsurance.Id);
                                if (PoliceInsurance != null)
                                {
                                    int a = (int)PoliceInsurance.InsuranceReduce;
                                    QJ_Gongjijin = QJ_Gongjijin.AddMonths(-a - 1);
                                }

                                string SBY = QJ_Gongjijin.ToString("yyyyMM");
                                if (EmployeeMiddle.Count() > 0)
                                {
                                    foreach (var order in EmployeeMiddle)
                                    {

                                        order.EndedDate = int.Parse(SBY);
                                        order.UpdateTime = DateTime.Now;
                                        order.UpdatePerson = LoginInfo.LoginName;


                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region 生育修改
                    if (QJ_Shengyu != DateTime.MinValue)
                    {

                        var EmployeeStopPayment = SysEntitiesO2O.EmployeeStopPayment.FirstOrDefault(p => p.Id == shengyu_id1);
                        if (EmployeeStopPayment != null)
                        {
                            string Enable = Status.启用.ToString();
                            int InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.生育;
                            if (DateTime.Compare((DateTime)EmployeeStopPayment.InsuranceMonth, QJ_Shengyu) != 0)
                            {
                                EmployeeStopPayment.InsuranceMonth = QJ_Shengyu;
                                EmployeeStopPayment.UpdateTime = DateTime.Now;
                                EmployeeStopPayment.UpdatePerson = LoginInfo.LoginName;

                                var EmployeeMiddle = SysEntitiesO2O.EmployeeMiddle.Where(o => o.CompanyEmployeeRelationId == EmployeeStopPayment.EmployeeAdd.CompanyEmployeeRelationId && o.InsuranceKindId == InsuranceKindId && o.State == Enable);
                                var PoliceInsurance = SysEntitiesO2O.PoliceInsurance.FirstOrDefault(o => o.Id == EmployeeStopPayment.EmployeeAdd.PoliceInsurance.Id);
                                if (PoliceInsurance != null)
                                {
                                    int a = (int)PoliceInsurance.InsuranceReduce;
                                    QJ_Shengyu = QJ_Shengyu.AddMonths(-a - 1);
                                }

                                string SBY = QJ_Shengyu.ToString("yyyyMM");
                                if (EmployeeMiddle.Count() > 0)
                                {
                                    foreach (var order in EmployeeMiddle)
                                    {

                                        order.EndedDate = int.Parse(SBY);
                                        order.UpdateTime = DateTime.Now;
                                        order.UpdatePerson = LoginInfo.LoginName;


                                    }
                                }
                            }
                        }
                    }
                    #endregion



                    //9.最后保存
                    SysEntitiesO2O.SaveChanges();
                    scope.Complete();
                    Common.ClientResult.Result result = new Common.ClientResult.Result();
                    result.Code = ClientCode.Succeed;
                    result.Message = "修改成功";
                    return result;
                }
            }
            catch (Exception er)
            {
                Common.ClientResult.Result result = new Common.ClientResult.Result();
                result.Code = ClientCode.Fail;
                result.Message = "修改失败";
                return result;
            }

        }
        #endregion

        #region 社保专员修改员工信息   
        public Common.ClientResult.Result Update_style([FromBody]PostInfo postinfos, string yanglao_id, string yiliao_id, string gongshang_id, string shiye_id, string gongjijin_id, string shengyu_id, string yanglao, string yiliao, string shiye, string gongshang, string gongjijin, string shengyu)
        {

            SysEntities SysEntitiesO2O = new SysEntities();
            try
            {


                int yanglao_id1 = 0;
                int yiliao_id1 = 0;
                int gongshang_id1 = 0;
                int shiye_id1 = 0;
                int gongjijin_id1 = 0;
                int shengyu_id1 = 0;

                int yanglao_1 = 0;
                int yiliao_1 = 0;
                int gongshang_1 = 0;
                int shiye_1 = 0;
                int gongjijin_1 = 0;
                int shengyu_1 = 0;
                int.TryParse(yanglao_id, out yanglao_id1);
                int.TryParse(yiliao_id, out yiliao_id1);
                int.TryParse(gongshang_id, out gongshang_id1);
                int.TryParse(shiye_id, out shiye_id1);
                int.TryParse(gongjijin_id, out gongjijin_id1);
                int.TryParse(shengyu_id, out shengyu_id1);

                int.TryParse(yanglao, out yanglao_1);
                int.TryParse(yiliao, out yiliao_1);
                int.TryParse(gongshang, out gongshang_1);
                int.TryParse(shiye, out shiye_1);
                int.TryParse(gongjijin, out gongjijin_1);
                int.TryParse(shengyu, out shengyu_1);

                using (TransactionScope scope = new TransactionScope())
                {
                    var EmployeeMiddle_BLL = new BLL.EmployeeMiddleBLL();


                    #region 养老修改
                    if (yanglao_1 != 0)
                    {


                        // var yanglao_EmployeeStopPayment = SysEntitiesO2O.EmployeeStopPayment.FirstOrDefault(p => p.Id == yanglao_id);

                        var yanglao_EmployeeStopPayment = SysEntitiesO2O.EmployeeStopPayment.FirstOrDefault(p => p.Id == yanglao_id1);
                        if (yanglao_EmployeeStopPayment != null)
                        {
                            string Enable = Status.启用.ToString();
                            int InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.养老;

                            yanglao_EmployeeStopPayment.PoliceOperationId = yanglao_1;
                            yanglao_EmployeeStopPayment.UpdateTime = DateTime.Now;
                            yanglao_EmployeeStopPayment.UpdatePerson = LoginInfo.LoginName;


                        }
                    }
                    #endregion

                    #region 医疗修改
                    if (yiliao_1 != 0)
                    {

                        var EmployeeStopPayment = SysEntitiesO2O.EmployeeStopPayment.FirstOrDefault(p => p.Id == yiliao_id1);
                        if (EmployeeStopPayment != null)
                        {
                            string Enable = Status.启用.ToString();
                            int InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.医疗;

                            EmployeeStopPayment.PoliceOperationId = yiliao_1;
                            EmployeeStopPayment.UpdateTime = DateTime.Now;
                            EmployeeStopPayment.UpdatePerson = LoginInfo.LoginName;



                        }
                    }
                    #endregion

                    #region 工伤修改
                    if (gongshang_1 != 0)
                    {

                        var EmployeeStopPayment = SysEntitiesO2O.EmployeeStopPayment.FirstOrDefault(p => p.Id == gongshang_id1);
                        if (EmployeeStopPayment != null)
                        {
                            string Enable = Status.启用.ToString();
                            int InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.工伤;
                            EmployeeStopPayment.PoliceOperationId = gongshang_1;
                            EmployeeStopPayment.UpdateTime = DateTime.Now;
                            EmployeeStopPayment.UpdatePerson = LoginInfo.LoginName;


                        }
                    }
                    #endregion

                    #region 失业修改
                    if (shiye_1 != 0)
                    {

                        var EmployeeStopPayment = SysEntitiesO2O.EmployeeStopPayment.FirstOrDefault(p => p.Id == shiye_id1);
                        if (EmployeeStopPayment != null)
                        {
                            string Enable = Status.启用.ToString();
                            int InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.失业;
                            EmployeeStopPayment.PoliceOperationId = shiye_1;
                            EmployeeStopPayment.UpdateTime = DateTime.Now;
                            EmployeeStopPayment.UpdatePerson = LoginInfo.LoginName;


                        }
                    }
                    #endregion

                    #region 公积金修改
                    if (gongjijin_1 != 0)
                    {

                        var EmployeeStopPayment = SysEntitiesO2O.EmployeeStopPayment.FirstOrDefault(p => p.Id == gongjijin_id1);
                        if (EmployeeStopPayment != null)
                        {
                            string Enable = Status.启用.ToString();
                            int InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.公积金;

                            EmployeeStopPayment.PoliceOperationId = gongjijin_1;
                            EmployeeStopPayment.UpdateTime = DateTime.Now;
                            EmployeeStopPayment.UpdatePerson = LoginInfo.LoginName;


                        }
                    }
                    #endregion

                    #region 生育修改
                    if (shengyu_1 != 0)
                    {

                        var EmployeeStopPayment = SysEntitiesO2O.EmployeeStopPayment.FirstOrDefault(p => p.Id == shengyu_id1);
                        if (EmployeeStopPayment != null)
                        {
                            string Enable = Status.启用.ToString();
                            int InsuranceKindId = (int)EmployeeAdd_InsuranceKindId.生育;

                            EmployeeStopPayment.PoliceOperationId = shengyu_1;
                            EmployeeStopPayment.UpdateTime = DateTime.Now;
                            EmployeeStopPayment.UpdatePerson = LoginInfo.LoginName;



                        }
                    }
                    #endregion



                    //9.最后保存
                    SysEntitiesO2O.SaveChanges();

                    Common.ClientResult.Result result = new Common.ClientResult.Result();
                    result.Code = ClientCode.Succeed;
                    result.Message = "修改成功";
                    return result;
                }
            }
            catch (Exception er)
            {
                Common.ClientResult.Result result = new Common.ClientResult.Result();
                result.Code = ClientCode.Fail;
                result.Message = "修改失败";
                return result;
            }

        }
        #endregion

        /// <summary>
        /// 根据ID获取数据模型
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public EmployeeStopPayment Get(int id)
        {
            EmployeeStopPayment item = m_BLL.GetById(id);
            return item;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public Common.ClientResult.Result Post([FromBody]EmployeeStopPayment entity)
        {

            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {
                //string currentPerson = GetCurrentPerson();
                //entity.CreateTime = DateTime.Now;
                //entity.CreatePerson = currentPerson;


                string returnValue = string.Empty;
                if (m_BLL.Create(ref validationErrors, entity))
                {
                    LogClassModels.WriteServiceLog(Suggestion.InsertSucceed + "，员工停缴的信息的Id为" + entity.Id, "员工停缴"
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
                    LogClassModels.WriteServiceLog(Suggestion.InsertFail + "，员工停缴的信息，" + returnValue, "员工停缴"
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

        // PUT api/<controller>/5
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>  
        public Common.ClientResult.Result Put([FromBody]EmployeeStopPayment entity)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();
            if (entity != null && ModelState.IsValid)
            {   //数据校验

                //string currentPerson = GetCurrentPerson();
                //entity.UpdateTime = DateTime.Now;
                //entity.UpdatePerson = currentPerson;

                string returnValue = string.Empty;
                if (m_BLL.Edit(ref validationErrors, entity))
                {
                    LogClassModels.WriteServiceLog(Suggestion.UpdateSucceed + "，员工停缴信息的Id为" + entity.Id, "员工停缴"
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
                    LogClassModels.WriteServiceLog(Suggestion.UpdateFail + "，员工停缴信息的Id为" + entity.Id + "," + returnValue, "员工停缴"
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

        // DELETE api/<controller>/5
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>  
        public Common.ClientResult.Result Delete(string query)
        {
            Common.ClientResult.Result result = new Common.ClientResult.Result();

            string returnValue = string.Empty;
            int[] deleteId = Array.ConvertAll<string, int>(query.Split(','), delegate(string s) { return int.Parse(s); });
            if (deleteId != null && deleteId.Length > 0)
            {
                if (m_BLL.DeleteCollection(ref validationErrors, deleteId))
                {
                    LogClassModels.WriteServiceLog(Suggestion.DeleteSucceed + "，信息的Id为" + string.Join(",", deleteId), "消息"
                        );//删除成功，写入日志
                    result.Code = Common.ClientCode.Succeed;
                    result.Message = Suggestion.DeleteSucceed;
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
                    LogClassModels.WriteServiceLog(Suggestion.DeleteFail + "，信息的Id为" + string.Join(",", deleteId) + "," + returnValue, "消息"
                        );//删除失败，写入日志
                    result.Code = Common.ClientCode.Fail;
                    result.Message = Suggestion.DeleteFail + returnValue;
                }
            }
            return result;
        }

        IBLL.IEmployeeStopPaymentBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public EmployeeStopPaymentApiController()
            : this(new EmployeeStopPaymentBLL()) { }

        public EmployeeStopPaymentApiController(EmployeeStopPaymentBLL bll)
        {
            m_BLL = bll;
        }



        #region 社保报减查询列表

        /// <summary>
        ///社保报减查询列表
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostEmployeeStopViewList([FromBody]GetDataParam getParam)
        {
            int total = 0;
            string search = getParam.search;
            //当没有险种查询条件时,拒绝进行查询

            var data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = new List<EmployeeStopView>()
            };
            if (string.IsNullOrWhiteSpace(search) || !search.Contains("InsuranceKinds"))
            {
                return data;
            }
            List<ORG_User> userList = getSubordinatesData(Common.ORG_Group_Code.SBKF.ToString(), "1016");      //	报增信息查询

            userList.AddRange(getSubordinatesData(Common.ORG_Group_Code.YGKF.ToString(), "1016"));
            userList.AddRange(getSubordinatesData(Common.ORG_Group_Code.ZRKF.ToString(), "1016"));
            List<EmployeeStopView> queryData = m_BLL.GetEmployeeStopList(getParam.page, getParam.rows, search, userList, ref total);
            data = new Common.ClientResult.DataResult
            {
                total = total,
                rows = queryData
            };
            return data;
        }
        #endregion

        #region 社保停缴信息查询列表-导出

        /// <summary>
        /// 社保停缴查询导出
        /// </summary>
        /// <param name="search">检索条件</param>
        /// <returns></returns>
        public Common.ClientResult.UrlResult PostEmployeeStopViewListForExcel(string search)
        {
            int total = 0;

            search = HttpUtility.HtmlDecode(search);

            //当没有险种查询条件时,拒绝进行查询
            if (string.IsNullOrWhiteSpace(search) || !search.Contains("InsuranceKinds"))
            {
                var data = new Common.ClientResult.UrlResult
                {
                    Code = ClientCode.FindNull,
                    Message = "没有符合条件的数据",
                };
                return data;
            }

            List<ORG_User> userList = getSubordinatesData(Common.ORG_Group_Code.SBKF.ToString(), "1016");      //	报增信息查询

            userList.AddRange(getSubordinatesData(Common.ORG_Group_Code.YGKF.ToString(), "1016"));
            userList.AddRange(getSubordinatesData(Common.ORG_Group_Code.ZRKF.ToString(), "1016"));
            List<EmployeeStopView> queryData = m_BLL.GetEmployeeStopListForExcel(search, userList);

            #region 停缴信息导出
            FileStream file = new FileStream(System.Web.HttpContext.Current.Server.MapPath("../../Template/Excel/停缴信息查询导出模版.xls"), FileMode.Open, FileAccess.Read);
            HSSFWorkbook workbook = new HSSFWorkbook(file);

            try
            {
                string excelName = Guid.NewGuid().ToString() + "停缴信息查询导出";
                using (MemoryStream ms = new MemoryStream())
                {
                    workbook.SetSheetName(0, "停缴查询导出");
                    ISheet sheet = workbook.GetSheetAt(0);
                    int rowNum = 2;
                    //IRow currentRow = sheet.CreateRow(rowNum);
                    // int colNum = 0;

                    for (int i = 0; i < queryData.Count; i++)
                    {
                        int colnum = 0;

                        IRow currentRow = sheet.CreateRow(rowNum);

                        // 基础信息
                        ICell cell = currentRow.CreateCell(colnum);
                        cell = currentRow.CreateCell(colnum);
                        cell.SetCellValue(queryData[i].Operator_CompanyName);
                        colnum++;
                        ICell cell1 = currentRow.CreateCell(colnum);
                        cell1 = currentRow.CreateCell(colnum);
                        cell1.SetCellValue(queryData[i].CompanyName);
                        colnum++;
                        ICell cell2 = currentRow.CreateCell(colnum);
                        cell2 = currentRow.CreateCell(colnum);
                        cell2.SetCellValue(queryData[i].Name);
                        colnum++;
                        ICell cell3 = currentRow.CreateCell(colnum);
                        cell3 = currentRow.CreateCell(colnum);
                        cell3.SetCellValue(queryData[i].CertificateNumber);
                        colnum++;
                        ICell cell4 = currentRow.CreateCell(colnum);
                        cell4 = currentRow.CreateCell(colnum);
                        cell4.SetCellValue(queryData[i].City);
                        colnum++;
                        ICell cell5 = currentRow.CreateCell(colnum);
                        cell5 = currentRow.CreateCell(colnum);
                        cell5.SetCellValue(queryData[i].PoliceAccountNatureName);
                        colnum++;

                        //养老
                        ICell cell_yl1 = currentRow.CreateCell(colnum);
                        cell_yl1 = currentRow.CreateCell(colnum);
                        cell_yl1.SetCellValue(queryData[i].PoliceOperationName_1);
                        colnum++;
                        ICell cell_yl2 = currentRow.CreateCell(colnum);
                        cell_yl2 = currentRow.CreateCell(colnum);
                        cell_yl2.SetCellValue(queryData[i].InsuranceMonth_1 == null ? "" : queryData[i].InsuranceMonth_1.ToString());
                        colnum++;
                        ICell cell_yl8 = currentRow.CreateCell(colnum);
                        cell_yl8 = currentRow.CreateCell(colnum);
                        cell_yl8.SetCellValue(queryData[i].State_1);
                        colnum++;
                        ICell cell_yl9 = currentRow.CreateCell(colnum);
                        cell_yl9 = currentRow.CreateCell(colnum);
                        cell_yl9.SetCellValue(queryData[i].PoliceInsuranceName_1);
                        colnum++;
                        ICell cell_yl10 = currentRow.CreateCell(colnum);
                        cell_yl10 = currentRow.CreateCell(colnum);
                        cell_yl10.SetCellValue(queryData[i].YearMonth_1 == null ? "" : queryData[i].YearMonth_1.ToString());
                        colnum++;

                        //医疗
                        ICell cell_yiliao1 = currentRow.CreateCell(colnum);
                        cell_yiliao1 = currentRow.CreateCell(colnum);
                        cell_yiliao1.SetCellValue(queryData[i].PoliceOperationName_2);
                        colnum++;
                        ICell cell_yiliao2 = currentRow.CreateCell(colnum);
                        cell_yiliao2 = currentRow.CreateCell(colnum);
                        cell_yiliao2.SetCellValue(queryData[i].InsuranceMonth_2 == null ? "" : queryData[i].InsuranceMonth_2.ToString());
                        colnum++;
                        ICell cell_yiliao8 = currentRow.CreateCell(colnum);
                        cell_yiliao8 = currentRow.CreateCell(colnum);
                        cell_yiliao8.SetCellValue(queryData[i].State_2);
                        colnum++;
                        ICell cell_yiliao9 = currentRow.CreateCell(colnum);
                        cell_yiliao9 = currentRow.CreateCell(colnum);
                        cell_yiliao9.SetCellValue(queryData[i].PoliceInsuranceName_2);
                        colnum++;
                        ICell cell_yiliao10 = currentRow.CreateCell(colnum);
                        cell_yiliao10 = currentRow.CreateCell(colnum);
                        cell_yiliao10.SetCellValue(queryData[i].YearMonth_2 == null ? "" : queryData[i].YearMonth_2.ToString());
                        colnum++;

                        //工伤
                        ICell cell_gongshang1 = currentRow.CreateCell(colnum);
                        cell_gongshang1 = currentRow.CreateCell(colnum);
                        cell_gongshang1.SetCellValue(queryData[i].PoliceOperationName_3);
                        colnum++;
                        ICell cell_gongshang2 = currentRow.CreateCell(colnum);
                        cell_gongshang2 = currentRow.CreateCell(colnum);
                        cell_gongshang2.SetCellValue(queryData[i].InsuranceMonth_3 == null ? "" : queryData[i].InsuranceMonth_3.ToString());
                        colnum++;
                        ICell cell_gongshang8 = currentRow.CreateCell(colnum);
                        cell_gongshang8 = currentRow.CreateCell(colnum);
                        cell_gongshang8.SetCellValue(queryData[i].State_3);
                        colnum++;
                        ICell cell_gongshang9 = currentRow.CreateCell(colnum);
                        cell_gongshang9 = currentRow.CreateCell(colnum);
                        cell_gongshang9.SetCellValue(queryData[i].PoliceInsuranceName_3);
                        colnum++;
                        ICell cell_gongshang10 = currentRow.CreateCell(colnum);
                        cell_gongshang10 = currentRow.CreateCell(colnum);
                        cell_gongshang10.SetCellValue(queryData[i].YearMonth_3 == null ? "" : queryData[i].YearMonth_3.ToString());
                        colnum++;

                        //失业
                        ICell cell_shiye1 = currentRow.CreateCell(colnum);
                        cell_shiye1 = currentRow.CreateCell(colnum);
                        cell_shiye1.SetCellValue(queryData[i].PoliceOperationName_4);
                        colnum++;
                        ICell cell_shiye2 = currentRow.CreateCell(colnum);
                        cell_shiye2 = currentRow.CreateCell(colnum);
                        cell_shiye2.SetCellValue(queryData[i].InsuranceMonth_4 == null ? "" : queryData[i].InsuranceMonth_3.ToString());
                        colnum++;
                        ICell cell_shiye8 = currentRow.CreateCell(colnum);
                        cell_shiye8 = currentRow.CreateCell(colnum);
                        cell_shiye8.SetCellValue(queryData[i].State_4);
                        colnum++;
                        ICell cell_shiye9 = currentRow.CreateCell(colnum);
                        cell_shiye9 = currentRow.CreateCell(colnum);
                        cell_shiye9.SetCellValue(queryData[i].PoliceInsuranceName_4);
                        colnum++;
                        ICell cell_shiye10 = currentRow.CreateCell(colnum);
                        cell_shiye10 = currentRow.CreateCell(colnum);
                        cell_shiye10.SetCellValue(queryData[i].YearMonth_4 == null ? "" : queryData[i].YearMonth_4.ToString());
                        colnum++;

                        //公积金
                        ICell cell_gongjijin1 = currentRow.CreateCell(colnum);
                        cell_gongjijin1 = currentRow.CreateCell(colnum);
                        cell_gongjijin1.SetCellValue(queryData[i].PoliceOperationName_5);
                        colnum++;
                        ICell cell_gongjijin2 = currentRow.CreateCell(colnum);
                        cell_gongjijin2 = currentRow.CreateCell(colnum);
                        cell_gongjijin2.SetCellValue(queryData[i].InsuranceMonth_5 == null ? "" : queryData[i].InsuranceMonth_5.ToString());
                        colnum++;
                        ICell cell_gongjijin8 = currentRow.CreateCell(colnum);
                        cell_gongjijin8 = currentRow.CreateCell(colnum);
                        cell_gongjijin8.SetCellValue(queryData[i].State_5);
                        colnum++;
                        ICell cell_gongjijin9 = currentRow.CreateCell(colnum);
                        cell_gongjijin9 = currentRow.CreateCell(colnum);
                        cell_gongjijin9.SetCellValue(queryData[i].PoliceInsuranceName_5);
                        colnum++;
                        ICell cell_gongjijin10 = currentRow.CreateCell(colnum);
                        cell_gongjijin10 = currentRow.CreateCell(colnum);
                        cell_gongjijin10.SetCellValue(queryData[i].YearMonth_5 == null ? "" : queryData[i].YearMonth_5.ToString());
                        colnum++;

                        //生育
                        ICell cell_shengyu1 = currentRow.CreateCell(colnum);
                        cell_shengyu1 = currentRow.CreateCell(colnum);
                        cell_shengyu1.SetCellValue(queryData[i].PoliceOperationName_6);
                        colnum++;
                        ICell cell_shengyu2 = currentRow.CreateCell(colnum);
                        cell_shengyu2 = currentRow.CreateCell(colnum);
                        cell_shengyu2.SetCellValue(queryData[i].InsuranceMonth_6 == null ? "" : queryData[i].InsuranceMonth_6.ToString());
                        colnum++;
                        ICell cell_shengyu8 = currentRow.CreateCell(colnum);
                        cell_shengyu8 = currentRow.CreateCell(colnum);
                        cell_shengyu8.SetCellValue(queryData[i].State_6);
                        colnum++;
                        ICell cell_shengyu9 = currentRow.CreateCell(colnum);
                        cell_shengyu9 = currentRow.CreateCell(colnum);
                        cell_shengyu9.SetCellValue(queryData[i].PoliceInsuranceName_6);
                        colnum++;
                        ICell cell_shengyu10 = currentRow.CreateCell(colnum);
                        cell_shengyu10 = currentRow.CreateCell(colnum);
                        cell_shengyu10.SetCellValue(queryData[i].YearMonth_6 == null ? "" : queryData[i].YearMonth_6.ToString());
                        colnum++;

                        //补充公积金工资
                        ICell cell_bcgongjijin1 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin1 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin1.SetCellValue(queryData[i].PoliceOperationName_7);
                        colnum++;
                        ICell cell_bcgongjijin2 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin2 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin2.SetCellValue(queryData[i].InsuranceMonth_7 == null ? "" : queryData[i].InsuranceMonth_7.ToString());
                        colnum++;
                        ICell cell_bcgongjijin8 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin8 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin8.SetCellValue(queryData[i].State_7);
                        colnum++;
                        ICell cell_bcgongjijin9 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin9 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin9.SetCellValue(queryData[i].PoliceInsuranceName_7);
                        colnum++;
                        ICell cell_bcgongjijin10 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin10 = currentRow.CreateCell(colnum);
                        cell_bcgongjijin10.SetCellValue(queryData[i].YearMonth_7 == null ? "" : queryData[i].YearMonth_7.ToString());
                        colnum++;

                        //大病工资
                        ICell cell_dbgongzi1 = currentRow.CreateCell(colnum);
                        cell_dbgongzi1 = currentRow.CreateCell(colnum);
                        cell_dbgongzi1.SetCellValue(queryData[i].PoliceOperationName_8);
                        colnum++;
                        ICell cell_dbgongzi2 = currentRow.CreateCell(colnum);
                        cell_dbgongzi2 = currentRow.CreateCell(colnum);
                        cell_dbgongzi2.SetCellValue(queryData[i].InsuranceMonth_8 == null ? "" : queryData[i].InsuranceMonth_8.ToString());
                        colnum++;
                        ICell cell_dbgongzi8 = currentRow.CreateCell(colnum);
                        cell_dbgongzi8 = currentRow.CreateCell(colnum);
                        cell_dbgongzi8.SetCellValue(queryData[i].State_8);
                        colnum++;
                        ICell cell_dbgongzi9 = currentRow.CreateCell(colnum);
                        cell_dbgongzi9 = currentRow.CreateCell(colnum);
                        cell_dbgongzi9.SetCellValue(queryData[i].PoliceInsuranceName_8);
                        colnum++;
                        ICell cell_dbgongzi10 = currentRow.CreateCell(colnum);
                        cell_dbgongzi10 = currentRow.CreateCell(colnum);
                        cell_dbgongzi10.SetCellValue(queryData[i].YearMonth_8 == null ? "" : queryData[i].YearMonth_8.ToString());
                        colnum++;

                        //大病
                        ICell cell_dabing1 = currentRow.CreateCell(colnum);
                        cell_dabing1 = currentRow.CreateCell(colnum);
                        cell_dabing1.SetCellValue(queryData[i].PoliceOperationName_9);
                        colnum++;
                        ICell cell_dabing2 = currentRow.CreateCell(colnum);
                        cell_dabing2 = currentRow.CreateCell(colnum);
                        cell_dabing2.SetCellValue(queryData[i].InsuranceMonth_9 == null ? "" : queryData[i].InsuranceMonth_9.ToString());
                        colnum++;
                        ICell cell_dabing8 = currentRow.CreateCell(colnum);
                        cell_dabing8 = currentRow.CreateCell(colnum);
                        cell_dabing8.SetCellValue(queryData[i].State_9);
                        colnum++;
                        ICell cell_dabing9 = currentRow.CreateCell(colnum);
                        cell_dabing9 = currentRow.CreateCell(colnum);
                        cell_dabing9.SetCellValue(queryData[i].PoliceInsuranceName_9);
                        colnum++;
                        ICell cell_dabing10 = currentRow.CreateCell(colnum);
                        cell_dabing10 = currentRow.CreateCell(colnum);
                        cell_dabing10.SetCellValue(queryData[i].YearMonth_9 == null ? "" : queryData[i].YearMonth_9.ToString());
                        colnum++;

                        rowNum++;
                    }

                    sheet.ForceFormulaRecalculation = true;
                    string fileName = excelName + ".xls";
                    string urlPath = "DataExport/" + fileName; // 文件下载的URL地址，供给前台下载
                    string filePath = System.Web.HttpContext.Current.Server.MapPath("\\" + urlPath); // 文件路径

                    file = new FileStream(filePath, FileMode.Create);
                    workbook.Write(file);
                    file.Close();

                    if (queryData.Count == 0)
                    {
                        var data = new Common.ClientResult.UrlResult
                        {
                            Code = ClientCode.FindNull,
                            Message = "没有符合条件的数据",
                            URL = urlPath
                        };
                        return data;
                    }
                    string Message = "已成功提取停缴信息";

                    return new Common.ClientResult.UrlResult
                    {
                        Code = ClientCode.Succeed,
                        Message = Message,
                        URL = urlPath
                    };
                }
            }
            catch (Exception e)
            {
                file.Close();
                return new Common.ClientResult.UrlResult
                {
                    Code = ClientCode.Fail,
                    Message = e.Message
                };
            }
            #endregion
        }
        #endregion



        #region 社保专员提取报减信息列表

        /// <summary>
        /// 社保专员提取报减信息列表
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostSupplierList([FromBody]GetDataParam getParam)
        {
            int total = 0;
            string search = getParam.search;

            if (!string.IsNullOrWhiteSpace(search))
            {
                Dictionary<string, string> queryDic = ValueConvert.StringToDictionary(search.GetString());
                if (queryDic != null && queryDic.Count > 0)
                {
                    if (queryDic.ContainsKey("CollectState") && !string.IsNullOrWhiteSpace(queryDic["CollectState"]))
                    {
                        string str = queryDic["CollectState"];
                        if (str.Contains(Common.CollectState.未提取.ToString()))
                        {
                            string state = Common.EmployeeAdd_State.员工客服已确认.ToString();
                            search += "State&" + state + "^";
                        }
                        else
                        {
                            string state = Common.EmployeeAdd_State.社保专员已提取.ToString();
                            search += "State&" + state + "^";
                        }
                    }
                }
            }
            else
            {
                string state = Common.EmployeeAdd_State.员工客服已确认.ToString();
                search = "State&" + state + "^";
            }
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


        #region 社保专员导出报减信息列表

        /// <summary>
        /// 社保专员导出报减信息列表
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.UrlResult SupplierExport([FromBody]GetDataParam getParam)
        {



            FileStream file = new FileStream(System.Web.HttpContext.Current.Server.MapPath("../../Images/供应商导出.xls"), FileMode.Open, FileAccess.Read);
            HSSFWorkbook workbook = new HSSFWorkbook(file);
            try
            {
                string search = getParam.search;
                int getid = 0;
                int total = 0;


                Dictionary<string, string> queryDic = ValueConvert.StringToDictionary(search.GetString());
                if (queryDic != null && queryDic.Count > 0)
                {
                    if (queryDic.ContainsKey("CollectState") && !string.IsNullOrWhiteSpace(queryDic["CollectState"]))
                    {
                        string str = queryDic["CollectState"];
                        if (str.Equals(Common.CollectState.未提取.ToString()))
                        {
                            string state = Common.EmployeeAdd_State.员工客服已确认.ToString();
                            search += "State&" + state + "^";
                        }
                        else if (str.Equals(Common.CollectState.已提取.ToString()))
                        {
                            string state = Common.EmployeeAdd_State.社保专员已提取.ToString();
                            search += "State&" + state + "^";
                        }
                    }
                }
                search += "UserID_SB&" + LoginInfo.UserID + "^";
                string excelName = "供应商导出" + DateTime.Now.ToString();
                using (MemoryStream ms = new MemoryStream())
                {


                    workbook.SetSheetName(0, "供应商导出");
                    List<ORG_User> userList = new List<ORG_User>();
                    ORG_User user= new ORG_User();
                    user.ID = LoginInfo.UserID;
                    userList.Add(user);
                    List<EmployeeStopView> queryData = m_BLL.GetEmployeeStopList(1, int.MaxValue, search,userList, ref total);
                    // List<EmployeeApprove> queryData = m_BLL.GetApproveListByParam(getid, search);
                    string ids = string.Empty;


                    //  IWorkbook workbook = new HSSFWorkbook();
                    //员工社保一览
                    ISheet sheet = workbook.GetSheetAt(0);
                    int rowNum = 0;
                    IRow currentRow = sheet.CreateRow(rowNum);

                    int colNum = 0;


                    ICell cell = currentRow.CreateCell(colNum);

                    #region 表头



                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("企业编号");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("企业名称");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("员工姓名");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("员工证件号");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("工作岗位");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("缴纳地");
                    colNum++;




                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("养老政策手续");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("养老社保政策");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("养老报减自然月");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("养老社保月");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("养老是否单立户");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("养老社保编号");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("养老创建时间");
                    colNum++;




                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("医疗政策手续");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("医疗社保政策");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("医疗报减自然月");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("医疗社保月");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("医疗是否单立户");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("医疗社保编号");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("医疗创建时间");
                    colNum++;



                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("工伤政策手续");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("工伤社保政策");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("工伤报减自然月");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("工伤社保月");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("工伤是否单立户");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("工伤社保编号");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("工伤创建时间");
                    colNum++;




                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("失业政策手续");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("失业社保政策");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("失业报减自然月");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("失业社保月");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("失业是否单立户");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("失业社保编号");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("失业创建时间");
                    colNum++;




                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("公积金政策手续");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("公积金社保政策");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("公积金报减自然月");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("公积金社保月");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("公积金是否单立户");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("公积金社保编号");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("公积金创建时间");
                    colNum++;




                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("生育政策手续");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("生育社保政策");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("生育报减自然月");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("生育社保月");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("生育是否单立户");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("生育社保编号");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("生育创建时间");
                    colNum++;


                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("补充公积金政策手续");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("补充公积金社保政策");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("补充公积金报减自然月");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("补充公积金社保月");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("补充公积金是否单立户");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("补充公积金社保编号");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("补充公积金创建时间");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("大病政策手续");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("大病社保政策");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("大病报减自然月");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("大病社保月");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("大病是否单立户");
                    colNum++;
                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("大病社保编号");
                    colNum++;

                    cell = currentRow.CreateCell(colNum);
                    cell.SetCellValue("大病创建时间");
                    colNum++;




                   

                    #endregion

                    #region 导出Excel内容

                    for (int i = 0; i < queryData.Count; i++)
                    {




                        rowNum++;
                        int colNumD = 0;
                        IRow currentRowD = sheet.CreateRow(rowNum);
                        ICell cell1 = currentRowD.CreateCell(colNumD);

                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].CompanyCode);
                        colNumD++;

                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].CompanyName);
                        colNumD++;

                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].Name);
                        colNumD++;

                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].CertificateNumber);
                        colNumD++;

                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].Station);
                        colNumD++;

                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].City);
                        colNumD++;




                        //（养老）政策手续
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].PoliceOperationName_1);
                        colNumD++;
                        //（养老）社保政策
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].PoliceInsuranceName_1);
                        colNumD++;
                        //（养老）报减自然月
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].YearMonth_1.ToString());
                        colNumD++;
                        //（养老）社保月
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].InsuranceMonth_1.ToString());
                        colNumD++;
                        //（养老）是否单立户
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].IsIndependentAccount_1);
                        colNumD++;
                        //（养老）社保编号
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].InsuranceCode_1);
                        colNumD++;
                        //（养老）创建时间
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].CreateTime_1.ToString());
                        colNumD++;



                        //（医疗）政策手续
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].PoliceOperationName_2);
                        colNumD++;
                        //（医疗）社保政策
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].PoliceInsuranceName_2);
                        colNumD++;
                        //（医疗）报减自然月
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].YearMonth_2.ToString());
                        colNumD++;
                        //（医疗）社保月
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].InsuranceMonth_2.ToString());
                        colNumD++;
                        //（医疗）是否单立户
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].IsIndependentAccount_2);
                        colNumD++;
                        //（医疗）社保编号
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].InsuranceCode_2);
                        colNumD++;
                        //（医疗）创建时间
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].CreateTime_2.ToString());
                        colNumD++;




                        //（工伤）政策手续
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].PoliceOperationName_3);
                        colNumD++;
                        //（工伤）社保政策
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].PoliceInsuranceName_3);
                        colNumD++;
                        //（工伤）报减自然月
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].YearMonth_3.ToString());
                        colNumD++;
                        //（工伤）社保月
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].InsuranceMonth_3.ToString());
                        colNumD++;
                        //（工伤）是否单立户
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].IsIndependentAccount_3);
                        colNumD++;
                        //（工伤）社保编号
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].InsuranceCode_3);
                        colNumD++;
                        //（工伤）创建时间
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].CreateTime_3.ToString());
                        colNumD++;




                        //（失业）政策手续
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].PoliceOperationName_4);
                        colNumD++;
                        //（失业）社保政策
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].PoliceInsuranceName_4);
                        colNumD++;
                        //（失业）报减自然月
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].YearMonth_4.ToString());
                        colNumD++;
                        //（失业）社保月
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].InsuranceMonth_4.ToString());
                        colNumD++;
                        //（失业）是否单立户
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].IsIndependentAccount_4);
                        colNumD++;
                        //（失业）社保编号
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].InsuranceCode_4);
                        colNumD++;
                        //（失业）创建时间
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].CreateTime_4.ToString());
                        colNumD++;



                        //（公积金）政策手续
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].PoliceOperationName_5);
                        colNumD++;
                        //（公积金）社保政策
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].PoliceInsuranceName_5);
                        colNumD++;
                        //（公积金）报减自然月
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].YearMonth_5.ToString());
                        colNumD++;
                        //（公积金）社保月
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].InsuranceMonth_5.ToString());
                        colNumD++;
                        //（公积金）是否单立户
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].IsIndependentAccount_5);
                        colNumD++;
                        //（公积金）社保编号
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].InsuranceCode_5);
                        colNumD++;
                        //（公积金）创建时间
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].CreateTime_5.ToString());
                        colNumD++;



                        //（生育）政策手续
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].PoliceOperationName_6);
                        colNumD++;
                        //（生育）社保政策
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].PoliceInsuranceName_6);
                        colNumD++;
                        //（生育）报减自然月
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].YearMonth_6.ToString());
                        colNumD++;
                        //（生育）社保月
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].InsuranceMonth_6.ToString());
                        colNumD++;
                        //（生育）是否单立户
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].IsIndependentAccount_6);
                        colNumD++;
                        //（生育）社保编号
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].InsuranceCode_6);
                        colNumD++;
                        //（生育）创建时间
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].CreateTime_6.ToString());
                        colNumD++;



                        ////（医疗大额）政策手续
                        //cell = currentRowD.CreateCell(colNumD);
                        //cell.SetCellValue(queryData[i].PoliceOperationName_7);
                        //colNumD++;
                        ////（医疗大额）社保政策
                        //cell = currentRowD.CreateCell(colNumD);
                        //cell.SetCellValue(queryData[i].PoliceInsuranceName_7);
                        //colNumD++;
                        ////（医疗大额）报减自然月
                        //cell = currentRowD.CreateCell(colNumD);
                        //cell.SetCellValue(queryData[i].YearMonth_7.ToString());
                        //colNumD++;
                        ////（医疗大额）社保月
                        //cell = currentRowD.CreateCell(colNumD);
                        //cell.SetCellValue(queryData[i].InsuranceMonth_7.ToString());
                        //colNumD++;
                        ////（医疗大额）是否单立户
                        //cell = currentRowD.CreateCell(colNumD);
                        //cell.SetCellValue(queryData[i].IsIndependentAccount_7);
                        //colNumD++;
                        ////（医疗大额）社保编号
                        //cell = currentRowD.CreateCell(colNumD);
                        //cell.SetCellValue(queryData[i].InsuranceCode_7);
                        //colNumD++;
                        ////（医疗大额）创建时间
                        //cell = currentRowD.CreateCell(colNumD);
                        //cell.SetCellValue(queryData[i].CreateTime_7.ToString());
                        //colNumD++;



                        //（补充公积金）政策手续
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].PoliceOperationName_8);
                        colNumD++;
                        //（补充公积金）社保政策
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].PoliceInsuranceName_8);
                        colNumD++;
                        //（补充公积金）报减自然月
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].YearMonth_8.ToString());
                        colNumD++;
                        //（补充公积金）社保月
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].InsuranceMonth_8.ToString());
                        colNumD++;
                        //（补充公积金）是否单立户
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].IsIndependentAccount_8);
                        colNumD++;
                        //（补充公积金）社保编号
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].InsuranceCode_8);
                        colNumD++;
                        //（补充公积金）创建时间
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].CreateTime_8.ToString());
                        colNumD++;


                        //（大病）政策手续
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].PoliceOperationName_9);
                        colNumD++;
                        //（大病）社保政策
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].PoliceInsuranceName_9);
                        colNumD++;
                        //（大病）报减自然月
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].YearMonth_9.ToString());
                        colNumD++;
                        //（大病）社保月
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].InsuranceMonth_9.ToString());
                        colNumD++;
                        //（大病）是否单立户
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].IsIndependentAccount_9);
                        colNumD++;
                        //（大病）社保编号
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].InsuranceCode_9);
                        colNumD++;
                        //（大病）创建时间
                        cell = currentRowD.CreateCell(colNumD);
                        cell.SetCellValue(queryData[i].CreateTime_9.ToString());
                        colNumD++;

                        ids += queryData[i].StopIds+",";

                    }

                    #endregion


                    var results = 0;//返回的结果
                    if (queryDic["CollectState"].Equals(Common.CollectState.未提取.ToString()))
                    {
                        int[] ApprovedId;
                        string[] strArray = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        ApprovedId = Array.ConvertAll<string, int>(strArray, s => int.Parse(s));

                        string returnValue = string.Empty;
                        if (ApprovedId != null && ApprovedId.Length > 0)
                        {
                            if (m_BLL.EmployeeStopPaymentApproved(ref validationErrors, ApprovedId, Common.EmployeeAdd_State.员工客服已确认.ToString(), Common.EmployeeAdd_State.社保专员已提取.ToString(), null, LoginInfo.LoginName))
                            {
                                results = ApprovedId.Count();
                                LogClassModels.WriteServiceLog("社保专员已提取" + "，信息的Id为" + string.Join(",", ApprovedId), "消息"
                                    );//回退成功，写入日志
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
                                LogClassModels.WriteServiceLog("社保专员提取失败" + "，信息的Id为" + string.Join(",", ApprovedId) + "," + returnValue, "消息"
                                    );//回退失败，写入日志
                            }
                        }
                    }
                    string fileName = "社保专员导出_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xls";
                    //string name = Guid.NewGuid().ToString();
                    //string fileName = name + "供应商导出.xls";
                    string urlPath = "DataExport\\" + fileName; // 文件下载的URL地址，供给前台下载
                    string filePath = System.Web.HttpContext.Current.Server.MapPath("\\") + urlPath; // 文件路径
                    file = new FileStream(filePath, FileMode.Create);
                    workbook.Write(file);
                    file.Close();
                    string Message = "已成功提取报减信息";
                    //if (queryDic["CollectState"].Equals(Common.CollectState.未提取.ToString()))
                    //{
                    //    Message = string.Format("已成功提取{0}人,共{1}条险种的信息", queryData.Count, results);
                    //}
                    //else {
                    //    Message = string.Format("已成功导出{0}人的信息", queryData.Count);
                    //}
                    if (queryData.Count == 0)
                    {
                        //ActionResult result = 
                        var data = new Common.ClientResult.UrlResult
                        {
                            Code = ClientCode.FindNull,
                            Message = "没有符合条件的数据",
                            URL = urlPath
                        };
                        return data;
                    }
                    return new Common.ClientResult.UrlResult
                    {
                        Code = ClientCode.Succeed,
                        Message = Message,
                        URL = urlPath
                    };

                    //string vv = Newtonsoft.Json.JsonConvert.SerializeObject(aa);
                    //return Content(vv);

                }
            }
            catch (Exception e)
            {
                file.Close();
                return new Common.ClientResult.UrlResult
                {
                    Code = ClientCode.Fail,
                    Message = e.Message
                };
            }

        }

        #endregion


        #region 社保专员回退报减信息
        /// <summary>
        /// 社保专员回退报减信息
        /// </summary>
        /// <param name="ids">回退人员的id集合</param>
        /// <returns></returns>

        public Common.ClientResult.Result EmployeeFallbackAction(string ids, string message, string alltype)
        {

            alltype = HttpUtility.HtmlDecode(alltype);
            string[] strArrayall = alltype.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            List<int?> InsuranceKindTypes = new List<int?>();
            foreach (var a in strArrayall)
            {
                int InsuranceKindId = (int)(Common.EmployeeAdd_InsuranceKindId)Enum.Parse(typeof(Common.EmployeeAdd_InsuranceKindId), a);
                InsuranceKindTypes.Add(InsuranceKindId);
            }

            Common.ClientResult.Result result = new Common.ClientResult.Result();

            string returnValue = string.Empty;
            int[] ApprovedId = Array.ConvertAll<string, int>(ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), delegate(string s) { return int.Parse(s); });
            if (ApprovedId != null && ApprovedId.Length > 0)
            {                
                    using (var ent = new SysEntities())
                {

                    var empadd = ent.EmployeeStopPayment.Where(a => true);

                    var updateEmpAdd = (from a in empadd.Where(a => ApprovedId.Contains(a.Id) )
                                        select new
                                        {
                                            a.State
                                        }).Distinct();
                    if (updateEmpAdd == null || updateEmpAdd.Count() != 1)
                    {
                        LogClassModels.WriteServiceLog("回退失败" + "，信息的Id为" + string.Join(",", ApprovedId) + "," + returnValue, "消息"
                       );//回退失败，写入日志
                        result.Code = Common.ClientCode.Fail;
                        result.Message = "回退成功失败" + returnValue;
                    }
                    else {
                        string newStr = "";
                        string oldStr = "";
                        if (updateEmpAdd.FirstOrDefault().State == Common.EmployeeAdd_State.员工客服已确认.ToString())
                        {
                            newStr = Common.EmployeeAdd_State.待员工客服确认.ToString();
                            oldStr = Common.EmployeeAdd_State.员工客服已确认.ToString();
                        }
                        else {
                            newStr = Common.EmployeeAdd_State.员工客服已确认.ToString();
                            oldStr = Common.EmployeeAdd_State.社保专员已提取.ToString();
                        }
                        if (EmployeeStopPaymentApprovedn( ApprovedId, oldStr, newStr, message, LoginInfo.LoginName, InsuranceKindTypes))
                        {
                            LogClassModels.WriteServiceLog("回退成功" + "，信息的Id为" + string.Join(",", ApprovedId), "消息"
                                );//回退成功，写入日志
                            result.Code = Common.ClientCode.Succeed;
                            result.Message = "回退成功";
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
                            LogClassModels.WriteServiceLog("回退失败" + "，信息的Id为" + string.Join(",", ApprovedId) + "," + returnValue, "消息"
                                );//回退失败，写入日志
                            result.Code = Common.ClientCode.Fail;
                            result.Message = "回退成功失败" + returnValue;
                        }
                    }
                    }
                
            }
            return result;
        }
        #endregion


        #region 审责任客服审核
        /// <summary>
        /// 审责任客服审核
        /// </summary>
        /// <param name="ApprovedId">员工关系id</param>
        /// <param name="StateOld">原状态</param>
        /// <param name="StateNew">新审核状态</param>
        /// <returns></returns>
        public bool EmployeeStopPaymentApprovedn(int[] ApprovedId, string StateOld, string StateNew, string message, string UpdatePerson, List<int?> Shebaozhonglei)
        {
            using (var ent = new SysEntities())
            {
                try
                {
                    if (ApprovedId == null || ApprovedId.Count() <= 0)
                    {
                        return false;
                    }
                    //var updateEmpAdd = ent.EmployeeAdd.Where(a => ApprovedId.Contains(a.CompanyEmployeeRelationId) && a.State == StateOld);
                    var updateEmpStop = ent.EmployeeStopPayment.Where(a => ApprovedId.Contains(a.Id) && a.State == StateOld && Shebaozhonglei.Contains(a.EmployeeAdd.InsuranceKindId));
                    if (updateEmpStop != null && updateEmpStop.Count() >= 1)
                    {
                        foreach (var item in updateEmpStop)
                        {
                            item.State = StateNew;
                            if (!string.IsNullOrWhiteSpace(message))
                            {
                                item.Remark = message;
                            }
                            if (!string.IsNullOrWhiteSpace(UpdatePerson))
                            {
                                item.UpdatePerson = UpdatePerson;
                            }
                            item.UpdateTime = DateTime.Now;
                        }
                        ent.SaveChanges();
                    }

                    return true;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        #endregion

         #region 获取登录人某菜单功能所有可查看人员
        /// <summary>
        /// 获取登录人某菜单功能所有可查看人员
        /// </summary>
        /// <returns></returns>

        public List<ORG_User> getSubordinatesData(string code, string menuID)
        {
            #region 权限
            string departments = "";
            int departmentScope = base.MenuDepartmentScopeAuthority(menuID);
            if (departmentScope == (int)DepartmentScopeAuthority.无限制)//无限制
            {
                //部门业务权限
                departments = MenuDepartmentAuthority(menuID);
            }
            #endregion
            List<ORG_User> queryData = userBLL.GetGroupUsers(code, departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID);
            return queryData;
        }
        #endregion

    }
}


