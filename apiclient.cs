using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vosplzen.sem2._2023.apiClient.Contracts;

namespace vosplzen.sem2._2023.apiClient
{

    public class ApiClient : IDisposable
    {
        private HttpClient httpClient;

        public ApiClient()
        {
            httpClient = new HttpClient();
        }
       
        public async Task<bool> SendMessage(object message, string baseurl, string authToken)
        {
            try
            {
                var settings = new Newtonsoft.Json.JsonSerializerSettings() { Culture = new CultureInfo("cs-CZ") };
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(message, Formatting.Indented, settings);

                Console.WriteLine("message:");
                Console.WriteLine(json);

                var data = new StringContent(json, Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Add("Security-Token", authToken);

                var response = await httpClient.PostAsync($@"{baseurl}/api/recieve-measurements", data);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Message sent successfully.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send data: {ex.Message}");
            }

            return false;
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }
}
