using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL
{
    public partial class PoliceInsuranceRepository : BaseRepository<PoliceInsurance>, IDisposable
    {
        #region 报增缴纳保险政策联动判断
        public string POSTPoliceCascadeRelationship(SysEntities db, string Cityid)
        {
            string guan = "";
            var s = (from a in db.InsuranceKind where a.City == Cityid select a).ToList();
            List<string> aa = new List<string>();
            foreach (var a in s)
            {
                var PoliceCascadeRelationshiplist = db.PoliceCascadeRelationship.FirstOrDefault(o => o.InsuranceKindId == a.Id);
                if (PoliceCascadeRelationshiplist != null)
                {
                    aa.Add(PoliceCascadeRelationshiplist.Tag);
                }
            }
            var e = (from a in db.PoliceCascadeRelationship where aa.Contains(a.Tag) select a).ToList();

            List<PoliceCascadeRelationship> distinctPeople = e.GroupBy(p => p.InsuranceKindId).Select(g => g.First()).ToList();

            foreach (var u in distinctPeople)
            {
                guan = u.InsuranceKindId + "," + guan;
            }
            String[] guanlian = guan.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string tishi = "";
            foreach (var sd in guanlian)
            {
                int sad = Convert.ToInt32(sd);
                var InsuranceKindnew = db.InsuranceKind.FirstOrDefault(o => o.Id == sad);
                if (InsuranceKindnew != null)
                {
                    tishi = InsuranceKindnew.Name + "," + tishi;
                }
            }
            return tishi;

        }
        #endregion
    }
}
