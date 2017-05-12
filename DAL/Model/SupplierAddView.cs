using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL.Model
{
    public class SupplierAddView
    {
        [Display(Name = "报增表ID")]
        public int? EmployeeAddId { get; set; }

        [Display(Name = "员工关系表主键")]
        public int? CompanyEmployeeRelationId { get; set; }

        [Display(Name = "政策主键")]
        public int? PoliceInsuranceId { get; set; }

        [Display(Name = "公司主键")]
        public int? CompanyId { get; set; }

        [Display(Name = "员工主键")]
        public int? EmployeeId { get; set; }

        [Display(Name = "公司编号")]
        public string CompanyCode { get; set; }

        [Display(Name = "公司名称")]
        public string CompanyName { get; set; }

        [Display(Name = "员工姓名")]
        public string EmployeeName { get; set; }

        [Display(Name = "证件号码")]
        public string CertificateNumber { get; set; }

        [Display(Name = "联系电话")]
        public string Telephone { get; set; }

        [Display(Name = "社保缴纳地")]
        public string City { get; set; }

        [Display(Name = "社保缴纳地id")]
        public string CityID { get; set; }

        [Display(Name = "客服姓名")]
        public string CustomerName { get; set; }

        [Display(Name = "分公司")]
        public string BranchName { get; set; }

        [Display(Name = "险种")]
        public int? InsuranceKindId { get; set; }

        [Display(Name = "社保自然月")]
        public int? YearMonth { get; set; }

        [Display(Name = "社保操作时间")]
        public DateTime? OperationTime { get; set; }


        [Display(Name = "基数")]
        public decimal? CompanyNumber { get; set; }

        [Display(Name = "单位比例")]
        public decimal? CompanyPercent { get; set; }

        [Display(Name = "单位缴纳金额")]
        public decimal? CompanyMoney
        {
            get
            {
                if (CompanyNumber == null || CompanyPercent == null)
                {
                    return null;
                }
                else
                {
                    decimal num = (decimal)CompanyNumber;
                    decimal per = (decimal)CompanyPercent;
                    return Math.Round(num * per, 2);
                }
            }
        }

        [Display(Name = "个人基数")]
        public decimal? EmployeeNumber { get; set; }

        [Display(Name = "个人比例")]
        public decimal? EmployeePercent { get; set; }

        [Display(Name = "个人缴纳金额")]
        public decimal? EmployeeMoney
        {
            get
            {
                if (EmployeeNumber == null || EmployeePercent == null)
                {
                    return null;
                }
                else
                {
                    decimal num = (decimal)EmployeeNumber;
                    decimal per = (decimal)EmployeePercent;
                    return Math.Round(num * per, 2);
                }
            }
        }

        [Display(Name = "状态")]
        public string State { get; set; }

        [Display(Name = "政策")]
        public string PoliceInsuranceName { get; set; }

        [Display(Name = "工资")]
        public decimal? Wage { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "供应商备注")]
        public string SupplierRemark { get; set; }
    }

    public class EmployeeCityInsuranceKindId
    {
        [Display(Name = "员工主键")]
        public int? EmployeeId { get; set; }

        [Display(Name = "社保缴纳地id")]
        public string CityID { get; set; }

        [Display(Name = "险种")]
        public int? InsuranceKindId { get; set; }
    }
}
