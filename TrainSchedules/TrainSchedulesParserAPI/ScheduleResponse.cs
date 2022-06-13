using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainSchedulesParserAPI
{
    public class ScheduleResponse
    {
        public string? nexttrain { get; set; }
        public string? NextTrainStatus { get; set; }
        public List<string>? Next3Trains { get; set; }
        public List<string>? Next3TrainsFull { get; set; }
    }
}
