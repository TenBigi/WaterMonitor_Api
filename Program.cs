
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Text;
using vosplzen.sem2._2023.apiClient;
using vosplzen.sem2._2023.apiClient.Contracts;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        IConfiguration config = builder.Build();

        string connectionString = config["Sql:ConnectionString"];

        string apiBaseUri = config["Api:BaseUri"];
        string token = config["Api:AuthToken"];

        string logFolder = config["App:LogFolder"];
        string errorFolder = config["App:ErrorFolder"];

        while (true)
        {
            await SendValues(config);
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }

    public static async Task SendValues(IConfiguration config)
    {
        string apiBaseUri = config["Api:BaseUri"];

        string tokenEndpoint = $"{apiBaseUri}/api/get-latest-token";
        string authToken = await GetLatestTokenAsync(tokenEndpoint);

        var message = GenerateFloodReport();

        using (var apiclient = new ApiClient())
        {
            await apiclient.SendMessage(message, apiBaseUri, authToken);
        }
    }

    public static async Task<string> GetLatestTokenAsync(string tokenEndpoint)
    {
        using (HttpClient client = new())
        {
            var response = await client.GetAsync(tokenEndpoint);

            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                return token;
            }
            else
            {
                throw new Exception($"Failed to retrieve latest token. Status code: {response.StatusCode}");
            }
        }
    }

    public static FloodReport GenerateFloodReport()
    {
        Random random = new Random();
        int value = (random.Next(1,100));
        int stationId = 3;
        DateTime now = DateTime.Now;
        DateTime oneYearAgo = now.AddYears(-1);

        DateTime dateTime = new DateTime(now.Year, random.Next(1, 13), random.Next(1, 29), random.Next(0, 24), random.Next(0, 60), random.Next(0, 60));

        if (dateTime > now)
        {
            dateTime = now;
        }
        else if (dateTime < oneYearAgo)
        {
            dateTime = oneYearAgo;
        }

        FloodReport floodReport = new FloodReport
        {
            StationId = stationId,
            TimeStamp = dateTime,
            Value = value
        };

        return floodReport;
    }
}