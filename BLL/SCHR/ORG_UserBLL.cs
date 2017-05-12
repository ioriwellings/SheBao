using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Langben.DAL;
using Common;

namespace Langben.BLL
{
    /// <summary>
    /// 费用_社保支出失业 
    /// </summary>
    public partial class ORG_UserBLL : IBLL.IORG_UserBLL, IDisposable
    {

        /// <summary>
        /// 私有的数据访问上下文
        /// </summary>
        protected SysEntities db;
        /// <summary>
        /// 费用_社保支出的数据库访问对象
        /// </summary>
        ORG_UserRepository repository = new ORG_UserRepository();
        /// <summary>
        /// 构造函数，默认加载数据访问上下文
        /// </summary>
        public ORG_UserBLL()
        {
            db = new SysEntities();
        }

            /// <summary>
        /// 查询的数据
            /// </summary>
            /// <param name="id"></param>
            /// <param name="page">页数</param>
            /// <param name="rows">条数</param>
            /// <param name="realName">员工姓名</param>
            /// <param name="total"></param>
            /// <returns></returns>
        public List<dynamic> GetUserList(int? id, int page, int rows, string realName, ref int total)
        {
            IQueryable<dynamic> queryData = repository.GetUserList(db, realName);
            total = queryData.Count();
            if (total > 0)
            {
                if (page <= 1)
                {
                    queryData = queryData.Take(rows);
                }
                else
                {
                    queryData = queryData.Skip((page - 1) * rows).Take(rows);
                }

            }
            return queryData.ToList();
        }

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
        public List<ORG_User> GetGroupUsers(string code, int departmentScope, string departments, int branchID, int departmentID, int userID)
        {
            List<ORG_User> list = new List<ORG_User>();

            IQueryable<ORG_User> query = repository.GetGroupUsers(db, code, departmentScope, departments, branchID, departmentID, userID);
            list = query.OrderBy(c => c.RName).ToList();
            return list;
        }

        /// <summary>
        /// 获取人员
        /// </summary>
        /// <param name="departments">部门范围权限</param>
        /// <param name="departmentScope">部门业务权限</param>
        /// <param name="branchID">登录人机构ID</param>
        /// <param name="departmentID">登录人部门ID</param>
        /// <param name="userID">登录人ID</param>
        /// <returns></returns>
        public List<ORG_User> GetUsers(int departmentScope, string departments, int branchID, int departmentID, int userID)
        {
            List<ORG_User> list = new List<ORG_User>();

            IQueryable<ORG_User> query = repository.GetUsers(db,departmentScope, departments, branchID, departmentID, userID);
            list = query.OrderBy(c => c.RName).ToList();
            return list;
        }

        /// <summary>
        /// 根据用户名查询用户组中的类型
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<ORG_GroupUser> GetGroupAuthority(string code, int userID)
        {
            List<ORG_GroupUser> list = new List<ORG_GroupUser>();

            IQueryable<ORG_GroupUser> query = repository.GetGroupAuthority(db, code, userID);
            list = query.OrderBy(c => c.ORG_Group_ID).ToList();
            return list;
        }

        public void Dispose()
        {

        }
    }
}

