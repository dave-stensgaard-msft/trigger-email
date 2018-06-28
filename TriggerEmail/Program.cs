using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TriggerEmail.Entities;

namespace TriggerEmail
{
    class Program
    {
        private static Configuration configuration;
        
        static async Task Main(string[] args)
        {
            configuration = Configuration.ReadConfiguration();
            Log.Logger = BuildLogger();

            try
            {
                using (var client = AuthClient.GetAuthenticatedClient(configuration))
                {
                    var messageIds = await CreateEvent(client);
                    foreach (var messageId in messageIds)
                    {
                        await GetMessageStatus(client, messageId);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, ex.Message);
                throw;
            }

            Log.CloseAndFlush();
            
            //Console.WriteLine("Done, press enter");
            //Console.ReadLine();
        }

        private static async Task<Guid[]> CreateEvent(HttpClient client)
        {
            Log.Logger.Information("creating event");
            var url = "https://emails-stable.azure.net/api/v1/event";
            var newEventRequest = new NewEventRequest
            {
                Email = "info@devdave.com",
                EventId = "dave-stensgaard-test",
                Culture = "en-US",
                Values = new Dictionary<string, object> {
                        { "SomeDate", DateTime.Now.ToShortDateString() },
                        { "CustomContent", "<strong>TEST</strong>"},
                        { "ShowList", new[]
                            {
                                new { ShowName = "Test 1", Time = "8 AM" },
                                new { ShowName = "Test 2", Time = "9 AM" },
                                new { ShowName = "Test 3", Time = "10 AM" },
                                new { ShowName = "Test 4", Time = "11 AM" },
                                new { ShowName = "Test 5", Time = "12 AM" },
                            }
                        }
                    }
            };

            var newEventResponse = await client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(newEventRequest), Encoding.UTF8, "application/json"));
            var newEventResponseContent = await newEventResponse.Content.ReadAsStringAsync();

            if (newEventResponse.IsSuccessStatusCode)
            {
                var newEventResponseObj = JsonConvert.DeserializeObject<NewEventResponse>(newEventResponseContent);
                Log.Logger.Information($"created event id: {newEventResponseObj.MessageIds[0]}");
                return newEventResponseObj.MessageIds;
            }

            Log.Logger.Error($"Failed with {newEventResponse.StatusCode}");
            return new Guid[] { };
        }

        private static async Task GetMessageStatus(HttpClient client, Guid messageId)
        {
            var url = $"https://emails-stable.azure.net/api/v1/messages/status/{messageId}";
            var messageStatusResponse = await client.GetAsync(url);

            if (messageStatusResponse.IsSuccessStatusCode)
            {
                var body = await messageStatusResponse.Content.ReadAsStringAsync();
                Log.Logger.Information($"Message Status: {body}");
            }
        }

        private static ILogger BuildLogger()
        {
            return new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.Seq(configuration.SeqServer, apiKey: configuration.SeqServerApiKey, restrictedToMinimumLevel: LogEventLevel.Information)
                .Enrich.WithProperty("Application", "Test.TriggerEmailJob")
                .Enrich.WithProperty("RequestId", Guid.NewGuid()) //a way to tie all logs with this evocation of the app together
                .CreateLogger();
        }
    }
}
