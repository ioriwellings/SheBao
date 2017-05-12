using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Langben.DAL;
using Common;
using System.Data;
using Langben.DAL.Model;

namespace Langben.BLL
{
    /// <summary>
    /// 增加员工 
    /// </summary>
    public partial class EmployeeAddBLL : IBLL.IEmployeeAddBLL, IDisposable
    {
        /// <summary>
        /// 查询的数据
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="search">查询条件</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        public List<EmployeeApprove> GetApproveListByParam(int? id, int page, int rows, string search, ref int total)
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
                    {//查询一对多关系的列名
                        CertificateNumber = item.Value;
                        continue;
                    }
                    if (item.Key == "Name")
                    {//查询一对多关系的列名
                        Name = item.Value;
                        continue;
                    }
                    if (item.Key == "CompanyName")
                    {//查询一对多关系的列名
                        CompanyName = item.Value; ;
                        continue;
                    }
                }
            }
            List<EmployeeApprove> queryData = repository.GetApproveList(db, CertificateNumber, page, rows, search, out total);
            return queryData;


            //return repository.GetApproveList(db, page, rows, search, out total);
        }

        /// <summary>
        /// 查询的数据
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="search">查询条件</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        public List<EmployeeApprove> GetApproveListByParam(int? id, string search)
        {
            return repository.GetApproveList(db, search);
        }

        /// <summary>
        /// 人员报增审核
        /// </summary>
        /// <param name="validationErrors"></param>
        /// <param name="ApprovedId">审核人员主键</param>
        /// <param name="state">审核状态</param>
        /// <returns></returns>
        public bool EmployeeAddApproved(ref ValidationErrors validationErrors, int?[] ApprovedId, string StateOld, string StateNew)
        {
            try
            {
                repository.EmployeeAddApproved(ApprovedId, StateOld, StateNew);
                return true;
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
            }
            return false;
        }

        #region 查询待客服经理分配列表
        /// <summary>
        /// 查询待客服经理分配列表
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <returns></returns>      
        public List<EmployeeAllot> GetAllotList(int page, int rows, string search, ref int total)
        {
            return repository.GetAllotList(db, page, rows, search, ref total);
        }
        #endregion
        #region 缴纳地户口性质
        /// <summary>
        /// 缴纳地户口性质
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<idname__> getPoliceAccountNatureid(string id)
        {
            var PoliceAccountNature = repository.getPoliceAccountNatureid(db, id);
            if (PoliceAccountNature != null)
            {
                return PoliceAccountNature.ToList();
            }
            else
            {
                return null;
            }
        }
        #endregion
        #region 社保种类
        public List<idname__> getInsuranceKindid(string id)
        {
            return repository.getInsuranceKindid(db, id).ToList();
        }
        #endregion
        #region 政策手续
        public List<idname__> getPoliceOperationid(int id)
        {
            return repository.getPoliceOperationid(db, id).ToList();
        }
        #endregion
        #region 报增保险。不更新数据库
        /// <summary>
        /// 报增保险。不更新数据库
        /// </summary>
        /// <param name="db">实体类</param>
        /// <param name="entity">初始值</param>
        /// <returns></returns>
        public bool CreateEmployee(SysEntities entities, EmployeeAdd entity)
        {

            repository.Create(entities, entity);
            return true;

        }
        #endregion


        #region 社保报增查询
        /// <summary>
        /// 社保报增查询
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <returns></returns>      
        public List<EmployeeAddView> GetEmployeeAddList(int page, int rows, string search, List<ORG_User> userList, ref int total)
        {
            return repository.GetEmployeeAddList(db, page, rows, search, userList, ref total);
        }
        #endregion

        #region 社保报增查询 导出excel
           /// <summary>
           /// 社保报增查询 导出excel
           /// </summary>
           /// <param name="search"></param>
           /// <param name="userList"></param>
           /// <param name="total"></param>
           /// <returns></returns>
        public List<EmployeeAddView> GetEmployeeAddListForExcel(string search, List<ORG_User> userList, ref int total)
        {
            return repository.GetEmployeeAddListForExcel(db, search, userList, ref total);
        }
        #endregion

        #region 社保单人报增身份证号多行查询
        /// <summary>
        ///  社保单人报增身份证号多行查询
        /// </summary>
        /// <param name="zrUserId">责任客服ID</param>
        /// <param name="cardIds">身份证号（可多条根据换行符分割）</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        public List<Employee> GetEmployeeList(int zrUserId, int page, int rows, string search, ref int total)
        {
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
                }
            }
            List<Employee> queryData = repository.GetEmployeeList(db, zrUserId, cardIds, page, rows, ref total);
            return queryData;
        }
        #endregion

        #region 企业初始绑定
        public List<idname__> getCompanyList()
        {
            return repository.getCompanyList(db).ToList();
        }
        #endregion
        #region 企业初始绑定
        public List<idname__> getCitylist()
        {
            return repository.getCitylist(db).ToList();
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
        public List<EmployeeApprove> GetCommissionerListByParam(int? id, int page, int rows, string search, ref int total)
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
                    {//查询一对多关系的列名
                        CertificateNumber = item.Value;
                        continue;
                    }

                }
            }
            List<EmployeeApprove> queryData = repository.GetCommissionerList(db, CertificateNumber, page, rows, search, out total);
            return queryData;


            //return repository.GetApproveList(db, page, rows, search, out total);
        }
        #endregion


        #region 社保专员提取报增信息
        /// <summary>
        /// 社保专员提取报增信息
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <returns></returns>      
        public List<EmployeeAddView> GetEmployeeAddExcelList(int page, int rows, string search, ref int total)
        {
            return repository.GetEmployeeAddExcelList(db, page, rows, search, ref total);
        }
        #endregion


        #region 校验社保关联信息是否存在


        public List<EmployeeAddCheckModel> EmployeeAddCkeck(string City, int PoliceAccountNatureId, int? PoliceOperationId, int? PoliceInsuranceId, string InsuranceKind)
        {
            return repository.EmployeeAddCkeck(City, PoliceAccountNatureId, PoliceOperationId, PoliceInsuranceId, InsuranceKind);
        }
        #endregion

        #region 社保专员提取报增信息
        /// <summary>
        /// 社保专员提取报增信息
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <returns></returns>      
        public List<EmployeeAddView> GetEmployeeAddExcelList1(int page, int rows, string search, ref int total)
        {
            return repository.GetEmployeeAddExcelList1(db, page, rows, search, ref total);
        }
        #endregion


        #region 社保失败
        public string ChangeServicecharge(EmployeeAdd item, string message, string LoginName)
        {
            return repository.ChangeServicecharge(db, item, message, LoginName);
        }
        #endregion

        #region 模板设置报增失败
        /// <summary>
        /// 模板设置报增失败
        /// </summary>
        /// <param name="dt">Excel数据</param>
        /// <param name="userID">操作人ID</param>
        /// <param name="userName">操作人姓名</param>
        /// <returns></returns>
        public string SetAddFailByExcel(DataTable dt, int userID, string userName)
        {
            try
            {
                string result = "";
                List<EmployeeAdd> EmployeeAddList = new List<EmployeeAdd>();
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
                        EmployeeAdd addModel = new EmployeeAdd();
                        int rst = repository.CheckApprove(cardid, username, Common.EnumsCommon.GetInsuranceKindValue(InKind[j]), cityname, userID, ref addModel);

                        if (rst == 1)
                        {
                            result += "第" + (i + 1) + "行，该员工不存在！<br/>";
                        }
                        else if (rst == 2)
                        {
                            result += "第" + (i + 1) + "行，【" + InKind[j] + "】没有可操作报增记录！<br/>";
                        }
                        else if (rst == 3)
                        {
                            result += "第" + (i + 1) + "行，没有操作【" + cityname + InKind[j] + "】的权限！<br/>";
                        }
                        else if (rst == 0)
                        {
                            EmployeeAddList.Add(addModel);
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
                    using (TransactionScope scope = new TransactionScope())
                    {
                        using (db)
                        {
                            for (int i = 0; i < EmployeeAddList.Count; i++)
                            {
                                EmployeeAdd item = EmployeeAddList[i];
                                string message = failMsgList[i];
                                result = repository.ChangeServicecharge(db, item, message, userName);
                                db.SaveChanges();

                            }
                            scope.Complete();
                        }
                    }
                }

                #endregion

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        #region 供应商客服提取报增信息
        /// <summary>
        /// 供应商客服提取报增信息
        /// </summary>
        /// <returns></returns>      
        public List<SupplierAddView> GetEmployeeAddExcelListBySupplier(string search)
        {
            return repository.GetEmployeeAddExcelListBySupplier(db, search);
        }
        #endregion

        #region 修改报增数据状态
        /// <summary>
        /// 修改报增数据状态
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ids">要修改数据的ID数组</param>
        /// <param name="oldStatus">原状态(可为空字符)</param>
        /// <param name="newStatus">新状态</param>
        /// <param name="name">修改人</param>
        /// <returns></returns>
        public bool ChangeStatus(int[] ids, string oldStatus, string newStatus, string name)
        {
            return repository.ChangeStatus(db, ids, oldStatus, newStatus, name);
        }
        #endregion

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

