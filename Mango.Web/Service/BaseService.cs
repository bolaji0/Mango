using Mango.Web.Models;
using Mango.Web.Service.IService;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Mango.Web.Service
{
    public class BaseService : IBaseService
    {
        //Redux Query for calling react api
        private readonly IHttpClientFactory _httpClientFactory;
        public BaseService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

       
        public async Task<ResponseDTO> SendAsync(RequestDTO requestDTO)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("MangoAPI");
                HttpRequestMessage message = new();
                message.Headers.Add("Accept", "application/json");
                // token

                message.RequestUri = new Uri(requestDTO.Url);
                if (requestDTO.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDTO.Data), Encoding.UTF8, "application/json");
                }
                //return 
                HttpResponseMessage apiResponse = null;
                switch (requestDTO.ApiType)
                {
                    case Utility.SD.ApiType.Post:
                        message.Method = HttpMethod.Post;
                        break;
                    case Utility.SD.ApiType.Put:
                        message.Method = HttpMethod.Put;
                        break;
                    case Utility.SD.ApiType.Delete:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                // To send response
                apiResponse = await client.SendAsync(message);
                //Handle status code response
                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new() { IsSuccess = false, Message = "not Found" };
                    case HttpStatusCode.Forbidden:
                        return new() { IsSuccess = false, Message = "Forbidden" };
                    case HttpStatusCode.Unauthorized:
                        return new() { IsSuccess = false, Message = "Unauthorised" };
                    case HttpStatusCode.InternalServerError:
                        return new() { IsSuccess = false, Message = "Internal Server Error" };
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var apiResponseDTO = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
                        return apiResponseDTO;
                }

            }
            catch (Exception ex)
            {
                var dto = new ResponseDTO
                {
                    Message = ex.Message.ToString(),
                    IsSuccess = false,
                };
                return dto;
            }
        }
    }
}
