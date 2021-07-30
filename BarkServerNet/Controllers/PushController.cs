using System;
using DotAPNS.Extensions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BarkServerNet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PushController : ControllerBase
    {
        [HttpGet("/{deviceKey}/{title}/{body?}")]
        public async Task<CommonResponse> Get([FromServices] ILogger<PushController> logger, [FromServices] IDeviceServer server, [FromServices] IApnsService apnsService,
            string deviceKey, string title, string? body, [FromQuery] Message message)
        {
            message.DeviceKey = deviceKey;
            message.Title = title;
            message.Body = body;

            return await Push(logger, server, apnsService, message);
        }

        [HttpGet("/{deviceKey}")]
        public async Task<CommonResponse> Get([FromServices] ILogger<PushController> logger, [FromServices] IDeviceServer server, [FromServices] IApnsService apnsService,
            string deviceKey, [FromQuery] Message message)
        {
            message.DeviceKey = deviceKey;

            return await Push(logger, server, apnsService, message);
        }

        [HttpPost]
        public async Task<CommonResponse> Post([FromServices] ILogger<PushController> logger, [FromServices] IDeviceServer server,
            [FromServices] IApnsService apnsService, Message message)
        {
            return await Push(logger, server, apnsService, message);
        }

        async Task<CommonResponse> Push(ILogger<PushController> logger, IDeviceServer server, IApnsService apnsService, Message message)
        {
            Device? device = default;
            CommonResponse resp = new()
            {
                Code = StatusCodes.Status200OK,
                Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
            };

            try
            {
                device = server.GetDevice(message.DeviceKey);
            }
            catch (Exception ex)
            {
                resp.Message = ex.Message;
                logger.LogError(ex, nameof(PushController));
            }
            finally
            {
                if (device != null && !string.IsNullOrWhiteSpace(device.DeviceToken))
                {
                    var apns = apnsService.CreateUsingJwt();
                    var result = await apns.SendAsync(message.CreatePush(device.DeviceToken));

                    resp.Message = result.IsSuccessful ? "success" : result.ReasonString;
                }
                else
                {
                    resp.Message = "DeviceToken is empty";
                }
            }

            return resp;
        }
    }
}
