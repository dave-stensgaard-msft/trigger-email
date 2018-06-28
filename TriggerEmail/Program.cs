using Newtonsoft.Json;
using Serilog;
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
        private static ILogger logger;
        
        static async Task Main(string[] args)
        {
            configuration = Configuration.ReadConfiguration();
            logger = BuildLogger();

            try
            {
                using (var client = AuthClient.GetAuthenticatedClient(configuration, logger))
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
                logger.Error(ex, ex.Message);
                throw;
            }


            Console.WriteLine("Done, press enter");
            Console.ReadLine();
        }

        private static ILogger BuildLogger()
        {
            return new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.Seq(serverUrl:"http://localhost:5341")
                .CreateLogger();
        }

        private static async Task<Guid[]> CreateEvent(HttpClient client)
        {
            logger.Information("creating event");
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
                logger.Information($"created event id: {newEventResponseObj.MessageIds[0]}");
                return newEventResponseObj.MessageIds;
            }

            logger.Error($"Failed with {newEventResponse.StatusCode}");
            return new Guid[] { };
        }

        private static async Task GetMessageStatus(HttpClient client, Guid messageId)
        {
            var url = $"https://emails-stable.azure.net/api/v1/messages/status/{messageId}";
            var messageStatusResponse = await client.GetAsync(url);

            if (messageStatusResponse.IsSuccessStatusCode)
            {
                var body = await messageStatusResponse.Content.ReadAsStringAsync();
                logger.Information($"Message Status: {body}");
            }
        }
    }
}
