using Contracts.Enums;

namespace Contracts.DTOs.TableDTOs
{
    public class TableResponse
    {
        public int TableID { get; set; }

        public TableStatusEnum.enTableStatus Status { get; set; }

        public int Seats { get; set; }
    }
}