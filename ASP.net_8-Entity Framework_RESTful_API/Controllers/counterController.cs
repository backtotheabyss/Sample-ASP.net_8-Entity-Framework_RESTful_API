using Microsoft.AspNetCore.Mvc;

namespace ASP.net_8_Entity_Framework_RESTful_API
{
    [ApiController]
    [Route("api/counter")]
    public class counterController : ControllerBase
    {
        private readonly Increment _incrementService;

        public counterController(Increment incrementService)
        {
            _incrementService = incrementService;
            _ = InitializeIncrementServiceAsync();
        }

        private async Task InitializeIncrementServiceAsync()
        {
            await _incrementService.StartIncrementing();
        }

        [HttpGet]
        [Route("retrieveCounter")]
        public ActionResult<long> RetrieveCounter()
        {
            //return Ok(new { Message = "Counter running - ", Number = _incrementService.GetCurrentNumber() });

            //deprecatad
            //Calculate the difference between the current timestamp and the last activity timestamp
            var timeDifference = (decimal)(DateTime.UtcNow - _incrementService.lastActivityTimestamp).TotalSeconds;

            // Check if the difference exceeds the threshold (e.g., 5 seconds)
            if (timeDifference >= 1)
            {
                // Get the last activity timestamp from the increment service
                _incrementService.lastActivityTimestamp = DateTime.UtcNow;

                // If the user has been inactive for more than 5 seconds, stop incrementing
                _incrementService.StopIncrementing();
                _incrementService.isStopped = false;
                return Ok(new { Message = "Counter stopped", Number = _incrementService.GetCurrentNumber() });
            }

            // Get the last activity timestamp from the increment service
            _incrementService.lastActivityTimestamp = DateTime.UtcNow;
            if (_incrementService.isStopped)
            {
                _incrementService.isStopped = false;
                return Ok(new { Message = "Counter stopped external", Number = _incrementService.GetCurrentNumber() });
            }                
            else
                return Ok(new { Message = "Counter running - " + timeDifference.ToString(), Number = _incrementService.GetCurrentNumber() });
        }

        [HttpPost("stopCounter")]
        public ActionResult<string> StopCounter()
        {
            _incrementService.StopIncrementing();
            return Ok(new { Message = "Counter stopped", Number = _incrementService.GetCurrentNumber() });
        }

        [HttpPost("stopCounterExternal")]
        public ActionResult<string> StopCounterExternal()
        {
            _incrementService.StopIncrementingExternal();
            return Ok(new { Message = "Counter stopped external", Number = _incrementService.GetCurrentNumber() });
        }

        [HttpPost("resetCounter")]
        public ActionResult<string> ResetCounter()
        {
            _incrementService.ResetIncrementing();
            return Ok(new { Message = "Counter set to zero", Number = 0 });
        }
    }
}