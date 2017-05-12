using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL.Model
{
    public class SupplierInfo
    {
        public string NatureCityId { get; set; }
        public Supplier BasicInfo { get; set; }
        public string LinkMan { get; set; }
        public string Bank { get; set; }
        public string Bill { get; set; }
        public string Price { get; set; }
        public string LadderPrice { get; set; }
    }
}
