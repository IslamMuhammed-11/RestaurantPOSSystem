namespace Contracts.DTOs.TableDTOs
{
    public class CreateTableRequest
    {
        private int _ID;
        public int TableID => _ID;
        public required string Name { get; set; }
        public int Seats { get; set; }
        public void SetID(int id) => _ID = id;
        public bool IsValid() => !string.IsNullOrEmpty(Name) && Seats > 0;
    }
}
