using Contracts.Enums;

namespace DataAccessLayer.Entites
{
    public class TableEntity
    {
        public int TableID { get; set; }
        public int TableStatus { get; set; }
        public short NumberOfSeats { get; set; }
    }
}