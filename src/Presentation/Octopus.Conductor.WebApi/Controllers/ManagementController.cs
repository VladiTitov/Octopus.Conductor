using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Octopus.Conductor.Infrastructure.WorkerService.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Conductor.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ManagementController : ControllerBase
    {
        private readonly ILogger<ManagementController> _logger;
        private readonly IExtendedHostedService _hostedService;

        public ManagementController(
            ILogger<ManagementController> logger,
            IExtendedHostedService hostedService)
        {
            _logger = logger;
            _hostedService = hostedService;
        }

        [HttpGet]
        [Route("Start")]
        public async Task<IActionResult> StartHostedService()
        {
            await _hostedService.StartAsync(new CancellationToken());
            return Ok();
        }

        [HttpGet]
        [Route("Stop")]
        public async Task<IActionResult> StopHostedService()
        {
            await _hostedService.StopAsync(new CancellationToken());
            return Ok();
        }
    }
}
