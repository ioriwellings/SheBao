using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Langben.DAL.Model
{
    public class EmployeeContacts
    {
        public object Id { get; set; }

        public object Telephone { get; set; }

        public object MobilePhone { get; set; }

        public object Email { get; set; }

        public object Address { get; set; }

        public int? EmployeeId { get; set; }

        public object Remark { get; set; }

        public object CState { get; set; }

        public DateTime? CreateTime { get; set; }

        public object CreatePerson { get; set; }

        public DateTime? UpdateTime { get; set; }

        public object UpdatePerson { get; set; }
    }
}
