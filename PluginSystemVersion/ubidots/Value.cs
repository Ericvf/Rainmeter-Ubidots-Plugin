using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubidots
{
    public class Value : ApiObject
    {
        public class StatisticFigures
        {
            public const string MEAN = "mean";
            public const string VARIANCE = "variance";
            public const string MIN = "min";
            public const string MAX = "max";
            public const string COUNT = "count";
            public const string SUM = "sum";
        }

        public Value(Dictionary<string, object> Raw, ApiClient Api) : base(Raw, Api) {}

        /// <summary>
        /// Gets the timestamp of the value.
        /// </summary>
        /// <returns>The timestamp of the value. Formatted in Unix Time</returns>
        public long GetTimestamp()
        {
            return (long)GetAttribute("timestamp");
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        // <returns>The value</returns>
        public double GetValue() 
        {
            return GetAttributeDouble("value");
        }
    }
}
