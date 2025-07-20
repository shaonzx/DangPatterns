using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DangPatterns.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WelcomeController : ControllerBase
    {

        private readonly ILogger<WelcomeController> _logger;

        public WelcomeController(ILogger<WelcomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            _logger.LogInformation("Welcome executed with success!");
            _logger.LogCritical("Sample Critical issue");
            _logger.LogError("Sample Error!");

            return "This is the thing you are looking for";
        }
    }
}
