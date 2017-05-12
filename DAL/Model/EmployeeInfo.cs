using Langben.DAL;
using Langben.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Langben.DAL
{
    public class EmployeeInfo
    {

        public int empId { get; set; }
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string Empname { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string CertificateNumber { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public string CertificateType { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 户口性质
        /// </summary>
        public string AccountType { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// 所属公司
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// 所属岗位
        /// </summary>
        public string Station { get; set; }

        /// <summary>
        /// 社保缴纳地
        /// </summary>
        public string Citys { get; set; }

        /// <summary>
        /// 户口性质
        /// </summary>
        public int PoliceAccountNatureId { get; set; }

        /// <summary>
        /// 联系方式状态
        /// </summary>
        public string CState { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 联系信息备注
        /// </summary>
        public string CRemark { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string Bank { get; set; }

        /// <summary>
        /// 支行名称
        /// </summary>
        public string BranchBank { get; set; }

        /// <summary>
        /// 银行帐号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 收款人名称
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 银行信息状态
        /// </summary>
        public string BState { get; set; }

        /// <summary>
        /// 银行信息备注
        /// </summary>
        public string BRemark { get; set; }

        public Nullable<int> zrkf { get; set; }
        public Nullable<int> ygkf { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }


        public List<EmployeeBanks> bankList { get; set; }

        public List<EmployeeContacts> contactList { get; set; }

        public Employee BasicInfo { get; set; }
        public EmployeeBank empBank { get; set; }
        public EmployeeContact empContacts { get; set; }

    }

    public class EmployeeAddExcle
    {

        /// <summary>
        ///  企业ID
        /// </summary>
        public int CompanyId { get; set; }


        /// <summary>
        ///  企业名称
        /// </summary>
        public string CompanyName { get; set; }


        /// <summary>
        ///  社保缴纳地
        /// </summary>
        public string City { get; set; }


        /// <summary>
        ///  公司岗位
        /// </summary>
        public string Station { get; set; }


        /// <summary>
        ///  户口性质名称
        /// </summary>
        public string PoliceAccountNatureName { get; set; }

        /// <summary>
        ///  户口性质
        /// </summary>
        public int PoliceAccountNature { get; set; }


        /// <summary>
        ///  员工ID
        /// </summary>
        public int empId { get; set; }
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string Empname { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string CertificateNumber { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public string CertificateType { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 户口性质
        /// </summary>
        public string AccountType { get; set; }



        /// <summary>
        /// 户口所在地
        /// </summary>
        public string AccountAddress { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// 联系方式状态
        /// </summary>
        public string CState { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 联系信息备注
        /// </summary>
        public string CRemark { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string Bank { get; set; }

        /// <summary>
        /// 支行名称
        /// </summary>
        public string BranchBank { get; set; }

        /// <summary>
        /// 银行帐号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 收款人名称
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 银行信息状态
        /// </summary>
        public string BState { get; set; }

        /// <summary>
        /// 银行信息备注
        /// </summary>
        public string BRemark { get; set; }
    }

}