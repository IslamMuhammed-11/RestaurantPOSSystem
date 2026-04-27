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
        Task<int?> AddNewTableAsync(CreateTableRequest table);

        Task<TableResponse?> GetTableByIDAsync(int id);

        Task<List<TableResponse>> GetAllTablesAsync();

        Task<bool> UpdateTableAsync(int ID, UpdateTableRequest table);

        Task<bool> DeleteTableByIDAsync(int id);
    }
}