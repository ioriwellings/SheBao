using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL.Model
{
    public class EmployeeBanks
    {
        public object Id { get; set; }
        public object AccountName { get; set; }

        public object Bank { get; set; }

        public object BranchBank { get; set; }

        public object Account { get; set; }

        public object IsDefault { get; set; }

        public int? EmployeeId { get; set; }

        public object Remark { get; set; }

        public object BState { get; set; }

        public DateTime? CreateTime { get; set; }

        public object CreatePerson { get; set; }

        public DateTime? UpdateTime { get; set; }

        public object UpdatePerson { get; set; }
    }
}
