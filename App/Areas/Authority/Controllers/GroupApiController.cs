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
    public class GroupApiController : ApiController
    {

        #region  用户组 修改/新增    王帅
        public Common.ClientResult.Result EditGroup(string ID, string Code, string GroupName, string Des)
        {
            ValidationErrors validationErrors = new ValidationErrors();
            StringBuilder sbError = new StringBuilder();
            SysEntities SysEntitiesO2O = new SysEntities();
            GroupName = HttpUtility.HtmlDecode(GroupName);
            Des = HttpUtility.HtmlDecode(Des);
            Code = HttpUtility.HtmlDecode(Code);
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    int IntId = 0;
                    int.TryParse(ID, out IntId);
                    var GropTable = SysEntitiesO2O.ORG_Group.FirstOrDefault(o => true && o.ID == IntId);
                    // GropTable = GropTable.FirstOrDefault(o => o.ID == IntId);
                    if (GropTable != null)
                    {
                        GropTable.Code = Code;
                        GropTable.GroupName = GroupName;
                        GropTable.Des = Des;
                        GropTable.XYBZ = "Y";
                    }
                    else
                    {
                        var ORG_GroupBLL = new BLL.ORG_GroupBLL();
                        ORG_Group Model = new ORG_Group();
                        Model.Code = Code;
                        Model.GroupName = GroupName;
                        Model.Des = Des;
                        Model.XYBZ = "Y";
                        ORG_GroupBLL.Create(ref validationErrors, Model);
                    }


                    //最后保存
                    SysEntitiesO2O.SaveChanges();
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
        #endregion

        #region  用户组 删除    王帅
        public Common.ClientResult.Result Shanchu(string IDs)
        {
            ValidationErrors validationErrors = new ValidationErrors();
            StringBuilder sbError = new StringBuilder();
            SysEntities SysEntitiesO2O = new SysEntities();

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    int[] intArray; List<int> intArrayuser = new List<int>();
                    string[] strArray = IDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    intArray = Array.ConvertAll<string, int>(strArray, s => int.Parse(s));
                    var GropTable = SysEntitiesO2O.ORG_Group.FirstOrDefault(o => true && intArray.Contains(o.ID));
                    // GropTable = GropTable.FirstOrDefault(o => o.ID == IntId);
                    if (GropTable != null)
                    {
                        var ORG_GroupBLL = new BLL.ORG_GroupBLL();
                        ORG_Group Model = new ORG_Group();
                        Model = GropTable;
                        ORG_GroupBLL.DeleteCollection(ref validationErrors, intArray);
                    }
                    var GropUserTable = SysEntitiesO2O.ORG_GroupUser.Where(o => true && intArray.Contains(o.ORG_Group_ID)).ToList();
                    if (GropUserTable != null)
                    {
                        foreach (var item in GropUserTable)
                        {
                            int j = 0;
                            intArrayuser.Add(item.ID);
                            j++;
                        }

                    }
                    if (intArrayuser.Count > 0)
                    {

                        var ORG_GroupUserBLL = new BLL.ORG_GroupUserBLL();
                        int[] allid = new int[intArrayuser.Count];
                        for (int i = 0; i < intArrayuser.Count(); i++)
                        {
                            allid[i] = intArrayuser[i];
                        }

                        ORG_GroupUserBLL.DeleteCollection(ref validationErrors, allid);
                    }


                    SysEntitiesO2O.SaveChanges();
                    scope.Complete();
                    Common.ClientResult.Result result = new Common.ClientResult.Result();
                    result.Code = ClientCode.Succeed;
                    result.Message = "删除成功";
                    return result;
                }
            }
            catch (Exception er)
            {
                Common.ClientResult.Result result = new Common.ClientResult.Result();
                result.Code = ClientCode.Fail;
                result.Message = "删除失败";
                return result;
            }


        }
        #endregion

        #region  用户组用户    王帅
        [HttpPost]
        public Common.ClientResult.Result EditGroupUser(string ID, [FromBody]GroupUserModel data)
        {
            ValidationErrors validationErrors = new ValidationErrors();
            StringBuilder sbError = new StringBuilder();
            SysEntities SysEntitiesO2O = new SysEntities();
            int GroupID = Convert.ToInt32(ID);
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    JavaScriptSerializer ser = new JavaScriptSerializer();
                    GroupUserModel listModel = ser.Deserialize<GroupUserModel>(data.DataInfo);
                    if (listModel != null)
                    {

                        if (!string.IsNullOrEmpty(listModel.ReturnData[0].DepartID.ToString()))
                        {
                            var aa = SysEntitiesO2O.ORG_GroupUser.Where(a => a.ORG_Group_ID == GroupID);
                            //List<ORG_GroupUser> customer = from c in SysEntitiesO2O.ORG_GroupUser
                            //                where c.ID == GroupID
                            //                select c;
                            if (aa.Count() > 0)
                            {
                                foreach (var item in aa)
                                {
                                    SysEntitiesO2O.ORG_GroupUser.Remove(item);
                                    SysEntitiesO2O.SaveChanges();
                                }

                            }
                            //插入
                            for (int i = 0; i < listModel.ReturnData.Count; i++)
                            {
                                ORG_GroupUser GroupUser = new ORG_GroupUser();
                                GroupUser.ORG_Group_ID = GroupID;
                                GroupUser.ORG_User_ID = Convert.ToInt32(listModel.ReturnData[i].UserID);
                                SysEntitiesO2O.ORG_GroupUser.Add(GroupUser);
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
                result.Message = "删除失败";
                return result;
            }


        }
        #endregion
    }
    public class GroupUserModel
    {
        public string DataInfo { get; set; }
        public List<GroupUser> ReturnData { get; set; }
    }
    public class GroupUser
    {
        public string DepartID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
    }
}
