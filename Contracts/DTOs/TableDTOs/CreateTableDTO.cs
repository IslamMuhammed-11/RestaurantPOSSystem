using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.TableDTOs
{
    public class CreateTableDTO
    {
        private int _ID;
        public int TableID { get { return _ID; } }

        public required short NumberOfSeats { get; set; }

        public void SetID(int id) => _ID = id;

        public bool IsValid() => NumberOfSeats > 0;
    }
}
