using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TrainSchedulesParserAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ScheduleManagerController : ControllerBase
    {
        private readonly ILogger<ScheduleManagerController> _logger;

        public ScheduleManagerController(ILogger<ScheduleManagerController> logger)
        {
            _logger = logger;
        }

        // GET /GetNextTrain/{stationName}
        [HttpGet("{stationName}", Name = "GetNextTrain")]
        [ActionName("GetNextTrain")]
        public async Task<ActionResult> GetNextTrain(string stationName)
        {
            ScheduleRequest request = new();

            request.Station = stationName;

            ScheduleResponse response = ScheduleManager.Search(request);

            return response != null ? Ok(response) : NotFound();
        }
    }
}