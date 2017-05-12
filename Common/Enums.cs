using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///Enum 的摘要说明
/// </summary>
/// 
namespace Common
{
    #region 增员状态
    /// <summary>
    /// 增员状态
    /// </summary>
    public enum EmployeeAdd_State
    {

        待责任客服确认 = 1,
        待员工客服经理分配 = 2,
        待员工客服确认 = 3,
        员工客服已确认 = 4,
        社保专员已提取 = 5,
        申报成功 = 6,
        申报失败 = 7,
        已报减 = 8,
        终止 = 9,
        责任客服未通过 = 10,
        待供应商客服提取 = 11,
        已发送供应商 = 12
    }
    #endregion

    #region 减员状态
    /// <summary>
    /// 减员状态
    /// </summary>
    public enum EmployeeStopPayment_State
    {

        待责任客服确认 = 1,
        待员工客服经理分配 = 2,
        待员工客服确认 = 3,
        员工客服已确认 = 4,
        社保专员已提取 = 5,
        申报成功 = 6,
        申报失败 = 7,
        终止 = 8,
        责任客服未通过 = 9,
        待供应商客服提取 = 10,
        已发送供应商 = 11
    }
    #endregion

    #region 政策状态
    /// <summary>
    /// 政策状态
    /// </summary>
    public enum PoliceOperation_Style
    {

        报增 = 1,
        报减 = 2
    }
    #endregion

    #region 费用类型
    public enum EmployeeMiddle_PaymentStyle
    {

        正常 = 1,
        补缴 = 2,
        退费 = 3,
        补收 = 4,
        补差 = 5,
        调整 = 6,
        补中间段 = 7

    }
    #endregion

    #region 费用来源
    public enum CostTable_CreateFrom
    {
        本地费用 = 1,
        供应商费用 = 2,
        供应商预算费用 = 3

    }
    #endregion

    #region 状态
    public enum Status
    {
        停用 = 0,
        启用 = 1,
        修改中 = 2

    }
    #endregion

    #region 审核状态
    public enum AuditStatus
    {
        失败 = 0,
        待处理 = 1,
        成功 = 2
    }
    #endregion

    #region 操作类型
    public enum OperateType
    {
        添加 = 1,
        修改 = 2
    }
    #endregion

    #region 社保种类
    public enum EmployeeAdd_InsuranceKindId
    {

        养老 = 1,
        医疗 = 2,
        工伤 = 3,
        失业 = 4,
        公积金 = 5,
        生育 = 6,
        // 医疗大额 = 7,
        补充公积金 = 8,
        大病 = 9,
        地方附加医疗=10
    }
    #endregion

    #region 社保种类
    public enum EmployeeMiddle_InsuranceKind
    {

        养老 = 1,
        医疗 = 2,
        工伤 = 3,
        失业 = 4,
        公积金 = 5,
        生育 = 6,
        // 医疗大额 = 7,
        补充公积金 = 8,
        大病 = 9,
        地方附加医疗=10,
        其他社保费用 = 12,
    }
    #endregion

    #region 费用大类
    public enum CostType
    {
        养老 = 1,
        医疗 = 2,
        工伤 = 3,
        失业 = 4,
        公积金 = 5,
        生育 = 6,
        //医疗大额 = 7,
        补充公积金 = 8,
        大病 = 9,
        其他费用 = 11,
        其他社保费用 = 12,
        //医疗外农 = 13
    }
    #endregion

    #region 社保支出社保种类
    public enum CostPay_InsuranceKind
    {

        养老 = 1,
        医疗 = 2,
        工伤 = 3,
        失业 = 4,
        公积金 = 5,
        生育 = 6,
        医疗大额 = 7,
        补充公积金 = 8,
        //大病 = 9,
        ////医疗外农=10
        //其他社保费用 = 12,
    }
    #endregion

    #region 分配状态

    public enum AllotState
    {
        未分配 = 1,
        已分配 = 2,
    }
    #endregion

    #region 提取状态

    public enum CollectState
    {
        未提取 = 1,
        已提取 = 2,
    }
    #endregion

    #region 费用表类型
    public enum COST_Table_CostTableType
    {
        单立户 = 1,
        大户代理 = 2
    }
    #endregion

    #region 费用表状态
    public enum COST_Table_Status
    {
        待责任客服验证 = 1,
        待客户确认 = 2,
        客户作废 = 3,
        待责任客服确认 = 4,
        责任客服作废 = 5,
        待核销 = 6,
        财务作废 = 7,
        已核销 = 8,
        已支付 = 9,
        已开票 = 10,
        供应商客服导入 = 11,
        供应商客服作废 = 12,


    }
    #endregion

    #region 支出费用表状态
    public enum COST_PayRecord_Status
    {
        未锁定 = 0,
        已锁定 = 1,
        已对比 = 2
    }
    #endregion

    #region 用户组CODE
    public enum ORG_Group_Code
    {
        /// <summary>
        /// 销售
        /// </summary>
        XS = 0,
        /// <summary>
        /// 责任客服
        /// </summary>
        ZRKF = 1,
        /// <summary>
        /// 员工客服
        /// </summary>
        YGKF = 2,
        /// <summary>
        /// 社保客服
        /// </summary>
        SBKF = 3,
        /// <summary>
        /// 质控
        /// </summary>
        ZKKF = 4,
        /// <summary>
        /// 供应商客服
        /// </summary>
        GYSKF = 5

    }
    #endregion

    #region 部门权限范围（权限相关）
    public enum DepartmentScopeAuthority
    {
        无限制 = 0,
        本机构及下属机构 = 1,
        本机构 = 2,
        本部门及其下属部门 = 3,
        本部门 = 4,
        本人 = 5
    }
    #endregion

    #region 证件类型

    public enum CertificateType
    {
        居民身份证 = 1,
        护照 = 2,
    }
    #endregion

    #region 社保补中间段申报状态
    /// <summary>
    /// 社保补中间段申报状态
    /// </summary>
    public enum EmployeeGoonPayment_STATUS
    {
        待责任客服确认 = 1,
        待员工客服确认 = 2,
        员工客服已确认 = 3,
        社保专员已提取 = 4,
        申报成功 = 5,
        申报失败 = 6,
        终止 = 7,
        责任客服未通过 = 8
    }
    #endregion

    #region 员工调基状态
    /// <summary>
    /// 员工调基状态
    /// </summary>
    public enum EmployeeGoonPayment2_STATUS
    {
        待责任客服确认 = 1,
        待员工客服确认 = 2,
        员工客服已确认 = 3,
        社保专员已提取 = 4,
        申报成功 = 5,
        申报失败 = 6,
        终止 = 7,
        责任客服未通过 = 8
    }
    #endregion

    #region 四舍五入
    /// <summary>
    /// 企业基数是否四舍五入
    /// </summary>
    public enum IS_Four_Five
    {
        四舍五入 = 1,
        不进位 = 2,
        进位 = 3,

    }

    public enum IS_BearMonth
    {
        先乘以月 = 1,
        后乘以月 = 0,

    }
    #endregion
}