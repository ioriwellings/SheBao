using Langben.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL.SCHR
{
    public partial class CRM_Company_SheBaoRepository
    {
        /// <summary>
        /// 根据企业名称查询企业（销售用）
        /// </summary>
        /// <param name="InsuranceKindName">社保种类名称</param>
        /// <param name="city">缴纳地</param>
        /// <returns></returns>
        public IQueryable<CRM_CompanySheBaoInfo> GetCompanyListForSales(SysEntities db, string InsuranceKindName, int? city)
        {
            var query = from a in db.CRM_Company_PoliceInsurance
                        join b in db.City on a.City equals b.Id
                        join d in db.PoliceInsurance on a.PoliceInsurance equals d.Id
                        join c in db.InsuranceKind on a.InsuranceKindID equals c.Id into tmpxs
                        from xs in tmpxs.DefaultIfEmpty()
                        //where xs.Name.Contains(InsuranceKindName) &&
                        //     (city == null || b.UserID_XS == userID_XS)
                        select new CRM_CompanySheBaoInfo()
                        {
                            CityID = b.Id,
                            CityName=b.Name,
                            InsuranceKindID=xs.Id,
                            InsuranceKindName = xs.Name,
                            PoliceInsurance=d.Name,

                            //CompanyCode = a.CompanyCode,
                            //CompanyName = a.CompanyName,
                            //UserID_XS = b.UserID_XS,
                            //OperateStatus = a.OperateStatus,
                            //CreateTime = a.CreateTime,
                            //UserID_ZR = b.UserID_ZR,
                            //UserID_XS_Name = xs.RName,
                            //UserID_ZR_Name = zr.RName
                        };      
          
            return query.OrderBy(e => e.CityID);
        }
    }
}
