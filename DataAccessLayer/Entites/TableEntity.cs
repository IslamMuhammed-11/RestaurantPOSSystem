using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entites
{
    public class TableEntity
    {
        public int TableID { get; set; }
        public int TableStatus {  get; set; }
        public short NumberOfSeats { get; set; }

    }
}
