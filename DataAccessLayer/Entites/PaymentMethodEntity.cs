using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entites
{
    public class PaymentMethodEntity
    {
        public int MethodID { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

    }
}
