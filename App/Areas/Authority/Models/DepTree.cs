using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Langben.App.Areas.Authority.Models
{
    public partial class DepTree
    {
        public DepTree()
        {
            ID = 0;
        }

        #region 属性

        /// <summary>
        /// 部门ID
        /// </summary>
        public int ID { set; get; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { set; get; }
        /// <summary>
        /// 父部门ID
        /// </summary>
        public int ParentID { set; get; }
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Checked { set; get; }

        /// <summary>
        /// 是否打开节点
        /// </summary>
        public bool open { set; get; }

        #endregion

    }
}