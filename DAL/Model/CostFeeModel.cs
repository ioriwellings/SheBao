using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL.Model
{
    /// <summary>
    /// 费用表信息（联合企业表，查询出企业对应名称）
    /// </summary>
    public class CostFeeModel
    {
        public int ID { get; set; }
        public int CostTableType { get; set; }
        public decimal ChargeCost { get; set; }
        public decimal ServiceCost { get; set; }
        public string Remark { get; set; }
        public int Status { get; set; }
        public int CRM_Company_ID { get; set; }
        public System.DateTime CreateTime { get; set; }
        public int CreateUserID { get; set; }
        public string CreateUserName { get; set; }
        public int BranchID { get; set; }
        public int YearMonth { get; set; }
        public string SerialNumber { get; set; }  // 批次号

        public string CompanyName { get; set; }  // 公司名称
        public string CompanyCode { get; set; }  // 公司编号
        public int? UserID_ZR { get; set; }  // 责任客服
        public string UserName_ZR { get; set; } // 责任客服名称

        public int? CreateFrom { get; set; }// 费用来源
        public string Suppler { get; set; }// 
        public int? Suppler_ID { get; set; }// 
        
    };
}
