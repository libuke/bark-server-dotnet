using Microsoft.AspNetCore.Mvc;

namespace BarkServerNet.Controllers;

[ApiController]
[Route("[controller]")]
public class PingController : ControllerBase
{
    [HttpGet]
    public CommonResponse Get()
    {
        CommonResponse response = new()
        {
            Code = StatusCodes.Status200OK,
            Message = "pong",
            Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
        };
        return response;
    }
}
