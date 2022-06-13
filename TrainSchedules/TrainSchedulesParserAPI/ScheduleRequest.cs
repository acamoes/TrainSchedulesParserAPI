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
            RequestTime = DateTime.Now;
        }
    }
}
