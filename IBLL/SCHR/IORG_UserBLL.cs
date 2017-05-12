using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using Langben.DAL;
using System.ServiceModel;

namespace Langben.IBLL
{
    /// <summary>
    /// 费用_社保支出工伤 接口
    /// </summary>
    [ServiceContract(Namespace = "www.langben.com")]
    public partial interface IORG_UserBLL
    {
        /// <summary>
        /// 查询的数据
        /// </summary>
        List<dynamic> GetUserList(int? id, int page, int rows, string realName, ref int total);
        /// <summary>
        /// 获取用户组人员
        /// </summary>
        /// <param name="code">用户组编码</param>
        /// <param name="departments">部门范围权限</param>
        /// <param name="departmentScope">部门业务权限</param>
        /// <param name="branchID">登录人机构ID</param>
        /// <param name="departmentID">登录人部门ID</param>
        /// <param name="userID">登录人ID</param>
        /// <returns></returns>
        List<ORG_User> GetGroupUsers(string code, int departmentScope, string departments, int branchID, int departmentID, int userID);

         /// <summary>
        /// 获取人员
        /// </summary>
        /// <param name="departments">部门范围权限</param>
        /// <param name="departmentScope">部门业务权限</param>
        /// <param name="branchID">登录人机构ID</param>
        /// <param name="departmentID">登录人部门ID</param>
        /// <param name="userID">登录人ID</param>
        /// <returns></returns>
        List<ORG_User> GetUsers(int departmentScope, string departments, int branchID, int departmentID, int userID);
    
        /// <summary>
        /// 根据用户名查询用户组中的类型
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        List<ORG_GroupUser> GetGroupAuthority(string code,int userID);
    }
}

