using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Enums;
using Contracts.DTOs.TableDTOs;
using Contracts.ErrorHandling;

namespace BusinessLogicLayer.Interfaces
{
    public interface ITableService
    {
        Task<Result<TableResponse>> AddNewTableAsync(CreateTableRequest table);

        Task<Result<TableResponse>> GetTableByIDAsync(int id);

        Task<Result<List<TableResponse>>> GetAllTablesAsync();

        Task<Result<bool>> UpdateTableAsync(int ID, UpdateTableRequest table);

        Task<Result<bool>> DeleteTableByIDAsync(int id);
    }
}