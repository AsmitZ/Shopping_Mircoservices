using System.Net;
using System.Text;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utilities;
using Newtonsoft.Json;

namespace Mango.Web.Service;

public class BaseService : IBaseService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ITokenProvider _tokenProvider;

    public BaseService(IHttpClientFactory clientFactory, ITokenProvider tokenProvider)
    {
        _clientFactory = clientFactory;
        _tokenProvider = tokenProvider;
    }

    public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true)
    {
        try
        {
            HttpClient client = _clientFactory.CreateClient("MangoAPI");
            HttpRequestMessage request = new HttpRequestMessage();
            if (requestDto.ContentType == SD.ContentType.Json)
            {
                request.Headers.Add("Accept", "application/json");
            }
            else
            {
                request.Headers.Add("Accept", "*/*");
            }

            // TODO: add token
            request.RequestUri = new Uri(requestDto.Url);

            if (requestDto.ContentType == SD.ContentType.MultipartFormData)
            {
                var content = new MultipartFormDataContent();

                foreach (var property in requestDto.Data.GetType().GetProperties())
                {
                    var value = property.GetValue(requestDto.Data);
                    if (value is FormFile file)
                    {
                        content.Add(new StreamContent(file.OpenReadStream()), property.Name, file.FileName);
                    }
                    else
                    {
                        content.Add(new StringContent(value?.ToString() ?? string.Empty), property.Name);
                    }
                }

                request.Content = content;
            }
            else
            {
                if (requestDto.Data != null)
                {
                    request.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8,
                        "application/json");
                }
            }

            request.Method = requestDto.ApiType switch
            {
                SD.ApiType.POST => HttpMethod.Post,
                SD.ApiType.PUT => HttpMethod.Put,
                SD.ApiType.DELETE => HttpMethod.Delete,
                _ => HttpMethod.Get
            };
            if (withBearer)
            {
                var token = _tokenProvider.GetToken();
                request.Headers.Add("Authorization", "Bearer " + token);
            }

            var responseMessage = await client.SendAsync(request);
            return responseMessage.StatusCode switch
            {
                HttpStatusCode.NotFound => new ResponseDto { IsSuccess = false, Message = "Not Found" },
                HttpStatusCode.Forbidden => new ResponseDto { IsSuccess = false, Message = "Access Denied" },
                HttpStatusCode.Unauthorized => new ResponseDto { IsSuccess = false, Message = "Please Login Again" },
                HttpStatusCode.BadRequest => new ResponseDto
                    { IsSuccess = false, Message = await responseMessage.Content.ReadAsStringAsync() },
                _ => JsonConvert.DeserializeObject<ResponseDto>(await responseMessage.Content.ReadAsStringAsync())
            };
        }
        catch (Exception e)
        {
            return new ResponseDto { IsSuccess = false, Message = e.Message };
        }
    }
}