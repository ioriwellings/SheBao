using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Langben.DAL;
using Common;
using Langben.DAL.Model;
using System.Data;

namespace Langben.BLL
{
    /// <summary>
    /// 员工停缴 
    /// </summary>
    public partial class EmployeeStopPaymentBLL : IBLL.IEmployeeStopPaymentBLL, IDisposable
    {

        /// <summary>
        /// 获取可单人报减员工信息
        /// </summary>
        /// <param name="zrUserId">责任客服ID</param>
        /// <param name="companyId">企业ID</param>
        /// <param name="employeeName">员工姓名</param>
        /// <param name="cardIds">身份证号（可多条根据换行符分割）</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        public List<SingleStopPaymentView> GetSingleStopPaymentInfo(int zrUserId, int page, int rows, string search, ref int total)
        {
            int companyId = 0;
            string employeeName = string.Empty;
            string cardIds = string.Empty;

            Dictionary<string, string> queryDic = ValueConvert.StringToDictionary(search.GetString());
            if (queryDic != null && queryDic.Count > 0)
            {
                foreach (var item in queryDic)
                {
                    if (item.Key == "CardIDs")
                    {//查询一对多关系的列名
                        cardIds = item.Value;
                        continue;
                    }
                    if (item.Key == "EmployeeName")
                    {//查询一对多关系的列名
                        employeeName = item.Value;
                        continue;
                    }
                    if (item.Key == "CompanyID")
                    {//查询一对多关系的列名
                        companyId = Convert.ToInt32(item.Value);
                        continue;
                    }
                }
            }

            List<SingleStopPaymentView> queryData = repository.GetSingleStopPaymentInfo(db, zrUserId, companyId, employeeName, cardIds, page, rows, ref total);

            return queryData;
        }


        /// <summary>
        /// 获取责任客服报减信息
        /// </summary>
        /// <param name="zrUserId">责任客服ID</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="search">查询条件</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        public List<SingleStopPaymentView> GetListFromP(int zrUserId, int page, int rows, string search, ref int total)
        {
            int companyId = 0;
            string employeeName = string.Empty;
            string cardIds = string.Empty;

            Dictionary<string, string> queryDic = ValueConvert.StringToDictionary(search.GetString());
            if (queryDic != null && queryDic.Count > 0)
            {
                foreach (var item in queryDic)
                {
                    if (item.Key == "CardIDs")
                    {//查询一对多关系的列名
                        cardIds = item.Value;
                        continue;
                    }
                    if (item.Key == "EmployeeName")
                    {//查询一对多关系的列名
                        employeeName = item.Value;
                        continue;
                    }
                    if (item.Key == "CompanyID")
                    {//查询一对多关系的列名
                        companyId = Convert.ToInt32(item.Value);
                        continue;
                    }
                }
            }
            List<SingleStopPaymentView> queryData = repository.GetListFromP(db, zrUserId, companyId, employeeName, cardIds, page, rows, ref total);

            return queryData;
        }


        /// <summary>
        /// 获取员工客服报减信息
        /// </summary>
        /// <param name="zrUserId">责任客服ID</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="search">查询条件</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        public List<SingleStopPaymentView> GetEmployeeStopForCustomerList(int zrUserId, int page, int rows, string search, ref int total, int yguid)
        {
            int companyId = 0;
            string employeeName = string.Empty;
            string cardIds = string.Empty;

            Dictionary<string, string> queryDic = ValueConvert.StringToDictionary(search.GetString());
            if (queryDic != null && queryDic.Count > 0)
            {
                foreach (var item in queryDic)
                {
                    if (item.Key == "CardIDs")
                    {//查询一对多关系的列名
                        cardIds = item.Value;
                        continue;
                    }
                    if (item.Key == "EmployeeName")
                    {//查询一对多关系的列名
                        employeeName = item.Value;
                        continue;
                    }
                    if (item.Key == "CompanyID")
                    {//查询一对多关系的列名
                        companyId = Convert.ToInt32(item.Value);
                        continue;
                    }
                }
            }
            List<SingleStopPaymentView> queryData = repository.GetEmployeeStopForCustomerList(db, zrUserId, companyId, employeeName, cardIds, page, rows, ref total, yguid);

            return queryData;
        }

        /// <summary>
        /// 获取员工客服报减信息
        /// </summary>
        /// <param name="zrUserId">责任客服ID</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="search">查询条件</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        public List<SingleStopPaymentViewDuty> GetEmployeeStopForDutyList(int zrUserId, int page, int rows, string search, ref int total)
        {
            int companyId = 0;
            string employeeName = string.Empty;
            string cardIds = string.Empty;
            string YearMonth = string.Empty;

            Dictionary<string, string> queryDic = ValueConvert.StringToDictionary(search.GetString());
            if (queryDic != null && queryDic.Count > 0)
            {
                foreach (var item in queryDic)
                {
                    if (item.Key == "CardIDs")
                    {//查询一对多关系的列名
                        cardIds = item.Value;
                        continue;
                    }
                    if (item.Key == "EmployeeName")
                    {//查询一对多关系的列名
                        employeeName = item.Value;
                        continue;
                    }
                    if (item.Key == "CompanyID")
                    {//查询一对多关系的列名
                        companyId = Convert.ToInt32(item.Value);
                        continue;
                    }
                    if (item.Key == "YearMonth")
                    {//查询一对多关系的列名
                        YearMonth = item.Value.Replace("-", "");
                        continue;
                    }
                }
            }
            List<SingleStopPaymentViewDuty> queryData = repository.GetEmployeeStopForDutyList(db, zrUserId, companyId, employeeName, YearMonth, cardIds, page, rows, ref total);

            return queryData;
        }



        /// <summary>
        /// 获取停缴时员工的信息
        /// </summary>
        /// <param name="companyEmployeeRelationId"></param>
        /// <returns></returns>
        public StopPaymentEmployeeInfo GetStopPaymentEmployeeInfo(int companyEmployeeRelationId)
        {
            return repository.GetStopPaymentEmployeeInfo(db, companyEmployeeRelationId);
        }



        /// <summary>
        /// 报减详情
        /// </summary>
        /// <param name="companyEmployeeRelationId"></param>
        /// <returns></returns>
        public StopPaymentEmployeeInfo GetStopPaymentEmployeeInfoForStop(int companyEmployeeRelationId, string State)
        {
            return repository.EmploeeStopInfo(db, companyEmployeeRelationId, State);
        }



        public bool InsertStopPayment(List<EmployeeStopPaymentSingle> list)
        {
            return repository.InsertStopPayment(db, list);
        }

        /// <summary>
        /// 责任客服审核平台数据
        /// </summary>
        /// <param name="kindsIDS"></param>
        /// <param name="IsPass"></param>
        /// <returns></returns>
        public bool PassStopPaymentRepository(string kindsIDS, int IsPass)
        {
            return repository.PassStopPaymentRepository(db, kindsIDS, IsPass);
        }
        /// <summary>
        /// 员工客服审核平台数据
        /// </summary>
        /// <param name="kindsIDS"></param>
        /// <param name="IsPass"></param>
        /// <returns></returns>
        public bool PassStopPaymentRepositoryForCustomer(string kindsIDS, int IsPass)
        {
            return repository.PassStopPaymentRepositoryForCustomer(db, kindsIDS, IsPass,"");
        }
        public bool PassStopPaymentRepositoryForCustomer(string kindsIDS, int IsPass, string remark)
        {
            return repository.PassStopPaymentRepositoryForCustomer(db, kindsIDS, IsPass, remark);
        }

        /// <summary>
        /// 通过企业ID获取企业名称
        /// </summary>
        /// <param name="companyId">企业ID</param>
        /// <returns>返回企业名称，如果没有查到返回""</returns>
        public string GetCompanyNameById(int companyId)
        {
            string name = string.Empty;
            var model = new CRM_CompanyRepository().GetById(db, companyId);
            if (model != null)
            {
                name = model.CompanyName;
            }
            return name;
        }
        /// <summary>
        /// 通过企业名称获取企业ID
        /// </summary>
        /// <param name="companyName">企业名称（全字符匹配）</param>
        /// <returns>返回企业ID，如果没有查到返回0</returns>
        public int GetCompanyIdByName(string companyName)
        {
            return new CRM_CompanyRepository().GetCompanyIdByName(db, companyName);
        }

        /// <summary>
        /// 根据责任客服Id和企业名称获取企业ID.如果该企业存在并且是该责任客服负责反悔企业ID，如果企业存在但不是该责任客服负责反悔-1，如果企业名称不存在返回0
        /// </summary>
        /// <param name="companyName">企业名称（全字符匹配）</param>
        /// <param name="userZRId">责任客服Id</param>
        /// <returns>如果该企业存在并且是该责任客服负责反悔企业ID，如果企业存在但不是该责任客服负责反悔-1，如果企业名称不存在返回0</returns>
        public int GetCompanyIdByNameAndUserZRId(string companyName, int userZRId)
        {
            using (var companyDal = new CRM_CompanyRepository())
            {
                int companyId = companyDal.GetCompanyIdByName(db, companyName);
                if (companyId != 0)
                {
                    bool b = companyDal.IsZRUserHaveCompany(db, companyId, userZRId);
                    if (!b)
                    {
                        companyId = -1;
                    }
                }
                return companyId;
            }

        }
        /// <summary>
        /// 通过员工姓名和身份证号获取员工ID
        /// </summary>
        /// <param name="employeeName">员工姓名</param>
        /// <param name="cardId">身份证号</param>
        /// <returns>返回员工ID，如果没有查到返回0</returns>
        public int GetEmployeeIdByNameAndCardId(string employeeName, string cardId)
        {
            return repository.GetEmployeeIdByNameAndCardId(db, employeeName, cardId);
        }
        /// <summary>
        /// 通过企业ID和员工ID获取企业员工关系表ID
        /// </summary>
        /// <param name="companyId">企业ID</param>
        /// <param name="employeeId">员工ID</param>
        /// <returns>返回企业员工关系表ID，如果没有查到返回0</returns>
        public int GetCompanyEmployeeRelationId(int companyId, int employeeId)
        {
            return repository.GetCompanyEmployeeRelationId(db, companyId, employeeId);
        }
        /// <summary>
        /// 通过险种和企业员工关系表ID获取增员表ID
        /// </summary>
        /// <param name="kind">险种</param>
        /// <param name="companyEmployeeRelationId">企业员工关系表ID</param>
        /// <returns>返回状态是报增成功的报增记录ID，如果没查到返回0</returns>
        public int GetEmployeeAddIdByKindIdAndRelationId(EmployeeAdd_InsuranceKindId kind, int companyEmployeeRelationId)
        {
            return repository.GetEmployeeAddIdByKindIdAndRelationId(db, kind, companyEmployeeRelationId);
        }
        /// <summary>
        /// 通过增员表ID和手续名称获取手续ID
        /// </summary>
        /// <param name="employeeAddId">增员表ID</param>
        /// <param name="operationName">手续名称</param>
        /// <returns>返回手续ID，如果没查到返回0</returns>
        public int GetPoliceOperationIdByEmployeeAddIdAndOperationName(int employeeAddId, string operationName)
        {
            return repository.GetPoliceOperationIdByEmployeeAddIdAndOperationName(db, employeeAddId, operationName);
        }

        /// <summary>
        /// 通过增员表ID获取政策
        /// </summary>
        /// <param name="addId">增员表ID</param>
        /// <returns></returns>
        public PoliceInsurance GetPoliceInsuranceByEmployeeAddId(int addId)
        {
            EmployeeAddRepository addRepository = new EmployeeAddRepository();
            var employeeAdd = addRepository.GetById(db, addId);
            return employeeAdd.PoliceInsurance;
        }

        /// <summary>
        /// 通过增员表ID 判断是否有正在报减的数据
        /// </summary>
        /// <param name="addId">增员表ID</param>
        /// <returns>有正在报减的记录返回true，否则返回false</returns>
        public bool IsHaveStopingByEmployeeAddId(int addId)
        {
            List<string> stopingState = new List<string>()
            {
                EmployeeStopPayment_State.待责任客服确认.ToString(),
                EmployeeStopPayment_State.待员工客服经理分配.ToString(),
                EmployeeStopPayment_State.待员工客服确认.ToString(),
                EmployeeStopPayment_State.员工客服已确认.ToString(),
                EmployeeStopPayment_State.社保专员已提取.ToString(),
            };
            return new EmployeeAddRepository().GetById(db, addId).EmployeeStopPayment.Any(o => stopingState.Contains(o.State));
        }


        #region 社保报减查询
        /// <summary>
        /// 社保报减查询
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <returns></returns>      
        public List<EmployeeStopView> GetEmployeeStopList(int page, int rows, string search, List<ORG_User> userList, ref int total)
        {
            return repository.GetEmployeeStopList(db, page, rows, search, userList, ref total);
        }
        #endregion

        #region 供应商客服提取报减数据
        /// <summary>
        /// 供应商客服提取报减数据
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="search">查询条件</param>
        /// <returns></returns>      
        public List<SupplierAddView> GetEmployeeStopExcelListBySupplier(string search)
        {
            return repository.GetEmployeeStopExcelListBySupplier(db, search);
        }
        #endregion

        #region 社保报减查询-导出excel
        /// <summary>
        /// 社保报减查询-无分页（导出excel时用）
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <param name="userList">员工列表</param>
        /// <returns></returns>
        public List<EmployeeStopView> GetEmployeeStopListForExcel(string search, List<ORG_User> userList)
        {
            return repository.GetEmployeeStopListForExcel(db, search, userList);
        }
        #endregion

        #region 查询社保专员列表
        /// <summary>
        /// 查询社保专员列表
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="search">查询条件</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        public List<EmployeeApprove> GetApproveList(int? id, int page, int rows, string search, ref int total)
        {
            string CompanyName = string.Empty;
            string Name = string.Empty;
            string CertificateNumber = string.Empty;
            Dictionary<string, string> queryDic = ValueConvert.StringToDictionary(search.GetString());
            if (queryDic != null && queryDic.Count > 0)
            {
                foreach (var item in queryDic)
                {
                    if (item.Key == "CertificateNumber")
                    {//身份证号
                        CertificateNumber = item.Value;
                        continue;
                    }
                    if (item.Key == "Name")
                    {//员工姓名
                        Name = item.Value;
                        continue;
                    }
                    if (item.Key == "CompanyName")
                    {//公司名称
                        CompanyName = item.Value; ;
                        continue;
                    }
                }
            }
            List<EmployeeApprove> queryData = repository.GetApproveList(db, CertificateNumber, page, rows, search, out total);
            return queryData;


            //return repository.GetApproveList(db, page, rows, search, out total);
        }
        #endregion

        #region 社保报减状态修改


        /// <summary>
        ///社保报减状态修改
        /// </summary>
        /// <param name="validationErrors"></param>
        /// <param name="ApprovedId">审核人员主键</param>
        /// <param name="state">审核状态</param>
        /// <returns></returns>
        public bool EmployeeStopPaymentApproved(ref ValidationErrors validationErrors, int[] ApprovedId, string StateOld, string StateNew, string message, string UpdatePerson)
        {
            try
            {
                repository.EmployeeStopPaymentApproved(ApprovedId, StateOld, StateNew, message, UpdatePerson);
                return true;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
            }
            return false;
        }
        #endregion
        #region 政策手续 王帅
        public List<idname__> getPoliceOperationid(int id)
        {
            return repository.getPoliceOperationid(db, id).ToList();
        }
        #endregion



        /// <summary>
        /// 修改停缴手续
        /// </summary>
        /// <param name="lstStopPayments"></param>
        /// <returns></returns>
        public bool EditStopPaymentOperation(List<EmployeeStopPayment> lstStopPayments)
        {
            return repository.EditStopPaymentOperation(db, lstStopPayments);
        }

        public bool SetStopPaymentSuccess(string stopIds)
        {
            List<int> lstStopId = new List<int>();
            string[] arrID = stopIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string id in arrID)
            {
                lstStopId.Add(Convert.ToInt32(id));
            }
            return repository.SetStopPaymentSuccess(db, lstStopId);
        }

        public bool SetStopPaymentFail(string stopIds, string remark)
        {
            List<int> lstStopId = new List<int>();
            string[] arrID = stopIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string id in arrID)
            {
                lstStopId.Add(Convert.ToInt32(id));
            }
            return repository.SetStopPaymentFail(db, lstStopId, remark);
        }

        /// <summary>
        /// 供应商根据报减编号及险种设置报减失败
        /// </summary>
        /// <param name="stopIds">报减编号</param>
        /// <param name="remark">失败原因</param>
        /// <param name="insuranceKindTypes">报减类型</param>
        /// <returns></returns>
        public bool SetStopPaymentFailByInsuranceKind(string stopIds, string remark, List<int?> insuranceKindTypes)
        {
            List<int> lstStopId = new List<int>();
            string[] arrID = stopIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string id in arrID)
            {
                lstStopId.Add(Convert.ToInt32(id));
            }
            var stopPayment = (from stop in db.EmployeeStopPayment.Where(o => lstStopId.Contains(o.Id) && insuranceKindTypes.Contains(o.EmployeeAdd.InsuranceKindId))
                                select stop.Id).ToList();

            return repository.SetStopPaymentFail(db, stopPayment, remark);
        }

        /// <summary>
        /// 模板设置报减失败
        /// </summary>
        /// <param name="dt">Excel数据</param>
        /// <param name="userID">操作人ID</param>
        /// <param name="userName">操作人姓名</param>
        /// <returns></returns>
        public string SetStopPaymentFailByExcel(DataTable dt, int userID, string userName)
        {
            try
            {
                string result = "";
                List<int> stopIDList = new List<int>();
                List<string> failMsgList = new List<string>();
                #region  验证
                //校验数据是否可操作

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    #region 数据验证
                    string tmpmsg = chkExcel(dt.Rows[i]);

                    if (tmpmsg != "")
                    {
                        result += "第" + (i + 1) + "行，" + tmpmsg + "<br/>";
                        continue;
                    }

                    #endregion

                    #region 业务验证

                    string[] InKind = dt.Rows[i]["险种"].ToString().Split(';');
                    string cardid = dt.Rows[i]["证件号码"].ToString();
                    string username = dt.Rows[i]["姓名"].ToString();
                    string cityname = dt.Rows[i]["社保缴纳地"].ToString();
                    string failMsg = dt.Rows[i]["失败原因"].ToString();

                    for (int j = 0; j < InKind.Length; j++)
                    {
                        int stopId = 0;
                        int rst = repository.CheckApprove(cardid, username, Common.EnumsCommon.GetInsuranceKindValue(InKind[j]), cityname, userID, ref stopId);

                        if (rst == 1)
                        {
                            result += "第" + (i + 1) + "行，该员工不存在！<br/>";
                        }
                        else if (rst == 2)
                        {
                            result += "第" + (i + 1) + "行，【" + InKind[j] + "】没有可操作报减记录！<br/>";
                        }
                        else if (rst == 3)
                        {
                            result += "第" + (i + 1) + "行，没有操作【" + cityname + InKind[j] + "】的权限！<br/>";
                        }
                        else if (rst == 0)
                        {
                            stopIDList.Add(stopId);
                            failMsgList.Add(failMsg);
                        }
                        else
                        {
                            result += "第" + (i + 1) + "行，未知错误<br/>";
                        }
                    }
                    #endregion
                }
                #endregion

                #region 写数据
                if (result == "")
                {
                    repository.SetStopFail(db, stopIDList, failMsgList, userName);
                }
                #endregion

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        #region private

        #region 失败原因Excel内容验证
        /// <summary>
        /// 失败原因Excel内容验证
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private string chkExcel(DataRow dr)
        {
            string result = "";

            if (dr["证件号码"] == DBNull.Value || dr["证件号码"].ToString() == "")
            {
                result += "证件号码不能为空！";
            }
            else
            {
                if (!Common.CardCommon.CheckCardID18(dr["证件号码"].ToString()))
                {
                    result += "证件号码不正确！";
                }

            }

            if (dr["姓名"] == DBNull.Value || dr["姓名"].ToString() == "")
            {
                result += "姓名不能为空！";
            }

            if (dr["社保缴纳地"] == DBNull.Value || dr["社保缴纳地"].ToString() == "")
            {
                result += "社保缴纳地不能为空！";
            }

            if (dr["险种"] == DBNull.Value || dr["险种"].ToString() == "")
            {
                result += "险种不能为空！";
            }

            return result;
        }
        #endregion

        #endregion
    }
}

