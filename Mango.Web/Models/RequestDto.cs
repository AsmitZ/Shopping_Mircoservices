using Mango.Web.Utilities;

namespace Mango.Web.Models;

public class RequestDto
{
    public SD.ApiType ApiType { get; set; } = SD.ApiType.GET;
    public object Data { get; set; }
    public string Url { get; set; }
    public string AccessToken { get; set; }
    public SD.ContentType ContentType { get; set; } = SD.ContentType.Json;
}