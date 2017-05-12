using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL.Model
{
    public class CostPayFenGe
    {
        public int? CID { get; set; }
        public int P_ID { get; set; }
        public string C_NAME { get; set; }
        public int QIJIAN { get; set; }
        public string YANGLAO_CARD_ID { get; set; }
        public string SB_CARD_ID { get; set; }
        public Decimal CHARGE_P { get; set; }
        public Decimal CHARGE_C { get; set; }
        public Decimal H_Sum { get; set; }
        public Decimal 工本费 { get; set; }
        public int? CW_CID { get; set; }
        public string CANBAODI { get; set; }
    }
}
