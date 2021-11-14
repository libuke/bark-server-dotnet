using Microsoft.AspNetCore.Mvc;

namespace BarkServerNet.Controllers;

[Route("[controller]")]
[ApiController]
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
