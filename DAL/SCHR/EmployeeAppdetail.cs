using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL.SCHR
{
    public class EmployeeAppdetail
    {
        /// <summary>
        /// 保险种类
        /// </summary>
        public string bxzl { set; get; }
        /// <summary>
        /// 政策类型
        /// </summary>
        public string zclx { set; get; }
        /// <summary>
        ///社保基数
        /// </summary>
        public decimal sbjs { set; get; }
        /// <summary>
        /// 起缴时间
        /// </summary>
        public DateTime qjsj { set; get; }
        /// <summary>
        ///报增类型
        /// </summary>
        public string bzlx { set; get; }

        /// <summary>
        ///bzyf
        /// </summary>
        public int YearMonth { set; get; }
    }
}
