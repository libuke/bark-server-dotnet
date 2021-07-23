using System;
using DotAPNS;
using System.Reflection;
using DotAPNS.Extensions;
using System.ComponentModel;
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

        #region Route
        public PushController(ILogger<PushController> logger, IDeviceServer server, IApnsService apnsService)
        {
            _logger = logger;
            _server = server;
            _apnsService = apnsService;
        }

        [HttpGet("/{deviceKey}/{title}/{body?}")]
        public async Task<CommonResponse> GetRoute(string deviceKey, string? title, string? body, [FromQuery] string? sound, [FromQuery] MessageExtra? extra)
        {
            var head = new MessageHead
            {
                DeviceKey = deviceKey,
                Title = title,
                Body = body,
                Sound = sound
            };
            return await Push(head, extra);
        }

        [HttpGet("/{deviceKey}")]
        public async Task<CommonResponse> GetQuery(string deviceKey, [FromQuery] string? title, [FromQuery] string? body, [FromQuery] string sound, [FromQuery] MessageExtra? extra)
        {
            var head = new MessageHead
            {
                DeviceKey = deviceKey,
                Title = title,
                Body = body,
                Sound = sound
            };
            return await Push(head, extra);
        }

        [HttpPost]
        public async Task<CommonResponse> Post(Message message)
        {
            return await Push(message.Head, message.Extra);
        }
        #endregion

        private async Task<CommonResponse> Push(MessageHead head, MessageExtra? extra)
        {
            CommonResponse resp = new()
            {
                Code = StatusCodes.Status200OK,
                Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
            };

            try
            {
                var device = _server.GetDevice(head.DeviceKey);
                if (device != null)
                {
                    var apns = _apnsService.CreateUsingJwt();
                    var push = new ApplePush(ApplePushType.Alert)
                                .AddMutableContent()
                                .AddToken(device.DeviceToken);

                    if (head.Title == null)
                    {
                        push.AddAlert(head.Body ?? "");
                    }
                    else
                    {
                        push.AddAlert(head.Title, head.Body ?? "");
                    }
                    if (!string.IsNullOrWhiteSpace(head.Sound))
                    {
                        push.AddSound(head.Sound);
                    }

                    if (extra != null)
                    {
                        foreach (var property in extra.GetType().GetProperties())
                        {
                            if (property.GetValue(extra) is string value)
                            {
                                var display = property.GetCustomAttribute<DisplayNameAttribute>() ?? throw new InvalidOperationException($"Not Add DisplayNameAttribute");
                                push.AddCustomProperty(display.DisplayName, value);
                            }
                        }
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
