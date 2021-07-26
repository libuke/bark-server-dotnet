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
        public async Task<CommonResponse> Get(string deviceToken, string? key)
        {
            CommonResponse response = new()
            {
                Code = StatusCodes.Status200OK,
                Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
            };
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    var deviceKey = await _server.RegisterDevice(deviceToken);

                    response.Message = deviceKey == null ? "fail" : "success";
                    response.Device = new()
                    {
                        Key = deviceKey,
                        DeviceToken = deviceToken
                    };
                }
                else
                {
                    var device = _server.GetDevice(key);

                    if (device == null || device.DeviceToken != deviceToken)
                    {
                        response.Message = $"{key} not error";
                    }
                    else
                    {
                        response.Message = "success";
                        response.Device = new()
                        {
                            Key = key,
                            DeviceToken = device.DeviceToken
                        };
                    }
                }
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
