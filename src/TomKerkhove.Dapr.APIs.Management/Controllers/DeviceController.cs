using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Filters;
using TomKerkhove.Dapr.Actors.Device.Interface.Contracts;
using TomKerkhove.Dapr.APIs.Management.Repositories;

namespace TomKerkhove.Dapr.APIs.Management.Controllers
{
    [ApiController]
    [Route("api/v1/devices")]
    public class DeviceController : ControllerBase
    {
        private readonly DeviceRepository _deviceRepository;
        private readonly ILogger<DeviceController> _logger;

        public DeviceController(DeviceRepository deviceRepository, ILogger<DeviceController> logger)
        {
            _logger = logger;
            _deviceRepository = deviceRepository;
        }

        /// <summary>
        ///     Get Device Info
        /// </summary>
        /// <remarks>Provides capability to get latest information for a given device.</remarks>
        /// <param name="deviceId">Unique id for a given device</param>
        /// <response code="200">Device information is provided</response>
        /// <response code="503">We are undergoing some issues</response>
        [HttpGet("{deviceId}/info", Name = "Device_GetInfo")]
        [ProducesResponseType(typeof(DeviceInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        [SwaggerResponseHeader(200, "RequestId", "string", "The header that has a request ID that uniquely identifies this operation call")]
        [SwaggerResponseHeader(200, "X-Transaction-Id", "string", "The header that has the transaction ID is used to correlate multiple operation calls.")]
        public async Task<IActionResult> GetInfo([FromRoute] string deviceId)
        {
            var data = await _deviceRepository.GetData(deviceId);
            return Ok(data);
        }

        /// <summary>
        ///     Set Device Info
        /// </summary>
        /// <remarks>Provides capability to update information for a given device.</remarks>
        /// <param name="deviceId">Unique id for a given device</param>
        /// <response code="200">Device information is provided</response>
        /// <response code="503">We are undergoing some issues</response>
        [HttpPut("{deviceId}/info", Name = "Device_SetInfo")]
        [ProducesResponseType(typeof(DeviceInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        [SwaggerResponseHeader(200, "RequestId", "string", "The header that has a request ID that uniquely identifies this operation call")]
        [SwaggerResponseHeader(200, "X-Transaction-Id", "string", "The header that has the transaction ID is used to correlate multiple operation calls.")]
        public async Task<IActionResult> SetInfo([FromRoute] string deviceId, [FromBody] DeviceInfo newData)
        {
            await _deviceRepository.SetData(deviceId, newData);
            return Ok();
        }
    }
}