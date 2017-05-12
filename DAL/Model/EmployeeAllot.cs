using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    public class EmployeeAllot
    {

        [Display(Name = "公司主键")]
        public int? CompanyId { get; set; }

         [Display(Name = "公司名称")]
        public string CompanyName { get; set; }

        [Display(Name = "缴纳地")]
        public string City { get; set; }

        [Display(Name = "申报人数")]
        public int EmployeeAddSum { get; set; }

        [Display(Name = "服务人数")]
        public int EmployeeServerSum { get; set; }

        [Display(Name = "员工客服")]
        public int? UserID_YG { get; set; }

        [Display(Name = "责任客服")]
        public int? UserID_ZR { get; set; }

        [Display(Name = "分配情况")]
        public string AllotState { get; set; }

        [Display(Name = "员工客服")]
        public string RealName_YG { get; set; }

        [Display(Name = "责任客服")]
        public string RealName_ZR { get; set; }
    }    
}
 

