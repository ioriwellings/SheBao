using Langben.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Langben.IBLL
{
    public partial interface ICRM_CompanyToBranchBLL
    {
        /// <summary>
        /// 查询是否属于登陆者的企业
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userid">操作的用户</param>
        /// <param name="CompanyId">企业ID</param>
        /// <returns></returns>
        List<CRM_CompanyToBranch> GetAuthority(int userid, int CompanyId);
    }
}
