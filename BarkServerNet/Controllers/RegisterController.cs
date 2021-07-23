using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BarkServerNet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        readonly ILogger<PushController> _logger;
        readonly IDeviceServer _server;

        public RegisterController(ILogger<PushController> logger, IDeviceServer server)
        {
            _logger = logger;
            _server = server;
        }


        [HttpGet]
        public async Task<CommonResponse> Get(string deviceToken)
        {
            CommonResponse response = new()
            {
                Code = StatusCodes.Status200OK,
                Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
            };

            if (string.IsNullOrWhiteSpace(deviceToken))
            {
                response.Message = $"{nameof(deviceToken)} is empty";
            }

            try
            {
                var deviceKey = await _server.RegisterDevice(deviceToken);

                response.Message = "success";
                response.Device = new()
                {
                    Key = deviceKey,
                    DeviceToken = deviceToken
                };
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                _logger.LogError(ex, nameof(RegisterController));
            }

            return response;
        }
    }
}
