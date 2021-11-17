using Microsoft.AspNetCore.Mvc;

namespace BarkServerNet.Controllers;

[ApiController]
[Route("[controller]")]
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

        if (string.IsNullOrWhiteSpace(deviceToken))
        {
            resp.Message = $"{nameof(deviceToken)} is empty";
            resp.Code = StatusCodes.Status400BadRequest;
            return resp;
        }

        try
        {
            var deviceKey = await server.RegisterDeviceAsync(key, deviceToken);

            resp.Message = "success";
            resp.DeviceInfo = new() { Key = deviceKey, DeviceKey = deviceKey, DeviceToken = deviceToken };
        }
        catch (Exception ex)
        {
            resp.Message = "registe exception";
            resp.Code = StatusCodes.Status500InternalServerError;

            logger.LogError(ex, "registe exception");
        }
        return resp;
    }
}
