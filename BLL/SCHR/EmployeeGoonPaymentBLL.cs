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
    //public partial class EmployeeGoonPaymentBLL : IBLL.IEmployeeGoonPaymentBLL, IDisposable
    public partial class EmployeeGoonPaymentBLL : IBLL.IEmployeeGoonPaymentBLL, IDisposable
    {

        #region 报增保险。不更新数据库
        /// <summary>
        /// 报增保险。不更新数据库
        /// </summary>
        /// <param name="db">实体类</param>
        /// <param name="entity">初始值</param>
        /// <returns></returns>
        public bool CreateEmployeeGoonPayment(SysEntities entities, EmployeeGoonPayment entity)
        {
            repository.Create(entities, entity);
            return true;
        }
        #endregion

        #region 责任客服审核平台数据列表获取

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


        #endregion

        #region 责任客服审核补缴数据

        /// <summary>
        /// 补缴数据审核
        /// </summary>
        /// <param name="validationErrors"></param>
        /// <param name="approvedId">待审核补缴数据ID</param>
        /// <param name="stateOld">原状态</param>
        /// <param name="stateNew">审核状态</param>
        /// <returns></returns>
        public bool EmployeeGoonPaymentApproved(ref ValidationErrors validationErrors, int?[] approvedId, string stateOld, string stateNew)
        {
            try
            {
                if (approvedId != null)
                {
                    using (TransactionScope transactionScope = new TransactionScope())
                    {
                        repository.EmployeeGoonPaymentApproved(db, approvedId, stateOld, stateNew);
                        if (approvedId.Length == repository.Save(db))
                        {
                            transactionScope.Complete();
                            return true;
                        }
                        else
                        {
                            Transaction.Current.Rollback();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
            }
            return false;
        }
        #endregion

        #region 社保专员提取补缴信息
        /// <summary>
        /// 社保专员提取补缴信息
        /// </summary>
        /// <param name="SysEntities">数据访问的上下文</param>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <returns></returns>      
        public List<EmployeeGoonPaymentView> GetEmployeeGoonPaymentExcelList(int page, int rows, string search, ref int total)
        {
            return repository.GetEmployeeGoonPaymentExcelList(db, page, rows, search, ref total);
        }
        #endregion
    }
}
