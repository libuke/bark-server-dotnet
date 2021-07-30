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
        [HttpGet]
        public async Task<CommonResponse> Get([FromServices] ILogger<RegisterController> logger, [FromServices] IDeviceServer server, string deviceToken, string? key)
        {
            Device? device = default;
            CommonResponse response = new()
            {
                Code = StatusCodes.Status200OK,
                Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
            };

            try
            {
                device = string.IsNullOrWhiteSpace(key) ? await server.RegisterDevice(deviceToken) : server.GetDevice(key);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                logger.LogError(ex, "Registe fail");
            }
            finally
            {
                if (device != null && device.DeviceToken == deviceToken)
                {
                    response.DeviceInfo = new() { DeviceKey = device.DeviceKey, DeviceToken = device.DeviceToken };
                    response.Message = "success";
                }
                else
                {
                    response.Message = "fail";
                }
            }
            return response;
        }
    }
}
