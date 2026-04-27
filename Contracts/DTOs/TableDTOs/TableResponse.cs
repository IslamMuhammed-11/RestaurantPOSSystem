namespace Contracts.DTOs.TableDTOs
{
    public class TableResponse
    {
        public int TableID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Seats { get; set; }
    }
}
