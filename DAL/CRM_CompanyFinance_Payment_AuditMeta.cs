﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Langben.DAL
{
    [MetadataType(typeof(CRM_CompanyFinance_Payment_AuditMetadata))]//使用CRM_CompanyFinance_Payment_AuditMetadata对CRM_CompanyFinance_Payment_Audit进行数据验证
    public partial class CRM_CompanyFinance_Payment_Audit 
    {
      
        #region 自定义属性，即由数据实体扩展的实体
        
        #endregion

    }
    public partial class CRM_CompanyFinance_Payment_AuditMetadata
    {
			[ScaffoldColumn(false)]
			[Display(Name = "主键", Order = 1)]
			public object ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业财务信息（付款）", Order = 2)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CRM_CompanyFinance_Payment_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "待审核企业", Order = 3)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CRM_Company_Audit_ID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "企业", Order = 4)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CRM_Company_ID { get; set; }

            [ScaffoldColumn(true)]
            [Required(ErrorMessage = "请填写{0}")]
			[Display(Name = "付款名称", Order = 5)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object PaymentName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建时间", Order = 6)]
			[DataType(System.ComponentModel.DataAnnotations.DataType.DateTime,ErrorMessage="时间格式不正确")]
			public DateTime? CreateTime { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人ID", Order = 7)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? CreateUserID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "创建人名称", Order = 8)]
			[StringLength(200, ErrorMessage = "长度不可超过200")]
			public object CreateUserName { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "所属分支机构", Order = 9)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? BranchID { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "操作状态（0：失败 1：待处理 2：成功）", Order = 10)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? OperateStatus { get; set; }

			[ScaffoldColumn(true)]
			[Display(Name = "操作节点（1：销售经理 2：质控）", Order = 11)]
			[Range(0,2147483646, ErrorMessage="数值超出范围")]
			public int? OperateNode { get; set; }


    }
}
 

