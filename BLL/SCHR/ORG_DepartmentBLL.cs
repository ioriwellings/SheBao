using Langben.DAL;
using Langben.DAL.SCHR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.BLL
{
    public class ORG_DepartmentBLL : IBLL.IORG_DepartmentBLL, IDisposable
    {
        /// <summary>
        /// 私有的数据访问上下文
        /// </summary>
        protected SysEntities db;
        /// <summary>
        /// 费用_社保支出的数据库访问对象
        /// </summary>
        ORG_DepartmentRepository repository = new ORG_DepartmentRepository();
        /// <summary>
        /// 构造函数，默认加载数据访问上下文
        /// </summary>
        public ORG_DepartmentBLL()
        {
            db = new SysEntities();
        }

        public List<ORG_Department> GetAll()
        {
            return repository.GetAll(db).ToList();
        }

        /// <summary>
        /// 查询的数据
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="order">升序asc（默认）还是降序desc</param>
        /// <param name="sort">排序字段</param>
        /// <param name="search">查询条件</param>
        /// <param name="total">结果集的总数</param>
        /// <returns>结果集</returns>
        public List<ORG_Department> GetByParam(int? id, int page, int rows, string order, string sort, string search, ref int total)
        {
            IQueryable<ORG_Department> queryData = repository.GetData(db, order, sort, search);
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

        /// <summary>
        /// 查询的数据 /*在6.0版本中 新增*/
        /// </summary>
        /// <param name="id">额外的参数</param>
        /// <param name="order">升序asc（默认）还是降序desc</param>
        /// <param name="sort">排序字段</param>
        /// <param name="search">查询条件</param>
        /// <returns>结果集</returns>
        public List<ORG_Department> GetByParam(string id, string order, string sort, string search)
        {
            IQueryable<ORG_Department> queryData = repository.GetData(db, order, sort, search);

            return queryData.ToList();
        }

        public void Dispose()
        {
        }
    }
}
