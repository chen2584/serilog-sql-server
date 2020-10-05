using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SerilogSqlServer.Controllers
{
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        readonly ILogger<TestController> _logger;
        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet("error")]
        public IActionResult ErrorLog()
        {
            // var zero = 0;
            // var test = 5 / zero; // there will insert to Exception field on SQL Server
            _logger.LogError("{RequestedBy}{Exception}{HostIpAddress}", "Chen Angelo", "No exception occurs", "192.168.58.36");
            return Ok("Logged!");
        }
    }
}