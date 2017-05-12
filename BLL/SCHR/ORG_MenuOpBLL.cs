using Common;
using Langben.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Langben.BLL
{
    public partial class ORG_MenuOpBLL : IBLL.IORG_MenuOpBLL, IDisposable
    {
        /// <summary>
        /// 查询的数据
        /// </summary>
        public List<dynamic> GetMenuOpList(int? id, int page, int rows, string menuId, ref int total)
        {
            IQueryable<dynamic> queryData = repository.GetMenuOpList(db, menuId);
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
        /// 根据菜单功能查询是否有重复的
        /// </summary>
        /// <param name="db"></param>
        /// <param name="OpId"></param>
        /// <returns></returns>
        public List<ORG_MenuOp> GetMenuOpId(string OpId)
        {
            IQueryable<ORG_MenuOp> queryData = repository.GetMenuOpId(db, OpId);
            return queryData.ToList();
        }

        /// <summary>
        /// 删除菜单功能（将标志位置为‘N’）
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="id">菜单功能编号</param>
        /// <returns></returns>
        public bool DeleteMenuOp(ref ValidationErrors validationErrors, string[] ids)
        {
            try
            {
                if (ids != null)
                {
                    using (TransactionScope transactionScope = new TransactionScope())
                    {
                        repository.DeleteMenuOp(db, ids);
                        if (ids.Length == repository.Save(db))
                        {
                            transactionScope.Complete();
                            return true;
                        }
                        else
                        {
                            Transaction.Current.Rollback();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                validationErrors.Add(ex.Message);
                ExceptionsHander.WriteExceptions(ex);
            }
            return false;
        }
    }
}
