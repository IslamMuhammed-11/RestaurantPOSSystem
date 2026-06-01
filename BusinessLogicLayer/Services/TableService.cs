using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Mapping;
using Contracts.DTOs.TableDTOs;
using Contracts.Enums;
using Contracts.Exceptions;
using DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.ErrorHandling;

namespace BusinessLogicLayer.Services
{
    public class TableService : ITableService
    {
        private readonly ITableRepo _tableRepo;

        public TableService(ITableRepo tableRepo)
        {
            _tableRepo = tableRepo;
        }

        public async Task<Result<TableResponse>> AddNewTableAsync(CreateTableRequest table)
        {
            if (table == null || !table.IsValid())
                return Result<TableResponse>.Failure(new Error("Invalid table data.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var entity = TableMap.ToEntity(table);
            int? id = await _tableRepo.CreateTableAsync(entity);

            if (!id.HasValue)
                return Result<TableResponse>.Failure(new Error("Failed to create table.", ErrorCodes.enErrorCodes.DB_ERROR));

            var response = new TableResponse
            {
                TableID = id.Value,
                Status = TableStatusEnum.enTableStatus.Available,
                Seats = entity.NumberOfSeats
            };

            return Result<TableResponse>.Success(response);
        }

        public async Task<Result<TableResponse>> GetTableByIDAsync(int id)
        {
            if (id < 0)
                return Result<TableResponse>.Failure(new Error("Invalid table ID.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var entity = await _tableRepo.GetTableByIdAsync(id);
            if (entity == null)
                return Result<TableResponse>.Failure(new Error("Table not found.", ErrorCodes.enErrorCodes.NOT_FOUND));

            return Result<TableResponse>.Success(TableMap.ToReadDTO(entity));
        }

        public async Task<Result<List<TableResponse>>> GetAllTablesAsync()
        {
            var tables = await _tableRepo.GetAllTablesAsync();
            return Result<List<TableResponse>>.Success(TableMap.ToReadDTOList(tables));
        }

        public async Task<Result<bool>> UpdateTableAsync(int ID, UpdateTableRequest table)
        {
            if (table == null || ID < 0)
                return Result<bool>.Failure(new Error("Invalid table data.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var existing = await _tableRepo.GetTableByIdAsync(ID);
            if (existing == null)
                return Result<bool>.Failure(new Error("Table not found.", ErrorCodes.enErrorCodes.NOT_FOUND));

            bool ok = TableMap.ToEntity(table, existing);
            if (!ok)
                return Result<bool>.Failure(new Error("Invalid table data.", ErrorCodes.enErrorCodes.INVALID_DATA));

            bool updated = await _tableRepo.UpdateTableAsync(existing);

            if (!updated)
                return Result<bool>.Failure(new Error("Failed to update table.", ErrorCodes.enErrorCodes.DB_ERROR));

            return Result<bool>.Success(updated);
        }

        public async Task<Result<bool>> DeleteTableByIDAsync(int id)
        {
            if (id < 0)
                return Result<bool>.Failure(new Error("Invalid table ID.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var existing = await _tableRepo.GetTableByIdAsync(id);
            if (existing == null)
                return Result<bool>.Failure(new Error("Table not found.", ErrorCodes.enErrorCodes.NOT_FOUND));

            bool deleted = await _tableRepo.DeleteTableAsync(id);

            return Result<bool>.Success(deleted);
        }
    }
}