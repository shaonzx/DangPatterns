using DangPatterns.DesignPatterns.Factory;
using DangPatterns.DesignPatterns.Observer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DangPatterns.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObserverController : ControllerBase
    {
        private readonly ILogger<ObserverController> _logger;

        public ObserverController(ILogger<ObserverController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Observe-INCOMPLETE")]
        public async Task<IActionResult> Observe()
        {
            var eventNotifier = new EventNotifier("System Monitor");

            /*var emailLogger = new EmailLoggerObserver("admin@company.com", _logger);
            var fileLogger = new FileLoggerObserver("system_events.log");
            eventNotifier.Attach(emailLogger);
            eventNotifier.Attach(fileLogger);*/

            await eventNotifier.DoSomethingHeavyAsync();

            return Ok("Observation done.");
        }
    }
}
