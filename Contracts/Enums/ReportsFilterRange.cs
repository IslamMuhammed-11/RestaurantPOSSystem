using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Contracts.Enums
{
    public record ReportsFilterRange
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum enRange
        {
            [EnumMember(Value = "today")]
            Today,
            [EnumMember(Value = "thismonth")]
            ThisMonth,
            [EnumMember(Value = "custom")]
            Custom
        };
    }
}