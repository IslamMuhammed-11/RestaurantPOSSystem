using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entites;
namespace DataAccessLayer.Interfaces
{
    public interface ITableRepo
    {
        Task<List<TableEntity>> GetAllTablesAsync();
        Task<TableEntity> GetTableByIdAsync(int tableId);
        Task<int?> CreateTableAsync(TableEntity table);
        Task<bool> UpdateTableAsync(TableEntity table);
        Task<bool> DeleteTableAsync(int tableId);
    }
}
