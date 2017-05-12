using Common;
using Langben.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace Langben.App.Areas.Authority.Controllers
{
    public class RoleApiController : ApiController
    {
        /// <summary>
        /// 配置角色人员
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public Common.ClientResult.Result EditRoleUser(string ID, [FromBody]RoleUserModel data)
        {
            ValidationErrors validationErrors = new ValidationErrors();
            StringBuilder sbError = new StringBuilder();
            SysEntities SysEntitiesO2O = new SysEntities();
            int RoleID = Convert.ToInt32(ID);
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    JavaScriptSerializer ser = new JavaScriptSerializer();
                    RoleUserModel listModel = ser.Deserialize<RoleUserModel>(data.DataInfo);
                    if (listModel != null)
                    {

                        if (!string.IsNullOrEmpty(listModel.ReturnData[0].DepartID.ToString()))
                        {
                            var aa = SysEntitiesO2O.ORG_UserRole.Where(a => a.ORG_Role_ID == RoleID);

                            if (aa.Count() > 0)
                            {
                                foreach (var item in aa)
                                {
                                    SysEntitiesO2O.ORG_UserRole.Remove(item);
                                    SysEntitiesO2O.SaveChanges();
                                }
                            }
                            //插入
                            for (int i = 0; i < listModel.ReturnData.Count; i++)
                            {
                                ORG_UserRole RoleUser = new ORG_UserRole();
                                RoleUser.ORG_Role_ID = RoleID;
                                RoleUser.ORG_User_ID = Convert.ToInt32(listModel.ReturnData[i].UserID);
                                SysEntitiesO2O.ORG_UserRole.Add(RoleUser);
                                SysEntitiesO2O.SaveChanges();
                            }
                        }
                    }
                    scope.Complete();
                    Common.ClientResult.Result result = new Common.ClientResult.Result();
                    result.Code = ClientCode.Succeed;
                    result.Message = "保存成功";
                    return result;
                }
            }
            catch (Exception er)
            {
                Common.ClientResult.Result result = new Common.ClientResult.Result();
                result.Code = ClientCode.Fail;
                result.Message = "保存失败";
                return result;
            }
        }
    }

    public class RoleUserModel
    {
        public string DataInfo { get; set; }
        public List<RoleUser> ReturnData { get; set; }
    }

    public class RoleUser
    {
        public string DepartID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
    }
}
