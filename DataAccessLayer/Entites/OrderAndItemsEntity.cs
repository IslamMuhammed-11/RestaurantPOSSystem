using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entites
{
    public class OrderAndItemsEntity
    {

        public OrderEntity Order { get; set; } = new OrderEntity();

        public List<ItemsEntity> Items { get; set; } = new List<ItemsEntity>();

    }
}
