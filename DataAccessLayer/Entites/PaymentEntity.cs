using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entites
{
    public class PaymentEntity
    {

        public int PaymentID { get; set; }

        public int OrderID { get; set; }

        public int PaymentMethodID { get; set; }

        public DateTime PaymentDate { get; set; }

        public decimal PaidAmount { get; set; }
    }
}
