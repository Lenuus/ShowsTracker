using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShowsTracker.Common.Helpers;
using ShowsTracker.Web.Constants;

namespace ShowsTracker.Web.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
        }

        public async Task<T> PostAsync<T>(string url, object data)
        {
            T responseMapped;
            using (HttpClient client = new HttpClient())
            {
                url = RouteConstants.LocalUrl + url;
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                var content = new StringContent(JsonConvert.SerializeObject(data), null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                responseMapped = JsonConvert.DeserializeObject<T>(responseBody);
            }
            return responseMapped;
        }

        public async Task<T> DeleteAsync<T>(string url)
        {
            T responseMapped;
            using (HttpClient client = new HttpClient())
            {
                url = RouteConstants.LocalUrl + url;
                var request = new HttpRequestMessage(HttpMethod.Delete, url);
                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                responseMapped = JsonConvert.DeserializeObject<T>(responseBody);
            }
            return responseMapped;
        }

        public async Task<T> GetAsync<T>(string url)
        {
            T responseMapped;
            using (HttpClient client = new HttpClient())
            {
                url = RouteConstants.LocalUrl + url;
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                responseMapped = JsonConvert.DeserializeObject<T>(responseBody);
            }
            return responseMapped;
        }
    }
}