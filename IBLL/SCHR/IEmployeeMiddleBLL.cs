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
    /// 费用中间表 接口
    /// </summary>
    public partial interface IEmployeeMiddleBLL
    {
        /// <summary>
        /// 修改费用中间表状态
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="id">费用中间表的主键</param>
        /// <param name="status">要修改成的状态</param>
        /// <param name="person">当前操作人</param>
        /// <returns></returns>  
        bool UpdateEmployeeMiddleState(ref ValidationErrors validationErrors, int id, string status, string person);

        /// <summary>
        /// 根据用户组权限获取公司列表（需进行权限判断）
        /// </summary>
        /// 责任客服可查询：自己负责的企业
        /// 社保客服可查询：所有的企业
        /// <param name="departmentScope">部门业务权限</param>
        /// <param name="departments">部门范围权限</param>
        /// <param name="branchID">登录人机构ID</param>
        /// <param name="departmentID">登录人部门ID</param>
        /// <param name="userID">登录人ID</param>
        List<CRM_Company> GetCompanyListByGroup(int departmentScope, string departments, int branchID, int departmentID, int userID);

        /// <summary>
        /// 获取费用中间表中数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        EmployeeMiddleShow GetDataByID(int id);

        /// <summary>
        /// 批量插入费用表
        /// </summary>
        /// <param name="employeeList"></param>
        /// <returns></returns>
        int InsertList(List<EmployeeMiddle> employeeList);

    }
}

