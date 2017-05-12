using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL
{
    public partial class ORG_MenuOpRepository : BaseRepository<ORG_MenuOp>, IDisposable
    {
        /// <summary>
        /// 查询的数据
        /// </summary>
        public IQueryable<dynamic> GetMenuOpList(SysEntities db, string menuId)
        {
            var query = from a in db.ORG_MenuOp
                        join b in db.ORG_Menu on a.ORG_Menu_ID equals b.ID
                        where a.XYBZ == "Y"
                        select new
                        {
                            Code=a.ID,
                            ID=a.ID,
                            MenuOpName=a.MenuOpName,
                            ORG_Menu_ID=a.ORG_Menu_ID,
                            MenuName=b.MenuName,
                            Sort=a.Sort
                        };
            
            if (!string.IsNullOrEmpty(menuId))
            {
                int mID=int.Parse(menuId);
                query = query.Where(e => e.ORG_Menu_ID == mID);
            }
            return query.OrderByDescending(e => e.ID);
        }

        /// <summary>
        /// 根据菜单功能查询是否有重复的
        /// </summary>
        /// <param name="db"></param>
        /// <param name="OpId"></param>
        /// <returns></returns>
        public IQueryable<ORG_MenuOp> GetMenuOpId(SysEntities db,string OpId)
        {
            var query = from a in db.ORG_MenuOp
                        where a.ID == OpId
                        select a;
            return query;
        }

        /// <summary>
        /// 删除菜单功能（将标志位置为‘N’）
        /// </summary>
        /// <param name="db">数据访问的上下文</param>
        /// <param name="id">菜单功能编号</param>
        /// <returns></returns>
        public void DeleteMenuOp(SysEntities db, string[] ids)
        {
            var query = from menuop in db.ORG_MenuOp
                        where ids.Contains(menuop.ID)
                        select menuop;

            foreach (var item in query)
            {
                item.XYBZ = "N";
            }
        }
    }
}
