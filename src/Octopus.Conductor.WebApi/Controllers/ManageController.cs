using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Conductor.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ManageController : ControllerBase
    {
        private readonly ILogger<ManageController> _logger;
        private readonly IHostedService _hostedService;

        public ManageController(
            ILogger<ManageController> logger, 
            IHostedService hostedService)
        {
            _logger = logger;
            _hostedService = hostedService;
        }

        [HttpGet]
        [Route("Start")]
        public async Task<IActionResult> StartHostedService()
        {
            await _hostedService.StartAsync(new System.Threading.CancellationToken());
            return Ok();
        }

        [HttpGet]
        [Route("Stop")]
        public async Task<IActionResult> StopHostedService()
        {
            await _hostedService.StopAsync(new System.Threading.CancellationToken());
            return Ok();
        }
    }
}
