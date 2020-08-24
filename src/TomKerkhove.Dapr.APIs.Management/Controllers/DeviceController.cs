using System.Net;
using System.Threading.Tasks;
using GuardNet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using TomKerkhove.Dapr.APIs.Management.Repositories;
using TomKerkhove.Dapr.Core.Contracts;

namespace TomKerkhove.Dapr.APIs.Management.Controllers
{
    [ApiController]
    [Route("api/v1/devices")]
    public class DeviceController : ControllerBase
    {
        private readonly DeviceRegistryRepository _deviceRegistryRepository;
        private readonly DeviceRepository _deviceRepository;

        public DeviceController(DeviceRegistryRepository deviceRegistryRepository, DeviceRepository deviceRepository)
        {
            Guard.NotNull(deviceRepository, nameof(deviceRepository));
            Guard.NotNull(deviceRepository, nameof(deviceRepository));

            _deviceRepository = deviceRepository;
            _deviceRegistryRepository = deviceRegistryRepository;
        }

        /// <summary>
        ///     Get Devices
        /// </summary>
        /// <remarks>Provides capability to get latest tags for a given device.</remarks>
        /// <response code="200">List of all devices is provided</response>
        /// <response code="503">We are undergoing some issues</response>
        [HttpGet(Name = "Device_GetAll")]
        [ProducesResponseType(typeof(DeviceInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        [SwaggerResponseHeader(new[] { (int)HttpStatusCode.OK, (int)HttpStatusCode.InternalServerError }, "RequestId", "string", "The header that has a request ID that uniquely identifies this operation call")]
        [SwaggerResponseHeader(new[] { (int)HttpStatusCode.OK, (int)HttpStatusCode.InternalServerError }, "X-Transaction-Id", "string", "The header that has the transaction ID is used to correlate multiple operation calls.")]
        public async Task<IActionResult> GetAll()
        {
            var devices = await _deviceRegistryRepository.GetAllAsync();
            return Ok(devices);
        }

        /// <summary>
        ///     Get Device Tags
        /// </summary>
        /// <remarks>Provides capability to get latest tags for a given device.</remarks>
        /// <param name="deviceId">Unique id for a given device</param>
        /// <response code="200">Tag information is provided</response>
        /// <response code="503">We are undergoing some issues</response>
        [HttpGet("{deviceId}/tags", Name = "Device_GetTags")]
        [ProducesResponseType(typeof(DeviceInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        [SwaggerResponseHeader(new[] { (int)HttpStatusCode.OK, (int)HttpStatusCode.InternalServerError }, "RequestId", "string", "The header that has a request ID that uniquely identifies this operation call")]
        [SwaggerResponseHeader(new[] { (int)HttpStatusCode.OK, (int)HttpStatusCode.InternalServerError }, "X-Transaction-Id", "string", "The header that has the transaction ID is used to correlate multiple operation calls.")]
        public async Task<IActionResult> GetTags([FromRoute] string deviceId)
        {
            var tags = await _deviceRepository.GetTagsAsync(deviceId);
            return Ok(tags);
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
        [SwaggerResponseHeader(new[] { (int)HttpStatusCode.OK, (int)HttpStatusCode.InternalServerError }, "RequestId", "string", "The header that has a request ID that uniquely identifies this operation call")]
        [SwaggerResponseHeader(new[] { (int)HttpStatusCode.OK, (int)HttpStatusCode.InternalServerError }, "X-Transaction-Id", "string", "The header that has the transaction ID is used to correlate multiple operation calls.")]
        public async Task<IActionResult> GetInfo([FromRoute] string deviceId)
        {
            var data = await _deviceRepository.GetDeviceInfoAsync(deviceId);
            return Ok(data);
        }

        /// <summary>
        ///     Set Device Info
        /// </summary>
        /// <remarks>Provides capability to update information for a given device.</remarks>
        /// <param name="deviceId">Unique id for a given device</param>
        /// <param name="newData">New information about the device</param>
        /// <response code="200">Device information is persisted</response>
        /// <response code="503">We are undergoing some issues</response>
        [HttpPut("{deviceId}/info", Name = "Device_SetInfo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        [SwaggerResponseHeader(new[] { (int)HttpStatusCode.OK, (int)HttpStatusCode.InternalServerError }, "RequestId", "string", "The header that has a request ID that uniquely identifies this operation call")]
        [SwaggerResponseHeader(new[] { (int)HttpStatusCode.OK, (int)HttpStatusCode.InternalServerError }, "X-Transaction-Id", "string", "The header that has the transaction ID is used to correlate multiple operation calls.")]
        public async Task<IActionResult> SetInfo([FromRoute] string deviceId, [FromBody] DeviceInfo newData)
        {
            await _deviceRepository.SetDeviceInfoAsync(deviceId, newData);
            return Ok();
        }

        /// <summary>
        ///     Provision Device
        /// </summary>
        /// <remarks>Provides capability to provision a new device.</remarks>
        /// <param name="deviceId">Unique id for a given device</param>
        /// <param name="provisionedDeviceInfo">Information about the new device</param>
        /// <response code="200">Device information is persisted</response>
        /// <response code="503">We are undergoing some issues</response>
        [HttpPut("{deviceId}/provision", Name = "Device_Provision")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        [SwaggerResponseHeader(new[] { (int)HttpStatusCode.OK, (int)HttpStatusCode.InternalServerError }, "RequestId", "string", "The header that has a request ID that uniquely identifies this operation call")]
        [SwaggerResponseHeader(new[] { (int)HttpStatusCode.OK, (int)HttpStatusCode.InternalServerError }, "X-Transaction-Id", "string", "The header that has the transaction ID is used to correlate multiple operation calls.")]
        public async Task<IActionResult> Provision([FromRoute] string deviceId, [FromBody] ProvisionedDeviceInfo provisionedDeviceInfo)
        {
            await _deviceRepository.ProvisionAsync(deviceId, provisionedDeviceInfo);
            await _deviceRegistryRepository.RegisterAsync(deviceId, provisionedDeviceInfo.DeviceInfo.IMEI);

            return Ok();
        }
    }
}