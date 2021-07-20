using System;
using DotAPNS;
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
        readonly IDeviceServer _server;
        readonly IApnsService _apnsService;
        readonly ILogger<PushController> _logger;


        public PushController(ILogger<PushController> logger, IDeviceServer server, IApnsService apnsService)
        {
            _logger = logger;
            _server = server;
            _apnsService = apnsService;
        }

        [HttpGet("/{deviceKey}/{title}/{body?}/{group?}")]
        public async Task<CommonResponse> SendGet([FromRoute] Message message)
        {
            return await Push(message);
        }


        [HttpGet]
        public async Task<CommonResponse> SendQuery([FromQuery] Message message)
        {
            return await Push(message);
        }

        [HttpPost]
        public async Task<CommonResponse> SendPost(Message message)
        {
            return await Push(message);
        }

        private async Task<CommonResponse> Push(Message message)
        {
            CommonResponse resp = new()
            {
                Code = StatusCodes.Status200OK,
                Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
            };

            try
            {
                var device = _server.GetDevice(message.DeviceKey);
                if (device != null)
                {
                    var apns = _apnsService.CreateUsingJwt();
                    var push = new ApplePush(ApplePushType.Alert)
                                .AddMutableContent()
                                .AddToken(device.DeviceToken)
                                .AddAlert(message.Title, message.Body);

                    if (!string.IsNullOrWhiteSpace(message.Group))
                    {
                        push.AddCustomProperty("group", message.Group);
                    }
                    var result = await apns.SendAsync(push);

                    resp.Message = result.IsSuccessful ? "success" : result.ReasonString;
                }
                else
                {
                    resp.Message = "DeviceToken is empty";
                }
            }
            catch (Exception ex)
            {
                resp.Message = ex.Message;
                _logger.LogError(ex, nameof(PushController));
            }
            return resp;
        }
    }
}
