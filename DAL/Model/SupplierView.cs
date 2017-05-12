using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL.Model
{
    public class SupplierView
    {
        public int  Id { get; set; }

        [Display(Name = "编号", Order = 2)]
        [StringLength(200, ErrorMessage = "长度不可超过200")]
        public string Code { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [ScaffoldColumn(true)]
        [Display(Name = "名称", Order = 3)]
        [StringLength(200, ErrorMessage = "长度不可超过200")]
        public string Name { get; set; }
        
        [Display(Name = "组织机构代码证", Order = 4)]
        [StringLength(200, ErrorMessage = "长度不可超过200")]
        public string OrganizationCode { get; set; }
        
        [Required(ErrorMessage = "请填写{0}")]
        [ScaffoldColumn(true)]
        [Display(Name = "注册地址", Order = 5)]
        [StringLength(200, ErrorMessage = "长度不可超过200")]
        public string RegisterAddress { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [ScaffoldColumn(true)]
        [Display(Name = "办公地址", Order = 6)]
        [StringLength(200, ErrorMessage = "长度不可超过200")]
        public string OfficeAddress { get; set; }

        public string Status { get; set; }

        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<int> CreateUserID { get; set; }
        public string CreateUserName { get; set; }

        public Nullable<int> CustomerServiceId { get; set; }
        public string CustomerServiceName { get; set; }
        //[Required(ErrorMessage = "请选择{0}")]
        //[ScaffoldColumn(true)]
        //[Display(Name = "缴纳地", Order = 7)]
        //public string NatureCity { get; set; }
        public string NatureCityId { get; set; }
    }
}
