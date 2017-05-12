using Common;
using Langben.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.IBLL
{
    public partial interface IORG_MenuOpBLL
    {
        /// <summary>
        /// 查询的数据
        /// </summary>
        List<dynamic> GetMenuOpList(int? id, int page, int rows, string menuId, ref int total);

        /// <summary>
        /// 根据菜单功能查询是否有重复的
        /// </summary>
        /// <param name="OpId"></param>
        /// <returns></returns>
        List<ORG_MenuOp> GetMenuOpId(string OpId);

        /// <summary>
        /// 删除菜单功能（将标志位置为‘N’）
        /// </summary>
        /// <param name="validationErrors">返回的错误信息</param>
        /// <param name="id">菜单功能编号</param>
        /// <returns></returns>
        bool DeleteMenuOp(ref ValidationErrors validationErrors, string[] ids);
    }
}
