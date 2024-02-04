using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using RoomieRosterWeb.Models;

namespace RoomieRosterWeb.Services
{
	public class BaseService
	{
        public async Task<object> SendGetRequestAsync<T>(string requestUrl, object requestOptions)
        {
            using (HttpClient client = new HttpClient())
            {
               

                Type myType = requestOptions.GetType();
                IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

                foreach (PropertyInfo prop in props)
                {
                    if(prop.Name == "token")
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" ,prop.GetValue(requestOptions, null).ToString());
                    }
                }

                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CustomResponseDto<T>>();
                }
                else
                {
                    return await response.Content.ReadFromJsonAsync<CustomResponseDto<ErrorModel>>();
                }
            }
        }

        public async Task<object> SendPostRequestAsync<T>(string requestUrl, object requestData)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(requestUrl,requestData);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CustomResponseDto<T>>();
                }
                else
                {
                    return await response.Content.ReadFromJsonAsync<CustomResponseDto<ErrorModel>>();
                }
            }
        }


        public async Task<object> SendPostRequestAsyncWithOptions<T>(string requestUrl, object requestData, object requestOptions)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Type myType = requestOptions.GetType();
                IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

                foreach (PropertyInfo prop in props)
                {
                    if (prop.Name == "token")
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", prop.GetValue(requestOptions, null).ToString());
                    }
                }

                HttpResponseMessage response = await client.PostAsJsonAsync(requestUrl, requestData);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CustomResponseDto<T>>();
                }
                else
                {
                    return await response.Content.ReadFromJsonAsync<CustomResponseDto<ErrorModel>>();
                }
            }
        }


        public async Task<object> SendPutRequestAsyncWithOptions<T>(string requestUrl, object requestData, object requestOptions)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Type myType = requestOptions.GetType();
                IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

                foreach (PropertyInfo prop in props)
                {
                    if (prop.Name == "token")
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", prop.GetValue(requestOptions, null).ToString());
                    }
                }

                HttpResponseMessage response = await client.PutAsJsonAsync<T>(requestUrl, (T)requestData);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CustomResponseDto<T>>();
                }
                else
                {
                    return await response.Content.ReadFromJsonAsync<CustomResponseDto<ErrorModel>>();
                }
            }
        }

        public async Task<object> SendDeleteRequestAsync<T>()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync("API_Url");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CustomResponseDto<T>>();
                }
                else
                {
                    return await response.Content.ReadFromJsonAsync<CustomResponseDto<ErrorModel>>();
                }
            }
        }
    }
}

