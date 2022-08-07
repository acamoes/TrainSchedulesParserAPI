using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainSchedulesParserAPI
{
    internal class ScheduleRequest
    {
        public string? Station { get; set; }
		public DateTime RequestTime { get; set; }

        public ScheduleRequest()
        {
            // Docker container runs with UTC Now. There is a need to add +1
            RequestTime = DateTime.UtcNow.AddHours(1);
        }
    }
}
