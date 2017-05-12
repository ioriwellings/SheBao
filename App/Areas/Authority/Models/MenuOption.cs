using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Langben.App.Areas.Authority.Models
{
    public class MenuOption
    {
        #region 属性

        /// <summary>
        ///主键
        /// </summary>
        public string ID { get; set; }


        /// <summary>
        ///所属菜单
        /// </summary>
        public int SYS_Menu_ID { get; set; }


        /// <summary>
        ///功能名称
        /// </summary>
        public string MenuOpName { get; set; }


        /// <summary>
        ///排序（从小到大）
        /// </summary>
        public int? Sort { get; set; }


        /// <summary>
        ///是否启用(Y：启用 N：关闭)
        /// </summary>
        public string XYBZ { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsChecked { get; set; }

        #endregion
    }
}