using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EsepWebhook;

public class Function
{
    private static readonly HttpClient Client = new HttpClient();

    public async Task<string> FunctionHandler(object input, ILambdaContext context)
    {
        dynamic json = JsonConvert.DeserializeObject<dynamic>(input.ToString());

        // Keep this line exactly
        string payload = $"{{'text':'Issue Created: {json.issue.html_url}'}}";

        string slackUrl = Environment.GetEnvironmentVariable("SLACK_URL");

        var request = new HttpRequestMessage(HttpMethod.Post, slackUrl)
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };

        HttpResponseMessage response = await Client.SendAsync(request);
        return await response.Content.ReadAsStringAsync();
    }
}
