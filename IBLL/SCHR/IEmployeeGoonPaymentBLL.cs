using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using Langben.DAL;
using System.ServiceModel;
using Langben.DAL.Model;

namespace Langben.IBLL
{
    /// <summary>
    /// 员工补缴 接口
    /// </summary>
    public partial interface IEmployeeGoonPaymentBLL
    {
        /// <summary>
        /// 查询责任客服审核平台数据列表数据
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="search">查询条件</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        List<EmployeeApprove> GetApproveListByParam(int? id, int page, int rows, string search, ref int total);

        /// <summary>
        /// 补缴数据审核
        /// </summary>
        /// <param name="validationErrors"></param>
        /// <param name="approvedId">待审核补缴数据ID</param>
        /// <param name="stateOld">原状态</param>
        /// <param name="stateNew">审核状态</param>
        /// <returns></returns>
        bool EmployeeGoonPaymentApproved(ref ValidationErrors validationErrors, int?[] approvedId, string stateOld, string stateNew);

        List<EmployeeGoonPaymentView> GetEmployeeGoonPaymentExcelList(int page, int rows, string search, ref int count);
    }
}

