using Contracts.Enums;
using Contracts.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Queries.ReportsQueries
{
    public class RangedQuery
    {
        public ReportsFilterRange.enRange range { get; set; }

        public PeriodicQuery? Periodic { get; set; } = null;

        public bool Validate()
        {
            if (range == ReportsFilterRange.enRange.Custom)
            {
                if (Periodic == null || Periodic.from > Periodic.to)
                    return false;
            }
            return true;
        }

        public PeriodicQuery ResolvePeriod()
        {
            return range switch
            {
                ReportsFilterRange.enRange.Today => new PeriodicQuery
                {
                    from = DateOnly.FromDateTime(DateTime.Now),
                    to = DateOnly.FromDateTime(DateTime.Now)
                },
                ReportsFilterRange.enRange.ThisMonth => new PeriodicQuery
                {
                    from = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
                    to = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
                },
                ReportsFilterRange.enRange.Custom => Periodic!,
                _ => throw new BusinessException("Invalid range value. Please (Today , ThisMonth , Custom) Enums", 9858, ActionResultEnum.ActionResult.InvalidData)
            };
        }
    }
}