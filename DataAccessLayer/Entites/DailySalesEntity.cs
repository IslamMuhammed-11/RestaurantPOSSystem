using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entites
{
    public class DailySalesEntity
    {
        public DateOnly Date { get; set; }
        public int TotalOrders { get; set; }
        public decimal Gross { get; set; }
        public decimal Net { get; set; }
        public decimal TotalRefund { get; set; }
    }
}