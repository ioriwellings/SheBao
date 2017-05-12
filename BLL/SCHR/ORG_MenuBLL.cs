using Common;
using Langben.DAL;
using Langben.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Langben.BLL
{
    public partial class ORG_MenuBLL : IBLL.IORG_MenuBLL, IDisposable
    {
        #region 信伟青
        /// <summary>
        /// 根据条件查询菜单列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<ORG_Menu> GetMenuList(int id)
        {
            List<ORG_Menu> list = repository.GetMenuList(db, id).ToList();
            List<ORG_Menu> source = new List<ORG_Menu>();
            foreach (var item in list.Where(x=>x.NodeLevel==1))
            {
                source.Add(item);
                List<ORG_Menu> drs = list.Where(x => x.ParentID == item.ID).ToList();
                this.BindMenu(list, drs, source, 1);//递归调用生成分分支结构树
            }
            return source;
        }

        /// <summary>
        /// 获取上级菜单树
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="drs"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private List<ORG_Menu> BindMenu(List<ORG_Menu> list, List<ORG_Menu> drs, List<ORG_Menu> source, int n)
        {
            foreach (var row in drs)
            {
                char nbsp = (char)0xA0;
                string ss = row.MenuName.ToString();
                int length = row.MenuName.ToString().Length;
                row.MenuName= row.MenuName.ToString().PadLeft(length + 6 * n, nbsp);
                source.Add(row);
                List<ORG_Menu> dr = list.Where(x => x.ParentID == row.ID).ToList();
                this.BindMenu(list, dr, source, n + 1);
            }
            return source;
        }
        #endregion

        /// <summary>
        /// 获取菜单树（包含上级菜单名称）
        /// </summary>
        /// <returns></returns>
        public List<MenuTreeModel> GetMenuTreeList()
        {
            IQueryable<MenuTreeModel> menuTree = repository.GetMenuTreeList(db);

            return menuTree.ToList();
        }

        /// <summary>
        /// 批量删除菜单数据
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="ids">菜单编号</param>
        /// <returns></returns>
        public bool DeleteMenuCollection(ref ValidationErrors validationErrors, int[] ids)
        {
            try
            {
                if (ids != null)
                {
                    using (TransactionScope transactionScope = new TransactionScope())
                    {
                        repository.DeleteMenu(db, ids);
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
