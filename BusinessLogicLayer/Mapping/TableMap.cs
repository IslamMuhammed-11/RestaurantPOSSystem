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
        public static TableResponse ToReadDTO(TableEntity entity)
        {
            return new TableResponse
            {
                TableID = entity.TableID,
                // Name = $"Table {entity.TableID}",
                Seats = entity.NumberOfSeats
            };
        }

        public static TableEntity ToEntity(CreateTableRequest dto)
        {
            return new TableEntity
            {
                NumberOfSeats = (short)dto.Seats,
                TableStatus = 0
            };
        }

        public static bool ToEntity(UpdateTableRequest dto, TableEntity table)
        {
            if (dto == null)
                return false;
            if (dto.Seats != null)
                table.NumberOfSeats = (short)dto.Seats.Value;
            return true;
        }

        public static List<TableResponse> ToReadDTOList(List<TableEntity> entities)
        {
            if (entities == null || entities.Count == 0)
                return new List<TableResponse>();
            return entities.Select(e => ToReadDTO(e)).ToList();
        }
    }
}