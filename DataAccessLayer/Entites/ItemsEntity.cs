using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entites
{
    public class ItemsEntity
    {
        public int ItemID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int OrderID { get; set; }
        public short Quantity { get; set; }
        public decimal Price { get; set; }

    }
}
