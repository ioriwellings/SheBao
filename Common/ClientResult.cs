using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
   public static class ClientResult
    {
        /// <summary>
        /// WebApi增、删、改操作返回
        /// </summary>
        public class Result
        {
            public ClientCode Code { get; set; }
            public string Message { get; set; }
        }

        /// <summary>
        /// WebApi分页获取数据
        /// </summary>
        public class DataResult
        {
            /// <summary>
            /// 总数
            /// </summary>
            public int total;
            /// <summary>
            /// 行数据集
            /// </summary>
            public dynamic rows;

        }

       /// <summary>
       /// 导出Excel的返回结果
       /// </summary>
        public class UrlResult
        {
            public ClientCode Code { get; set; }
            public string Message { get; set; }
            public string URL { get; set; }
        }
    }
}
