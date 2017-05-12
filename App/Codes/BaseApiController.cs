using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using NPOI.HPSF;
using System.IO;
using System.Data;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Web;
using Common;
using System.Reflection;
using System.Web.Script.Serialization;

namespace Models
{
    public class BaseApiController : ApiController
    {
        #region 弃用
        /// <summary>
        /// 获取当前登陆人的名称
        /// </summary>
        /// <returns></returns>
        //public string GetCurrentPerson()
        //{
        //    Account account = GetCurrentAccount();
        //    if (account != null && !string.IsNullOrWhiteSpace(account.PersonName))
        //    {
        //        return account.PersonName;
        //    }
        //    return string.Empty;
        //}
        /// <summary>
        /// 获取当前登陆人的账户信息
        /// </summary>
        /// <returns>账户信息</returns>
        //public Account GetCurrentAccount()
        //{
        //    if (HttpContext.Current.Session["account"] != null)
        //    {
        //        Account account = (Account)HttpContext.Current.Session["account"];
        //        return account;
        //    }
        //    return null;
        //}
        #endregion

        #region 获取当前登录信息
        /// <summary>
        /// 获取当前登录信息
        /// </summary>
        protected LoginUserInfo LoginInfo
        {
            get
            {
                HttpCookie loginCookie = HttpContext.Current.Request.Cookies["Login"];
                LoginUserInfo model = new LoginUserInfo()
                {
                    //CompanyID = Convert.ToInt32(loginCookie.Values["CompanyID"]),
                    //CompanyName = loginCookie.Values["CompanyName"],
                    UserID = Convert.ToInt32(loginCookie.Values["UserID"]),
                    UserCode = loginCookie.Values["UserCode"],
                    LoginName = loginCookie.Values["LoginName"],
                    RealName = loginCookie.Values["RealName"],
                    DepartmentID = Convert.ToInt32(loginCookie.Values["DepartmentID"]),
                    DepartmentName = loginCookie.Values["DepartmentName"],
                    //PositionID = Convert.ToInt32(loginCookie.Values["PositionID"]),
                    //PositionName = loginCookie.Values["PositionName"],
                    Sex = loginCookie.Values["Sex"],
                    UserName = "【" + loginCookie.Values["DepartmentName"] + "】" + loginCookie.Values["RealName"],
                   // CompanyCityCode = loginCookie.Values["CompanyCityCode"]
                    CompanyCityCode = "130100",
                    BranchID = Convert.ToInt32(loginCookie.Values["BranchID"]),
                };
                return model;
            }
        }

        #endregion

        #region 获取权限

        #region 获取菜单对应的操作按钮权限
        /// <summary>
        /// 获取菜单对应的操作按钮权限
        /// </summary>
        /// <param name="buttonCode">操作按钮代码</param>
        /// <returns>true：有权限 false：无权限</returns>
        protected bool MenuOpAuthority(string buttonCode)
        {
            //return true;
            if (HttpContext.Current.Request.Cookies["Login"] != null)
            {
                //HttpCookie loginCookie = HttpContext.Current.Request.Cookies["Login"];
                System.Web.HttpContext context = System.Web.HttpContext.Current;  
                //从登陆cookie中获取当前用户当前系统的所有菜单操作权限及业务数据范围权限
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(Common.EncryptAndDecrypte.DecrypteString(context.Session["AuthorityMenu"].ToString()));
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow[] rows = dt.Select("ORG_Menu_ID='" + buttonCode.Substring(0, buttonCode.LastIndexOf('-')) + "'");
                    if (rows != null && rows.Length > 0)
                    {
                        if (rows[0]["ORG_MenuOp_ID_List"].ToString().Split(',').Contains(buttonCode))
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                    return false;

            }
            else
            {
                return false;
            }
        }
        #endregion


        #region 获取菜单对应的部门范围权限
        /// <summary>
        /// 获取菜单对应的业部门范围权限
        /// </summary>
        /// <param name="menuID">菜单ID</param>
        /// <returns>0：无限制 1：本部门及其下属部门 2：本部门 3：本人</returns>
        protected int MenuDepartmentScopeAuthority(string menuID)
        {
            //return 0;
            if (HttpContext.Current.Request.Cookies["Login"] != null)
            {
                //HttpCookie loginCookie = HttpContext.Current.Request.Cookies["Login"];
                System.Web.HttpContext context = System.Web.HttpContext.Current;  
                //从登陆cookie中获取当前用户当前系统的所有菜单操作权限及业务数据范围权限
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(Common.EncryptAndDecrypte.DecrypteString(context.Session["AuthorityDepartmentScope"].ToString()));
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow[] rows = dt.Select("ORG_Menu_ID='" + menuID + "'");
                    if (rows != null && rows.Length > 0)
                        return Convert.ToInt32(rows[0]["DepartmentScope"].ToString());
                    else
                        return 0;
                }
                else
                    return 0;
            }
            else
                return 0;
        }

        #endregion

        #region 获取菜单对应的部门权限
        /// <summary>
        /// 获取菜单对应的部门权限
        /// </summary>
        /// <param name="menuID">菜单ID</param>
        /// <returns>多个部门ID组合的字符串，用","分隔</returns>
        protected string MenuDepartmentAuthority(string menuID)
        {
            string department = "";
            //return department;
            if (HttpContext.Current.Request.Cookies["Login"] != null)
            {
                //HttpCookie loginCookie = HttpContext.Current.Request.Cookies["Login"];
                System.Web.HttpContext context = System.Web.HttpContext.Current;  
                //从登陆cookie中获取当前用户当前系统的所有菜单部门数据权限
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(Common.EncryptAndDecrypte.DecrypteString(context.Session["AuthorityDepartment"].ToString()));
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow[] rows = dt.Select("ORG_Menu_ID='" + menuID + "'");
                    if (rows != null && rows.Length > 0)
                    {
                        department = rows[0]["ORG_Department_ID_List"].ToString();
                    }
                }
                else department = "";
            }
            return department;
        }

        #endregion

        #endregion

        #region 登录人员信息
        /// <summary>
        /// 登录人员信息
        /// </summary>
        public class LoginUserInfo
        {

            /// <summary>
            /// 当前人员ID
            /// </summary>
            public int UserID { get; set; }
            /// <summary>
            /// 当前登录人编码
            /// </summary>
            public string UserCode { get; set; }
            /// <summary>
            /// 登录名
            /// </summary>
            public string LoginName { get; set; }
            /// <summary>
            /// 真实姓名
            /// </summary>
            public string RealName { get; set; }
            /// <summary>
            /// 性别
            /// </summary>
            public string Sex { get; set; }
            /// <summary>
            /// 所属部门ID
            /// </summary>
            public int DepartmentID { get; set; }
            /// <summary>
            /// 所属部门名称
            /// </summary>
            public string DepartmentName { get; set; }
            /// <summary>
            /// 直属机构ID
            /// </summary>
            public int BranchID { get; set; }
            /// <summary>
            /// 直属机构名称
            /// </summary>
            public string BranchName { get; set; }
            /// <summary>
            /// 城市代码
            /// </summary>
            public string CompanyCityCode { get; set; }
            /// <summary>
            /// 所属岗位ID
            /// </summary>
           // public int PositionID { get; set; }
            /// <summary>
            /// 所属岗位名称
            /// </summary>
          //  public string PositionName { get; set; }
            public string UserName { get; set; }
            //public int CompanyID { get; set; }
            //public string CompanyName { get; set; }

        }
        #endregion

        /// <summary>
        /// 导出数据集到excle
        /// </summary>
        /// <param name="titles">第一行显示的标题名称</param>
        /// <param name="fields">字段</param>
        /// <param name="query">数据集</param>
        /// <param name="path">excle模版的位置</param>
        /// <param name="from">显示的标题默认行数为1</param>
        /// <returns></returns>
        public static string WriteExcle(string[] titles, string[] fields, dynamic[] query, string path = @"~/up/b.xls", int from = 1)
        {
            HSSFWorkbook _book = new HSSFWorkbook();
            string xlsPath = System.Web.HttpContext.Current.Server.MapPath(path);

            FileStream file = new FileStream(xlsPath, FileMode.Open, FileAccess.Read);
            IWorkbook hssfworkbook = new HSSFWorkbook(file);
            ISheet sheet = hssfworkbook.GetSheet("Sheet1");
            string guid = Guid.NewGuid().ToString();
            string saveFileName = xlsPath.Path(guid);

            Dictionary<string, string> propertyName;
            PropertyInfo[] properties;
            //标题行  
            HSSFRow dataRow = sheet.CreateRow(0) as HSSFRow;
            for (int i = 0; i < titles.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(titles[i]))
                {

                    dataRow.CreateCell(i).SetCellValue(titles[i]); //列值

                }
            }
            //内容行
            for (int i = 0; i < query.Length; i++)
            {
                propertyName = new Dictionary<string, string>();
                if (query[i] == null)
                {
                    continue;
                }
                Type type = query[i].GetType();
                properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (PropertyInfo property in properties)
                {
                    object o = property.GetValue(query[i], null);
                    if (!string.IsNullOrEmpty(property.Name) && o != null)
                    {
                        propertyName.Add(property.Name, o.ToString());
                    }
                }
                int j = 0;
                dataRow = sheet.CreateRow(i + from) as HSSFRow;
                fields.All(a =>
                {

                    if (propertyName.ContainsKey(a)) //列名
                    {

                        dataRow.CreateCell(j).SetCellValue(propertyName[a]);
                        //列值
                    }
                    j++;
                    return true;
                });
            }
            sheet.ForceFormulaRecalculation = true;
            using (FileStream fileWrite = new FileStream(saveFileName, FileMode.Create))
            {
                hssfworkbook.Write(fileWrite);
            }


            //一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet  
            return string.Format("../../up/{0}.xls", guid);
            //记录日志

        }
        public class GetDataParam
        {
            public string sort { get; set; }
            public string order { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
            public int id { get; set; }
            public string search { get; set; }
        }
        public class ExportParam
        {
            public int id { get; set; }
            public string title { get; set; }
            public string field { get; set; }
            public string sortName { get; set; }
            public string sortOrder { get; set; }
            public string search { get; set; }

        }

        public static HttpResponseMessage toJson(Object obj)
        {
            String str;
            if (obj is String || obj is Char)
            {
                str = obj.ToString();
            }
            else
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                str = serializer.Serialize(obj);
            }
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, System.Text.Encoding.GetEncoding("UTF-8"), "application/json") };

            return result;
        }

        public bool IsValidation()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
            HttpRequestBase request = context.Request;//定义传统request对象
            string ts = request.Params["ts"];
            string sign = request.Params["sign"];
            try
            {
                var diff = Convert.ToDouble(ts) - Convert.ToDouble(DateTime.Now.ToString("yyyyMMddHHmm"));
                if (diff > 5 || diff < -5)
                {
                    return false;
                }
                string signNew = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(ts + "28C504617A57E2B5", "MD5");
                if (signNew.ToUpper() != sign.ToUpper())
                {
                    return false;
                }
                return true;


            }
            catch
            {

                return false;
            }


        }
    }
}
