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

namespace BusinessLogicLayer.Services
{
    public class TableService : ITableService
    {
        private readonly ITableRepo _tableRepo;

        public TableService(ITableRepo tableRepo)
        {
            _tableRepo = tableRepo;
        }

        public async Task<int?> AddNewTableAsync(CreateTableRequest table)
        {
            if (table == null || !table.IsValid())
                throw new BusinessException("Invalid table data.", 80000, ActionResultEnum.ActionResult.InvalidData);

            var entity = TableMap.ToEntity(table);
            int? id = await _tableRepo.CreateTableAsync(entity);
            if (!id.HasValue)
                throw new BusinessException("Failed to create table.", 80002, ActionResultEnum.ActionResult.DBError);

            return id;
        }

        public async Task<TableResponse?> GetTableByIDAsync(int id)
        {
            if (id < 0)
                throw new BusinessException("Invalid table ID.", 80000, ActionResultEnum.ActionResult.InvalidData);

            var entity = await _tableRepo.GetTableByIdAsync(id);
            if (entity == null)
                throw new BusinessException("Table not found.", 80001, ActionResultEnum.ActionResult.NotFound);

            return TableMap.ToReadDTO(entity);
        }

        public async Task<List<TableResponse>> GetAllTablesAsync()
        {
            var tables = await _tableRepo.GetAllTablesAsync();
            return TableMap.ToReadDTOList(tables);
        }

        public async Task<bool> UpdateTableAsync(int ID, UpdateTableRequest table)
        {
            if (table == null || ID < 0)
                throw new BusinessException("Invalid table data.", 80000, ActionResultEnum.ActionResult.InvalidData);

            var existing = await _tableRepo.GetTableByIdAsync(ID);
            if (existing == null)
                throw new BusinessException("Table not found.", 80001, ActionResultEnum.ActionResult.NotFound);

            bool ok = TableMap.ToEntity(table, existing);
            if (!ok)
                throw new BusinessException("Invalid table data.", 80000, ActionResultEnum.ActionResult.InvalidData);

            bool updated = await _tableRepo.UpdateTableAsync(existing);
            if (!updated)
                throw new BusinessException("Failed to update table.", 80002, ActionResultEnum.ActionResult.DBError);

            return true;
        }

        public async Task<bool> DeleteTableByIDAsync(int id)
        {
            if (id < 0)
                throw new BusinessException("Invalid table ID.", 80000, ActionResultEnum.ActionResult.InvalidData);

            var existing = await _tableRepo.GetTableByIdAsync(id);
            if (existing == null)
                throw new BusinessException("Table not found.", 80001, ActionResultEnum.ActionResult.NotFound);

            bool deleted = await _tableRepo.DeleteTableAsync(id);
            if (!deleted)
                throw new BusinessException("Failed to delete table.", 80002, ActionResultEnum.ActionResult.DBError);

            return true;
        }
    }
}