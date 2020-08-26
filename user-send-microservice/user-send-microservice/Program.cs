using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace user_send_microservice
{
    class Program
    {
        static ITopicClient topicClient;
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            string ServiceBusConnectionString = "[primary-connection-string]";
            string TopicName = "[topic-name]";

            topicClient = new TopicClient(ServiceBusConnectionString, TopicName);

            // Send messages.
            await SendUserMesasage();

            Console.ReadKey();

            await topicClient.CloseAsync();
        }

        static async Task SendUserMesasage()
        {
            List<User> users = GetDummyDataForUser();

            var serializeUser = JsonConvert.SerializeObject(users);

            string messageType = "userData";

            string messageId = Guid.NewGuid().ToString();

            var message = new ServiceBusMessage
            {
                Id = messageId,
                Type = messageType,
                Content = serializeUser
            };

            var serializeBody = JsonConvert.SerializeObject(message);

            // send data to bus
            var busMessage = new Message(Encoding.UTF8.GetBytes(serializeBody));
            busMessage.UserProperties.Add("Type", messageType);
            busMessage.MessageId = messageId;

            await topicClient.SendAsync(busMessage);

            Console.WriteLine("Message has been sent!");
        }

        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class ServiceBusMessage
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public string Content { get; set; }
        }

        private static List<User> GetDummyDataForUser()
        {
            User user = new User();
            List<User> listUsers = new List<User>();
            for(int i = 1; i < 3; i++)
            {
                user = new User();
                user.Id = i;
                user.Name = "User" + i;
                listUsers.Add(user);
            }
            return listUsers;
        }
    }
}
