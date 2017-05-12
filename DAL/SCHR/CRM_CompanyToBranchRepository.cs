using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL
{
    public partial class CRM_CompanyToBranchRepository : BaseRepository<CRM_CompanyToBranch>, IDisposable
    {
        /// <summary>
        /// 查询是否属于登陆者的企业
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userid">操作的用户</param>
        /// <param name="CompanyId">企业ID</param>
        /// <returns></returns>
        public IQueryable<CRM_CompanyToBranch> GetAuthority(SysEntities db, int userid, int CompanyId)
        {
            // 获取特定用户组所有人员
            var query = from a in db.CRM_CompanyToBranch
                        join b in db.CompanyEmployeeRelation on a.CRM_Company_ID equals b.CompanyId
                        where (a.UserID_YG == userid || a.UserID_ZR == userid) &&  a.CRM_Company_ID == CompanyId
                        select a;
            return query;
        }
    }
}
