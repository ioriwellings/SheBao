using Langben.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.BLL
{
    public partial class CRM_CompanyToBranchBLL : IBLL.ICRM_CompanyToBranchBLL, IDisposable
    {
         /// <summary>
        /// 根据用户名查询用户组中的类型
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<CRM_CompanyToBranch> GetAuthority(int userid, int CompanyId)
        {
            List<CRM_CompanyToBranch> list = new List<CRM_CompanyToBranch>();

            IQueryable<CRM_CompanyToBranch> query = repository.GetAuthority(db, userid, CompanyId);
            list = query.OrderBy(c => c.ID).ToList();
            return list;
        }
    }
}
