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

namespace Langben.App.Controllers
{
    /// <summary>
    /// Allot
    /// </summary>
    public class AllotApiController : BaseApiController
    {
        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="getParam"></param>
        /// <returns></returns>
        public Common.ClientResult.DataResult PostData([FromBody]GetDataParam getParam)
        {
            int total = 0;
            List<Allot> queryData = m_BLL.GetByParam(getParam.id.ToString(), getParam.page, getParam.rows, getParam.order, getParam.sort, getParam.search, ref total);
            var data = new Common.ClientResult.DataResult
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
					

                })
            };
            return data;
        }
 
  

        IBLL.IAllotBLL m_BLL;

        ValidationErrors validationErrors = new ValidationErrors();

        public AllotApiController()
            : this(new AllotBLL()) { }

        public AllotApiController(AllotBLL bll)
        {
            m_BLL = bll;
        }
        
    }
}


