using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.TableDTOs
{
    public class ReadTableDTO
    {
        public required int TableID { get; set; }
        public required int TableStatus { get; set; }
        public required short NumberOfSeats { get; set; }
    }
}
