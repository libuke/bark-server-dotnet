using DotAPNS.Extensions;
using BarkServerNet.Apns;
using Microsoft.AspNetCore.Mvc;

namespace BarkServerNet.Controllers;

[ApiController]
[Route("[controller]")]
public class PushController : ControllerBase
{
    readonly ILogger<PushController> _logger;
    readonly IDeviceServer _server;
    readonly IApnsService _apnsService;

    public PushController(ILogger<PushController> logger, [FromServices] IDeviceServer server, [FromServices] IApnsService apnsService)
    {
        _logger = logger;
        _server = server;
        _apnsService = apnsService;
    }

    [HttpGet("/{deviceKey}/{title}/{body?}")]
    public async Task<CommonResponse> Get(string deviceKey, string? title, string? body, [FromQuery] Message message)
    {
        message.Title = title;
        message.Body = body;

        return await PushAppleAsync(deviceKey, message);
    }

    [HttpGet("/{deviceKey}")]
    public async Task<CommonResponse> Get(string deviceKey, [FromQuery] Message message)
    {
        return await PushAppleAsync(deviceKey, message);
    }

    [HttpPost]
    public async Task<CommonResponse> Post(string deviceKey, Message message)
    {
        return await PushAppleAsync(deviceKey, message);
    }

    #region Helper Nethod 
    async Task<CommonResponse> PushAppleAsync(string deviceKey, Message message)
    {
        CommonResponse resp = new()
        {
            Code = StatusCodes.Status200OK,
            Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
        };

        try
        {
            var deviceToken = _server.GetDeviceToken(deviceKey);

            if (deviceToken == null)
            {
                resp.Message = $"{nameof(deviceToken)}does not exist";
                return resp;
            }
            var clien = _apnsService.CreateUsingJwt();
            var result = await clien.PushAlertAsync(deviceToken, message);
            resp.Message = result.ReasonString;
        }
        catch (Exception ex)
        {
            resp.Message = "push exception";
            _logger.LogError(ex, "push exception");
        }
        return resp;
    }

    #endregion
}

