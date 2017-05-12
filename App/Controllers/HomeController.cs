using System.Data;
using System.Data.Common;
using System.IO;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Langben.App.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            ViewBag.BranchName = base.LoginInfo.BranchName;
            ViewBag.DepartmentName = base.LoginInfo.DepartmentName;
            ViewBag.RealName = base.LoginInfo.RealName;
            string strParents = string.Empty;

            if (Session["AuthorityMenu"] != null)
            {

                string strMenu = Session["AuthorityMenu"].ToString();

                string jsonMenu = Common.EncryptAndDecrypte.DecrypteString(strMenu);


                JsonSerializer serializer = new JsonSerializer();
                StringReader sr = new StringReader(jsonMenu);
                object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<ORG_Menu>));
                List<ORG_Menu> list = o as List<ORG_Menu>;

                string parentHtml =
                    "<div data-options=\"iconCls:'tu0625'\" title=\"{0}\"><div class=\"easyui-panel\" fit=\"true\" border=\"false\"><ul class=\"easyui-tree\">{1}</ul></div></div>";
                string childHtml =
                    "<li data-options=\"iconCls:'tu0202'\"><a href=\"#\" icon=\"tu0202\" rel=\"{1}\">{0}</a></li>";
                var lstMenu = list.Where(w => w.IsDisplay == "Y").OrderBy(b => b.Sort).ThenBy(t => t.ORG_Menu_ID);

                if (list != null)
                {
                    var lstParent = lstMenu.Where(w => w.ParentID == 0 && w.NodeLevel == 1);

                    foreach (ORG_Menu pMenu in lstParent)
                    {
                        var lstChild = list.Where(w => w.ParentID == pMenu.ORG_Menu_ID && w.NodeLevel == 2);
                        string strChilds = string.Empty;
                        foreach (ORG_Menu cMenu in lstChild)
                        {
                            strChilds += string.Format(childHtml, cMenu.MenuName, cMenu.MenuUrl);
                        }
                        strParents += string.Format(parentHtml, pMenu.MenuName, strChilds);

                    }
                }
            }
            else
            {
                strParents = "菜单加载失败，请重新登录";
            }
            ViewBag.MenuTreeHtml = strParents;
            return View();
        }

        private string GetMenuHtml()
        {
            return "";
        }
        public ActionResult Welcome()
        {

            return View();
        }

    }

    public class ORG_Menu
    {
        public int ORG_Menu_ID { get; set; }
        public string ORG_MenuOp_ID_List { get; set; }
        public string MenuName { get; set; }
        public string MenuUrl { get; set; }
        public int NodeLevel { get; set; }
        public int ParentID { get; set; }
        public string IsDisplay { get; set; }
        public string Sort { get; set; }
    }
}

