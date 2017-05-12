using Langben.DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Langben.App.Areas.Authority.Controllers
{
    public class PersonController : BaseController
    {
        //
        // GET: /Authority/Common/
        public ActionResult Add()
        {
            //ViewBag.MenuID = Request["menuid"].ToString(); ;
            int Id = Convert.ToInt32(Request["Id"]);//角色或者用户组ID
            int Type = Convert.ToInt32(Request["Type"]); ;//1为用户组，2为角色
            dynamic query = null;
            if (Id != -1 && Type != -1)
            {
                using (SysEntities db = new SysEntities())
                {

                    StringBuilder sb = new StringBuilder();
                    if (Type == 1)///从用户组中取用户数据
                    {
                        query = (from a in db.ORG_GroupUser
                                 join b in db.ORG_User on a.ORG_User_ID equals b.ID
                                 join c in db.ORG_Department on b.ORG_Department_ID equals c.ID
                                 where a.ORG_Group_ID == Id
                                 select new
                                 {
                                     UserName = b.RName,
                                     UserID = a.ORG_User_ID,
                                     Group_ID = a.ORG_Group_ID,
                                     DepartID = b.ORG_Department_ID,
                                     DepartmentName = c.DepartmentName

                                 }).ToList();
                    }
                    if (Type == 2)///从角色中取用户数据
                    {
                        query = (from a in db.ORG_UserRole
                                 join b in db.ORG_User on a.ORG_User_ID equals b.ID
                                 join c in db.ORG_Department on b.ORG_Department_ID equals c.ID
                                 where a.ORG_Role_ID == Id
                                 select new
                                 {
                                     UserName = b.RName,
                                     UserID = a.ORG_User_ID,
                                     Group_ID = a.ORG_Role_ID,
                                     DepartID = b.ORG_Department_ID,
                                     DepartmentName = c.DepartmentName

                                 }).ToList();
                    }
                    string strDepartName = "";
                    string strDepartID = "";
                    string strUserID = "";
                    string strUserName = "";
                    for (int i = 0; i < query.Count; i++)
                    {
                        if (i + 1 < query.Count)
                        {
                            if (query[i].DepartmentName == query[i + 1].DepartmentName)
                            {
                                strDepartName = query[i].DepartmentName;
                                strDepartID = query[i].DepartID.ToString();
                                strUserID += query[i].UserID.ToString() + ",";
                                strUserName += query[i].UserName + ",";
                            }
                            else
                            {
                                strDepartName = query[i].DepartmentName;
                                strDepartID = query[i].DepartID.ToString();
                                strUserID += query[i].UserID.ToString() + ",";
                                strUserName += query[i].UserName + ",";

                                sb.Append("<tr departid=" + strDepartID + " class=\"datagrid-header-row\"><td >" + strDepartName + "</td><td  userid=" + strUserID + ">" + strUserName + "</td></tr>");
                                strDepartName = strDepartID = strUserID = strUserName = "";
                            }
                        }
                        else
                        {
                            strDepartName = query[i].DepartmentName;
                            strDepartID = query[i].DepartID.ToString();
                            strUserID += query[i].UserID.ToString() + ",";
                            strUserName += query[i].UserName + ",";

                            sb.Append("<tr departid=" + strDepartID + " class=\"datagrid-header-row\"><td >" + strDepartName + "</td><td  userid=" + strUserID + ">" + strUserName + "</td></tr>");
                            strDepartName = strDepartID = strUserID = strUserName = "";
                        }
                    }
                    ViewBag.HTML = new MvcHtmlString(sb.ToString());
                }
            }
            return View();
        }

        /// <summary>
        /// 获取公司组织机构
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCompany()
        {
            //IBLL.IORG_DepartmentBLL m_BLL = new BLL.ORG_DepartmentBLL();

            using (SysEntities db = new SysEntities())
            {
                var query = db.ORG_Department.Where(o => true);

                string menuID = "9"; //Request["menuID"].ToString();
                string where = "";
                if (menuID != "")
                {
                    #region  权限配置
                    int departmentScope = base.MenuDepartmentScopeAuthority(menuID);
                    if (departmentScope == (int)Common.DepartmentScopeAuthority.无限制)// 
                    {
                        /*****组织机构—菜单表“DepartmentAuthority（是否拥有部门业务权限配置功能）”字段为“Y”时才有如下逻辑判断
                         *
                         *不管“DepartmentAuthority”是否为“Y”，都写上以下判断逻辑，程序也没有问题
                         * 
                         */
                        string departments = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19";// base.MenuDepartmentAuthority(menuID);
                        //query = from a in query
                        //        where (a.ID > 0 && departments.Split(',').Contains(a.ID))
                        //        select g.Key.BH;
                        string[] departs = departments.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        int[] newDeparts = Array.ConvertAll<string, int>(departs, s => int.Parse(s));
                        query = from a in db.ORG_Department
                                select a;
                        query = query.Where(a => newDeparts.Contains(a.ID));
                        //if (!string.IsNullOrEmpty(departments))
                        //{

                        //    where += " and b.ID in (" + departments + ")";

                        //}
                        //else//查询所有部门数据
                        //{

                        //}
                    }
                    else if (departmentScope == (int)Common.DepartmentScopeAuthority.本机构及下属机构)//本机构及下属机构
                    {
                        //    //当前用户直属机构

                        //查询本机构及下属机构所有部门数据
                        var branch = db.ORG_Department.FirstOrDefault(o => o.ID == LoginInfo.BranchID);

                        query = from a in db.ORG_Department
                                where a.XYBZ == "Y" && a.LeftValue >= branch.LeftValue && a.RightValue <= branch.RightValue
                                select a;

                    }
                    else if (departmentScope == (int)Common.DepartmentScopeAuthority.本机构) //本机构
                    {
                        //    //查询本机构所有部门数据

                        query = from a in db.ORG_Department
                                where a.XYBZ == "Y" && a.BranchID == LoginInfo.BranchID
                                select a;
                    }
                    else if (departmentScope == (int)Common.DepartmentScopeAuthority.本部门及其下属部门)//本部门及其下属部门
                    {
                        //当前用户所属部门
                        ORG_Department department = db.ORG_Department.FirstOrDefault(o => o.ID == LoginInfo.DepartmentID);
                        //查询本部门及下属部门所有部门数据
                        query = from a in db.ORG_Department
                                where a.XYBZ == "Y" && a.LeftValue >= department.LeftValue && a.RightValue <= department.RightValue
                                select a;
                    }
                    else if (departmentScope == (int)Common.DepartmentScopeAuthority.本部门) //本部门
                    {
                        //    //查询本部门所有用户数据
                        query = from a in db.ORG_Department.Where(o => o.XYBZ == "Y" && o.BranchID == LoginInfo.DepartmentID)
                                select a;
                    }
                    else if (departmentScope == (int)Common.DepartmentScopeAuthority.本人) //本人
                    {
                        query = null;
                    }
                    #endregion
                }

                //var data = new ActionResult
                //{
                //    rows = query.Select(s => new
                //    {
                //        ID = s.ID,
                //        Name = s.CompanyName,
                //        ParentID = s.ParentID
                //    })
                //};

                string content = Newtonsoft.Json.JsonConvert.SerializeObject(query.ToList());
                content = content.Replace("Checked", "checked");
                return Content(content);
            }

            //string backColumn = "ID,DepartmentName,ParentID,DepartmentType";
            //DataTable dt = bll.ExecuteTable("ORG_Department b", where, backColumn);
            //ViewBag.Json = Newtonsoft.Json.JsonConvert.SerializeObject(dt);

        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <returns></returns>
        public string GetUser()
        {
            int departID = Convert.ToInt32(Request["departID"].ToString());
            StringBuilder sb = new StringBuilder();
            using (SysEntities db = new SysEntities())
            {
                List<ORG_User> listUser = db.ORG_User.Where(o => o.XYBZ == "Y" && o.ORG_Department_ID == departID).ToList();

                sb.Append("<table with='100%' border='0'><tr>");
                for (int i = 0; i < listUser.Count; i++)
                {

                    if (i % 6 != 0)
                    {

                        sb.Append("<td>");
                        sb.AppendFormat("<input type='checkbox' onclick='selectChk(this)' value='{0}' userid='{0}' name='chk' text='{1}' departid='{2}'>{1}</input>", listUser[i].ID, listUser[i].RName, listUser[i].ORG_Department_ID);
                        sb.Append("</td>");
                    }
                    else
                    {
                        sb.Append("</tr>");
                        sb.Append("<tr>");
                        sb.Append("<td>");
                        sb.AppendFormat("<input type='checkbox' onclick='selectChk(this)' value='{0}' userid='{0}'  text='{1}' name='chk' departid='{2}'>{1}</input>", listUser[i].ID, listUser[i].RName, listUser[i].ORG_Department_ID);
                        sb.Append("</td>"); ;
                    }
                }
                sb.Append("</tr>");
                sb.Append("</table>");
            }
            return sb.ToString();
        }
    }
    public class TreeList
    {
        public int id { set; get; }
        public string name { set; get; }
        public int nodeLevel { set; get; }
        public string XYBZ { set; get; }
        public List<TreeList> children { set; get; }

        public bool Checked { set; get; }
        public bool open { set; get; }
    }
}