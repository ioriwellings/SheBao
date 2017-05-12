using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL
{
    public partial class EmployeeMiddleImportRecordRepository
    {
        public IQueryable<EmployeeMiddleImportRecord> GetDataByParam(SysEntities db, string createUser, string beginTime, string endTime)
        {
                var queryData = db.EmployeeMiddleImportRecord.Where(e=>true);
                if (!string.IsNullOrEmpty(createUser))
                {
                    queryData = queryData.Where(e => e.CreateUserName.Contains(createUser));
                }
                if (beginTime != "" && endTime != "")
                {
                    DateTime dtBegin = Convert.ToDateTime(beginTime);
                    DateTime dtEnd = Convert.ToDateTime(endTime).AddDays(1);
                    queryData = queryData.Where(e => (e.CreateTime >= dtBegin && e.CreateTime < dtEnd));
                }
                return queryData.OrderByDescending(e => e.CreateTime);
        }
    }
}
