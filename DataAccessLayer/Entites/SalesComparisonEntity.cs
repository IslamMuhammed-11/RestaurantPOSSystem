using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entites
{
    public class SalesComparisonEntity
    {
        public DailySalesEntity Current { get; set; } = new();
        public DailySalesEntity Prev { get; set; } = new();
    }
}