using BarkServerNet.Apns;
using DotAPNS.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BarkServerNet.Controllers;

[ApiController]
[Route("[controller]")]
public class PushController : ControllerBase
{
    readonly ILogger<PushController> _logger;
    readonly IDeviceServer _server;
    readonly IApnsClientService _apnsService;
    readonly ApnsStrings _apnsStrings;

    public PushController(ILogger<PushController> logger, IDeviceServer server, IApnsClientService apnsService, IOptions<ApnsStrings> apnsStrings)
    {
        _logger = logger;
        _server = server;
        _apnsService = apnsService;
        _apnsStrings = apnsStrings.Value;
    }

    [HttpGet("/{deviceKey}/{title}/{body?}")]
    public async Task<CommonResponse> Get(string deviceKey, string title, string? body, [FromQuery] Message message)
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

    #region Helper Method 
    async Task<CommonResponse> PushAppleAsync(string deviceKey, Message message)
    {
        CommonResponse resp = new()
        {
            Code = StatusCodes.Status200OK,
            Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
        };

        try
        {
            var deviceToken = await _server.GetDeviceTokenAsync(deviceKey);

            if (deviceToken == null)
            {
                resp.Message = $"{nameof(deviceToken)} does not exist";
                resp.Code = StatusCodes.Status400BadRequest;
                return resp;
            }
            var clien = _apnsService.CreateUsingJwt();
            var result = await clien.PushAlertAsync(deviceToken, _apnsStrings.Topic, message);
            resp.Message = result.IsSuccessful ? "success" : result.ReasonString;
        }
        catch (Exception ex)
        {
            resp.Message = "push exception";
            resp.Code = StatusCodes.Status500InternalServerError;

            _logger.LogError(ex, "push exception");
        }
        return resp;
    }
    #endregion
}

