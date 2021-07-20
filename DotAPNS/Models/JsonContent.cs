using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotAPNS
{
    public class JsonContent : StringContent
    {
        const string JsonMediaType = "application/json";

        public JsonContent(object obj) : this(obj is string str ? str : JsonSerializer.Serialize(obj, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault })) { }

        JsonContent(string content) : base(content, Encoding.UTF8, JsonMediaType) { }
    }
}