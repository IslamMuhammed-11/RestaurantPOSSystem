using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.TableDTOs
{
    public class UpdateTableDTO
    {
        public int? TableStatus { get; set; }
        public short? NumberOfSeats { get; set; }
    }
}
