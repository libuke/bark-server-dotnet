using Microsoft.AspNetCore.Mvc;

namespace BarkServerNet.Controllers;

[Route("[controller]")]
[ApiController]
public class RegisterController : ControllerBase
{
    [HttpGet]
    public async Task<CommonResponse> Get([FromServices] ILogger<RegisterController> logger, [FromServices] IDeviceServer server, string deviceToken, string? key)
    {
        CommonResponse resp = new()
        {
            Code = StatusCodes.Status200OK,
            Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
        };

        try
        {
            var deviceKey = string.IsNullOrWhiteSpace(key) ? await server.RegisterDeviceAsync(deviceToken) : server.GetDeviceToken(key);

            if (string.IsNullOrWhiteSpace(deviceKey))
            {
                resp.Message = $"{deviceKey} is empty";
                return resp;
            }
            resp.Message = "success";
            resp.DeviceInfo = new() { Key = deviceKey, DeviceKey = deviceKey, DeviceToken = deviceToken };
        }
        catch (Exception ex)
        {
            resp.Message = "registe exception";
            logger.LogError(ex, "registe exception");
        }
        return resp;
    }
}
