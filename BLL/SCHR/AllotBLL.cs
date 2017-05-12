using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Langben.DAL;
using Common;

namespace Langben.BLL
{
    /// <summary>
    /// Allot 
    /// </summary>
    public partial class AllotBLL : IBLL.IAllotBLL, IDisposable
    {
        /// <summary>
        /// 查询的数据
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="order">排序字段</param>
        /// <param name="sort">升序asc（默认）还是降序desc</param>
        /// <param name="search">查询条件</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        public List<Allot> GetByParam(string id, int page, int rows, string order, string sort, string search, ref int total, int UserID)
        {
            
            IQueryable<Allot> queryData = repository.DaoChuData(db, order, sort, search);

            //根据登录人信息增加缴纳地查询条件
            SysEntities SysEntitiesO2O = new SysEntities();
            List<ORG_UserCity> ucList = SysEntitiesO2O.ORG_UserCity.Where(a => a.UserID == UserID).ToList();
            List<string> cityList = ucList.Select(a => a.CityId).ToList();
           // queryData = queryData.Where(a => cityList.Contains(a.CityId));

            total = queryData.Count();
            if (total > 0)
            {
                if (page <= 1)
                {
                    queryData = queryData.Take(rows);
                }
                else
                {
                    queryData = queryData.Skip((page - 1) * rows).Take(rows);
                }

            }
            return queryData.ToList();
        }
    }
}

