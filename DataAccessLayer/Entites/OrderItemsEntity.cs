using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entites
{
    public class OrderItemsEntity
    {
        public int ItemID { get; set; }
        public required int ProductID { get; set; }
        public required int OrderID { get; set; }
        public required int Quantity { get; set; }
        public required decimal Price { get; set; }

    }
}
