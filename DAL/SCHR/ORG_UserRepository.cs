using Common;
using Langben.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL
{
    public partial class ORG_UserRepository : BaseRepository<ORG_User>, IDisposable
    {

        /// <summary>
        /// 查询的数据
        /// </summary>
        public IQueryable<dynamic> GetUserList(SysEntities db, string realName)
        {
            var query = from u in db.ORG_User
                        join d in db.ORG_Department on u.ORG_Department_ID equals d.ID into tempDepart
                        from depart in tempDepart.DefaultIfEmpty()
                        //join p in db.ORG_Position on u.ORG_Position_ID equals p.ID into tempPost
                        //from post in tempPost.DefaultIfEmpty()
                        where u.XYBZ=="Y"
                        select new 
                        {
                            u.ID,
                            u.LoginName,
                            u.RName,
                            u.Code,
                            u.Sex,
                            u.ORG_Department_ID,
                            //u.ORG_Position_ID,
                            depart.DepartmentName
                            //post.PositionName
                        };
            if (!string.IsNullOrEmpty(realName))
            {
                query = query.Where(e => e.RName.Contains(realName));
            }
            return query.OrderByDescending(e => e.ID);
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
        public IQueryable<ORG_User> GetGroupUsers(SysEntities db, string code, int departmentScope, string departments, int branchID, int departmentID, int userID)
        {
            // 获取特定用户组所有人员
            var query = from a in db.ORG_User
                        join b in db.ORG_GroupUser on a.ID equals b.ORG_User_ID
                        join c in db.ORG_Group on b.ORG_Group_ID equals c.ID
                        where c.Code == code && a.XYBZ == "Y" && c.XYBZ == "Y"
                        select a;

            if (departmentScope == (int)DepartmentScopeAuthority.无限制)//无限制
            {
                if (!string.IsNullOrEmpty(departments))
                {
                    query = from a in query
                            join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                            where a.XYBZ == "Y" && b.XYBZ == "Y" && departments.Split(',').Contains(b.ID.ToString())
                            select a;
                }
            }
            else if (departmentScope == (int)DepartmentScopeAuthority.本机构及下属机构)//本机构及下属机构
            {
                //查询本机构及下属机构所有部门数据
                var branch = db.ORG_Department.FirstOrDefault(o => o.ID == branchID);
                query = from a in query
                        join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                        where a.XYBZ == "Y" && b.XYBZ == "Y" && b.LeftValue >= branch.LeftValue && b.RightValue <= branch.RightValue
                        select a;
            }
            else if (departmentScope == (int)DepartmentScopeAuthority.本机构) //本机构
            {
                query = from a in query
                        join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                        where a.XYBZ == "Y" && b.XYBZ == "Y" && b.BranchID == branchID
                        select a;
            }
            else if (departmentScope == (int)DepartmentScopeAuthority.本部门及其下属部门)//本部门及其下属部门
            {
                //当前用户所属部门
                ORG_Department department = db.ORG_Department.FirstOrDefault(o => o.ID == departmentID);

                //查询本部门及下属部门所有部门数据
                query = from a in query
                        join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                        where a.XYBZ == "Y" && b.XYBZ == "Y" && b.LeftValue >= department.LeftValue && b.RightValue <= department.RightValue
                        select a;
            }
            else if (departmentScope == (int)DepartmentScopeAuthority.本部门) //本部门
            {
                query = query.Where(c => c.XYBZ == "Y" && c.ORG_Department_ID == departmentID);

            }
            else if (departmentScope == (int)DepartmentScopeAuthority.本人) //本人
            {
                query = query.Where(c => c.XYBZ == "Y" && c.ID == userID);
            }

            return query;
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
        public IQueryable<ORG_User> GetUsers(SysEntities db,int departmentScope, string departments, int branchID, int departmentID, int userID)
        {
            // 获取特定用户组所有人员
            var query = from a in db.ORG_User
                        where a.XYBZ == "Y" 
                        select a;

            if (departmentScope == (int)DepartmentScopeAuthority.无限制)//无限制
            {
                if (!string.IsNullOrEmpty(departments))
                {
                    query = from a in query
                            join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                            where a.XYBZ == "Y" && b.XYBZ == "Y" && departments.Split(',').Contains(b.ID.ToString())
                            select a;
                }
            }
            else if (departmentScope == (int)DepartmentScopeAuthority.本机构及下属机构)//本机构及下属机构
            {
                //查询本机构及下属机构所有部门数据
                var branch = db.ORG_Department.FirstOrDefault(o => o.ID == branchID);
                query = from a in query
                        join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                        where a.XYBZ == "Y" && b.XYBZ == "Y" && b.LeftValue >= branch.LeftValue && b.RightValue <= branch.RightValue
                        select a;
            }
            else if (departmentScope == (int)DepartmentScopeAuthority.本机构) //本机构
            {
                query = from a in query
                        join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                        where a.XYBZ == "Y" && b.XYBZ == "Y" && b.BranchID == branchID
                        select a;
            }
            else if (departmentScope == (int)DepartmentScopeAuthority.本部门及其下属部门)//本部门及其下属部门
            {
                //当前用户所属部门
                ORG_Department department = db.ORG_Department.FirstOrDefault(o => o.ID == departmentID);

                //查询本部门及下属部门所有部门数据
                query = from a in query
                        join b in db.ORG_Department on a.ORG_Department_ID equals b.ID
                        where a.XYBZ == "Y" && b.XYBZ == "Y" && b.LeftValue >= department.LeftValue && b.RightValue <= department.RightValue
                        select a;
            }
            else if (departmentScope == (int)DepartmentScopeAuthority.本部门) //本部门
            {
                query = query.Where(c => c.XYBZ == "Y" && c.ORG_Department_ID == departmentID);

            }
            else if (departmentScope == (int)DepartmentScopeAuthority.本人) //本人
            {
                query = query.Where(c => c.XYBZ == "Y" && c.ID == userID);
            }

            return query;
        }

        /// <summary>
        /// 查询是否为领导
        /// </summary>
        /// <param name="db"></param>
        /// <param name="code">用户组编码</param>
        /// <param name="loinguser">登陆名</param>
        /// <returns></returns>
        public IQueryable<ORG_GroupUser> GetGroupAuthority(SysEntities db, string code,int loinguser)
        {
            // 获取特定用户组所有人员
            var query = from a in db.ORG_GroupUser
                        join b in db.ORG_Group on a.ORG_Group_ID equals b.ID
                        where b.Code == code && b.XYBZ == "Y" && a.ORG_User_ID == loinguser
                        select a;
            return query;
        }

        public void Dispose()
        {
        }
    }

}
