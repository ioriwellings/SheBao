using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using System.EnterpriseServices;
using System.Configuration;
using Models;
using Common;
using Langben.DAL;
using Langben.BLL;
using System.Web.Http;
using Langben.App.Models;
using System.Reflection;

namespace Langben.App.Areas.CRM.Controllers
{
    /// <summary>
    /// 客户_企业信息_待审核
    /// </summary>
    public class CRM_ZD_HYApiController : BaseApiController
    {
        /// <summary>
        /// 根据父节点得到下级行业类别
        /// </summary>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        public string GetHYNode(string id)
        {
            List<CRM_ZD_HY> listHY = new List<CRM_ZD_HY>();

            string sqlWhere = "ParentCodeDDL_String&" + id + "^XYBZDDL_String&Y";
            listHY = m_BLL.GetByParam(null, "asc", "code", sqlWhere);
            return Newtonsoft.Json.JsonConvert.SerializeObject(listHY);
        }

        #region 内置
   
        #endregion

        IBLL.ICRM_ZD_HYBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public CRM_ZD_HYApiController()
            : this(new CRM_ZD_HYBLL()) { }

        public CRM_ZD_HYApiController(CRM_ZD_HYBLL bll)
        {
            m_BLL = bll;
        }
        
    }
}


