using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Queries.ReportsQueries
{
    public class PeriodicQuery
    {
        public DateOnly from { get; set; }
        public DateOnly to { get; set; }
    }
}