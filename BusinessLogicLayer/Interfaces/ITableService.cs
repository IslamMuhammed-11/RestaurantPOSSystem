using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Enums;
using Contracts.DTOs.TableDTOs;

namespace BusinessLogicLayer.Interfaces
{
    public interface ITableService
    {
        Task<int?> AddNewTableAsync(CreateTableDTO table);
        Task<ReadTableDTO?> GetTableByIDAsync(int id);
        Task<List<ReadTableDTO>> GetAllTablesAsync();
        Task<bool> UpdateTableAsync(int ID, UpdateTableDTO table);
        Task<bool> DeleteTableByIDAsync(int id);
    }
}
