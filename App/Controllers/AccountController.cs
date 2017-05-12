using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Langben.DAL;
using Models;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Langben.App.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/
        public ActionResult Index()
        {
            return View();
        }

        #region 登录
        [HttpPost]
        public ActionResult Login(string lname, string pwd)
        {
            try
            {
                 //dynamic login = this.LoginCheck(lname, pwd, "3");//用户登录（调用API接口）

                if (lname != "admin" || pwd != "123456")
                {
                    return Json(new { Code = "0", Message = "用户名或密码错误！" });
                }
                dynamic login = new { Code = "ok" };
                if (login.Code == "ok")//登录成功
                {
                    HttpCookie loginCookie = new HttpCookie("Login");


                    //loginCookie.Values["CompanyID"] = login.Result.CompanyID;
                    //loginCookie.Values["CompanyName"] = login.Result.CompanyName;
                    //loginCookie.Values["UserID"] = login.Result.UserID;//登录人ID
                    //loginCookie.Values["UserCode"] = login.Result.UserCode;//人员编码（暂时无用）
                    //loginCookie.Values["LoginName"] = login.Result.LoginName;//登录名
                    //loginCookie.Values["RealName"] = login.Result.RealName;//登录人真实姓名
                    //loginCookie.Values["Sex"] = login.Result.Sex;//性别
                    //loginCookie.Values["DepartmentID"] = login.Result.DepartmentID;//所属部门ID
                    //loginCookie.Values["DepartmentName"] = login.Result.DepartmentName;//所属部门名称
                    //loginCookie.Values["BranchID"] = login.Result.BranchID;//直属机构ID
                    //loginCookie.Values["BranchName"] = login.Result.BranchName;//直属机构名称
                    //loginCookie.Values["PositionID"] = login.Result.PositionID;//所属岗位ID
                    //loginCookie.Values["PositionName"] = login.Result.PositionName;//所属岗位名称

                    //loginCookie.Values["UserName"] = "【" + login.Result.DepartmentName + "】" + login.Result.RealName;
                    //loginCookie.Values["CompanyCityCode"] = login.Result.CompanyCityCode;

                    loginCookie.Values["CompanyID"] = "1";
                    loginCookie.Values["CompanyName"] = "狼奔";
                    loginCookie.Values["UserID"] = "1";//登录人ID
                    loginCookie.Values["UserCode"] = "001";//人员编码（暂时无用）
                    loginCookie.Values["LoginName"] = "admin";//登录名
                    loginCookie.Values["RealName"] = "刘腾飞";//登录人真实姓名
                    loginCookie.Values["Sex"] = "男";//性别
                    loginCookie.Values["DepartmentID"] = "1";//所属部门ID
                    loginCookie.Values["DepartmentName"] = "研发中心";//所属部门名称
                    loginCookie.Values["BranchID"] = "1";//直属机构ID
                    loginCookie.Values["BranchName"] = "狼奔科技";//直属机构名称
                    loginCookie.Values["PositionID"] = "2";//所属岗位ID
                    loginCookie.Values["PositionName"] = "开发工程师";//所属岗位名称
                    loginCookie.Values["UserName"] = "【研发中心】刘腾飞";
                    loginCookie.Values["CompanyCityCode"] = "110100";

                    #region 获取用户权限
                    Database dbEntity = EnterpriseLibraryContainer.Current.GetInstance<Database>("connStr");

                    DbCommand cmd = dbEntity.GetStoredProcCommand("Pro_GetUserAuthority");
                    dbEntity.AddInParameter(cmd, "@userID", DbType.Int32, "1");
                    cmd.CommandTimeout = 0;
                    dbEntity.ExecuteNonQuery(cmd);
                    DataSet ds = dbEntity.ExecuteDataSet(cmd);

                    DataTable authorityMenu = ds.Tables[0];//菜单及菜单操作权限
                    DataTable authorityDepartmentScope = ds.Tables[1];//部门范围权限
                    DataTable authorityDepartment = ds.Tables[2];//部门业务权限
                    //loginCookie.Values["AuthorityMenu"] = Common.EncryptAndDecrypte.EncryptString(Newtonsoft.Json.JsonConvert.SerializeObject(authorityMenu));
                    //loginCookie.Values["AuthorityDepartmentScope"] = Common.EncryptAndDecrypte.EncryptString(Newtonsoft.Json.JsonConvert.SerializeObject(authorityDepartmentScope));
                    //loginCookie.Values["AuthorityDepartment"] = Common.EncryptAndDecrypte.EncryptString(Newtonsoft.Json.JsonConvert.SerializeObject(authorityDepartment));

                    Session["AuthorityMenu"] = Common.EncryptAndDecrypte.EncryptString(Newtonsoft.Json.JsonConvert.SerializeObject(authorityMenu));
                    Session["AuthorityDepartmentScope"] = Common.EncryptAndDecrypte.EncryptString(Newtonsoft.Json.JsonConvert.SerializeObject(authorityDepartmentScope));
                    Session["AuthorityDepartment"] = Common.EncryptAndDecrypte.EncryptString(Newtonsoft.Json.JsonConvert.SerializeObject(authorityDepartment));

                    #endregion
                    Response.Cookies.Add(loginCookie);

                    return Json(new { Code = "ok" });
                }
                else
                {
                    string code = login.Code;
                    string message = login.Message;
                    return Json(new { Code = code, Message = message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Code = "-1", Message = "系统错误：" + ex.ToString() });
            }

        }
        #endregion

        #region 退出登录
        public ActionResult Logout()
        {
            //清空所有Cookie
            //int n = Request.Cookies.Count;
            //string webSite = "";
            //string loginUrl = "";
            //for (int i = 0; i < n; i++)
            //{
            //    HttpCookie myCookie = Request.Cookies[i];
            //    if (myCookie.Name == "accountHR")
            //        continue;
            //    if (myCookie.Name == "accountEmployee")
            //        continue;
            //    if (myCookie.Name == "LoginHR")
            //    {
            //        webSite = myCookie.Values["WebSite"].ToString();
            //        loginUrl = myCookie.Values["LogInUrl"].ToString();
            //    }
            //    myCookie.Expires = DateTime.Today.AddDays(-1);
            //    Response.Cookies.Add(myCookie);
            //}

            //清空所有Cookie
            int n = Request.Cookies.Count;

            for (int i = 0; i < n; i++)
            {
                HttpCookie myCookie = Request.Cookies[i];

                myCookie.Expires = DateTime.Today.AddDays(-1);
                Response.Cookies.Add(myCookie);
            }
            Request.Cookies.Clear();

            return RedirectToAction("Index", "Account");
        }

        #endregion


        #region Private
        /// <summary>
        /// 用户登录（调用API接口）
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <param name="pwd">密码（未加密）</param>
        /// <param name="sys">当前系统ID</param>
        /// <returns></returns>
        public dynamic LoginCheck(string loginName, string pwd, string sysID)
        {
            dynamic fruit = "";
            try
            {
                //api地址
                string url = System.Configuration.ConfigurationManager.AppSettings["Employeeurl"] + "api/authoritysystem/login"
                    + "?loginName=" + loginName + "&pwd=" + pwd + "&system=" + sysID;

                //fruit = CallAPI.PostAPI.PostGetJson<string>(url, "");
                fruit = CallAPI.GetAPI.GetJson<dynamic>(url);

                return fruit;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion
    }
}