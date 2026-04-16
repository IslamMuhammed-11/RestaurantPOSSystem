using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.DTOs.TableDTOs;
using DataAccessLayer.Entites;

namespace BusinessLogicLayer.Mapping
{
    public class TableMap
    {
        public static ReadTableDTO ToReadDTO(TableEntity entity)
        {
            return new ReadTableDTO
            {
                TableID = entity.TableID,
                TableStatus = entity.TableStatus,
                NumberOfSeats = entity.NumberOfSeats
            };
        }

        public static TableEntity ToEntity(CreateTableDTO dto)
        {
            return new TableEntity
            {
                NumberOfSeats = dto.NumberOfSeats,
                TableStatus = 0
            };
        }

        public static bool ToEntity(UpdateTableDTO dto, TableEntity table)
        {
            if (dto == null)
                return false;
            if (dto.TableStatus != null)
                table.TableStatus = dto.TableStatus.Value;
            if (dto.NumberOfSeats != null)
                table.NumberOfSeats = dto.NumberOfSeats.Value;
            return true;
        }

        public static List<ReadTableDTO> ToReadDTOList(List<TableEntity> entities)
        {
            if (entities == null || entities.Count == 0)
                return new List<ReadTableDTO>();
            return entities.Select(e => ToReadDTO(e)).ToList();
        }
    }
}
