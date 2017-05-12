using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL.Model
{
    public class CRM_CompanyInfo
    {
        public CRM_Company BasicInfo { get; set; }
        public CRM_CompanyContract Contract { get; set; }
        public CRM_CompanyFinance_Bill Bill { get; set; }
        public string LinkMan { get; set; }
        public string Bank { get; set; }
        //public string Bill { get; set; }
        public string Payment { get; set; }
        public string Price { get; set; }
        public string LadderPrice { get; set; }
        public string  SheBaoInfo { get; set; }

    }
    public class SheBao
    {
        public List<SheBaoInfo> data { get; set; }
    }
    public class SheBaoInfo
    {
        public string CompanyID { get; set; }
        public string JiaoNaDi { get; set; }
        public string GongShangZhengCeName { get; set; }
        public string GongShangZhengCe { get; set; }
        public string GongJiJinZhengCeName { get; set; }
        public string GongJiJinZhengCe { get; set; }
        public string QiYeSheBaoName { get; set; }
        public string QiYeSheBaoAccount { get; set; }
        public string GongJiJinName { get; set; }
        public string GongJiJinAccount { get; set; }
        public string State { get; set; }
    }
}
