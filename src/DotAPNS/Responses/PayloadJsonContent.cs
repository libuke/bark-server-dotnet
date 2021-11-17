using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotAPNS.Responses;

public class PayloadJsonContent : StringContent
{
    const string JsonMediaType = "application/json";

    public PayloadJsonContent(object obj) : this(obj is string str ? str : JsonSerializer.Serialize(obj, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault })) { }

    PayloadJsonContent(string content) : base(content, Encoding.UTF8, JsonMediaType) { }
}
