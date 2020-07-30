using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TomKerkhove.Dapr.Actors.Device.Interface;
using TomKerkhove.Dapr.APIs.Management.Repositories;

namespace TomKerkhove.Dapr.APIs.Management.Controllers
{
    [ApiController]
    [Route("api/v1/devices")]
    public class DeviceController : ControllerBase
    {
        private readonly ILogger<DeviceController> _logger;
        private readonly DeviceRepository _deviceRepository;

        public DeviceController(DeviceRepository deviceRepository, ILogger<DeviceController> logger)
        {
            _logger = logger;
            _deviceRepository = deviceRepository;
        }

        [HttpGet("{deviceId}/data")]
        public async Task<IActionResult> Get([FromRoute] string deviceId)
        {
            var data = await _deviceRepository.GetData(deviceId);
            return Ok(data);
        }

        [HttpPut("{deviceId}/data")]
        public async Task<IActionResult> Get([FromRoute] string deviceId, [FromBody]DeviceState newData)
        {
            await _deviceRepository.SetData(deviceId, newData);
            return Ok();
        }
    }
}
