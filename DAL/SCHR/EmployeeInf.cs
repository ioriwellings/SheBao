using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL.SCHR
{
    public class EmployeeInf
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { set; get; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string code { set; get; }
        /// <summary>
        /// 企业
        /// </summary>
        public string company { set; get; }
    }
    public class InsuranceKindtype
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { set; get; }
        /// <summary>
        /// 险种名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 标签
        /// </summary>
        public string Tag { set; get; }
       
    }
}
