using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using Langben.DAL;
using Langben.BLL;
using System.Web.Mvc;
using System.Text;
using System.EnterpriseServices;
using System.Configuration;
using Models;
using Langben.DAL.Model;
using Langben.IBLL;

namespace Langben.App.Controllers
{
    /// <summary>
    /// Allot
    /// </summary>
    public class AllotController : BaseController
    {

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Index()
        {
            ViewBag.UserList = getSubordinatesData("1012");
            return View();
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult Details()
        {
          
            return View();
        }


           
        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="order">排序字段</param>
        /// <param name="sort">升序asc（默认）还是降序desc</param>
        /// <param name="search">查询条件</param>
        /// <returns></returns>
        [HttpPost]
        [SupportFilter]
        public JsonResult GetData(string id, int page, int rows, string order, string sort, string search)
        {

            int total = 0;

            List<Allot> queryData = m_BLL.GetByParam(id, page, rows, order, sort, search, ref total, LoginInfo.UserID);




            


            return Json(new datagrid
            {
                total = total,
                rows = queryData.Select(s => new
                {
                    CompanyId = s.CompanyId
					,CompanyName = s.CompanyName
					,City = s.City
					,CityId = s.CityId
					,EmployeeAddSum = s.EmployeeAddSum
					,EmployeeServerSum = s.EmployeeServerSum
					,RealName_ZR = s.RealName_ZR
					,RealName_YG = s.RealName_YG
					,UserID_ZR = s.UserID_ZR
					,UserID_YG = s.UserID_YG
					,AllotState = s.AllotState
					
                }

                    )
            });
        }


        #region 员工客服经理分配详细信息
        /// <summary>
        /// 员工客服经理分配详细信息
        /// </summary>
        /// <param name="id">企业id</param>
        /// <returns></returns>
        [SupportFilter]
        public ActionResult EmpAllotDetail(string CompanyId, string UserID_YG, string CompanyName)
        {
            //ViewBag.id = id;
            //ViewBag.usernid = usernid;
            //ViewBag.compname = compname;
            var citycode = Request["CityId"];
            ViewBag.citycode = citycode;
            ViewBag.UserList = getSubordinatesData("1012");
            ViewBag.Json = Newtonsoft.Json.JsonConvert.SerializeObject(ViewBag.UserList);
            int uid = 0;

            ViewBag.id = CompanyId;
            ViewBag.usernid = UserID_YG;
            ViewBag.compname = CompanyName;
            if (int.TryParse(UserID_YG, out uid))
            {
                ViewBag.count = GetSercount(uid.ToString());
            }
            return View();
        }
        #endregion




        #region 获取客服人数

        public ActionResult GetSercountss(string UserID_YG)
        {
           var json = Newtonsoft.Json.JsonConvert.SerializeObject(GetSercount(UserID_YG));
           return Content(json);
        }
        #endregion


        #region 获取当前客服服务人数
        /// <summary>
        /// 获取当前客服服务人数
        /// </summary>
        /// <param name="UserID_YG">客服人员id</param>
        /// <returns></returns>


        public string GetSercount(string UserID_YG)
        {

            int userid = 0;
            int.TryParse(UserID_YG,out userid);

            string CityId = LoginInfo.CompanyCityCode;
            int count = 0;
            using (var ent = new SysEntities())
            {
                try
                {
                    List<string> StateList = new List<string>();
                    StateList.Add(EmployeeAdd_State.待责任客服确认.ToString());
                    StateList.Add(EmployeeAdd_State.待员工客服确认.ToString());
                    StateList.Add(EmployeeAdd_State.员工客服已确认.ToString());
                    StateList.Add(EmployeeAdd_State.社保专员已提取.ToString());
                    StateList.Add(EmployeeAdd_State.申报成功.ToString());
                    var CrmComp = ent.UserCityCompany.Where(a => true);
                    var ComEmpRel = ent.CompanyEmployeeRelation.Where(a => true);
                    var Emp = ent.EmployeeAdd.Where(a => true);
                    CrmComp = CrmComp.Where(a => a.UserID_YG == userid);
                    ComEmpRel = ComEmpRel.Where(a => true);
                    Emp = Emp.Where(a => StateList.Contains(a.State));
                    var list = (from a in CrmComp
                                join b in ComEmpRel on a.CompanyId equals b.CompanyId
                                join c in Emp on b.Id equals c.CompanyEmployeeRelationId
                                select b).Distinct();
                    if (list != null )
                    {
                        count = list.Count();
                    }




                }
                catch (Exception e)
                {
                    count = -1;
                }

            }
            return count.ToString();
        }
        #endregion
        #region 获取当前权限客服人员
        public ActionResult kfry() 
        {
            string citycode = Request["citycode"];         

            var UserList = getUserbyCityData("1012", citycode);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(UserList);            
            return Content(json);
        }
        #endregion

        IBLL.IAllotBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public AllotController()
            : this(new AllotBLL()) { }

        public AllotController(AllotBLL bll)
        {
            m_BLL = bll;
        }
        IBLL.IORG_UserBLL userBLL = new ORG_UserBLL();
        #region 获取登录人所有下属
        /// <summary>
        /// 获取登录人所有下属
        /// </summary>
        /// <returns></returns>

        public Common.ClientResult.DataResult Subordinates()
        {
            string menuID = "1012";//员工客服经理分配员工客服
            List<ORG_User> queryData = getSubordinatesData(menuID);

            var data = new Common.ClientResult.DataResult
            {
                rows = queryData
            };
            return data;
        }

        public List<ORG_User> getSubordinatesData(string menuID)
        {
            #region 权限
            string departments = "";
            int departmentScope = base.MenuDepartmentScopeAuthority(menuID);
            if (departmentScope == (int)DepartmentScopeAuthority.无限制)//无限制
            {
                //部门业务权限
                departments = MenuDepartmentAuthority(menuID);
            }
            #endregion

            List<ORG_User> queryData = userBLL.GetGroupUsers(Common.ORG_Group_Code.YGKF.ToString(), departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID);
            return queryData;
        }

        public List<ORG_User> getUserbyCityData(string menuID,string citycode)
        {
            #region 权限
            string departments = "";
            int departmentScope = base.MenuDepartmentScopeAuthority(menuID);
            if (departmentScope == (int)DepartmentScopeAuthority.无限制)//无限制
            {
                //部门业务权限
                departments = MenuDepartmentAuthority(menuID);
            }
            #endregion

            List<ORG_User> queryData = userBLL.GetGroupUsers(Common.ORG_Group_Code.YGKF.ToString(), departmentScope, departments, LoginInfo.BranchID, LoginInfo.DepartmentID, LoginInfo.UserID);
            
            citycode = citycode.Trim();
            List<int> temArray = new List<int>();
            foreach (ORG_User item in queryData)
            {
                temArray.Add(item.ID);
            }
            int[] intArray = temArray.ToArray();

           SysEntities db = new SysEntities();

           var usrquery = db.ORG_User.Where(o => intArray.Contains(o.ID));
           var query = db.ORG_UserCity.Where(o => o.CityId == citycode && intArray.Contains(o.UserID));
           var query1 = (from a in usrquery join b in query on a.ID equals b.UserID select a).ToList<ORG_User>();
           return query1;
        }
        #endregion
    }
}


