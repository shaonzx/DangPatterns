using DangPatterns.DesignPatterns.Factory;
using DangPatterns.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DangPatterns.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FactoryController : ControllerBase
    {

        [HttpGet("GetEmployee")]
        public async Task<IActionResult> GetEmployee(string employeeType)
        {
            /*
             * The factory class (EmployeeFactory) needs to violet the OCP.
             * Bottom line: Don't use a factory just because you can.
             * Use it when you have a real need for abstracted, configurable, or runtime-determined object creation.
             * Otherwise, new ClassName() is perfectly fine and often better.
             */
            var manager = EmployeeFactory.CreateEmployee(EmployeeType.Manager);
            var developer = EmployeeFactory.CreateEmployee(EmployeeType.Developer);


            return Ok(manager.GetRole() + "-" + developer.GetRole());
        }
    }
}
