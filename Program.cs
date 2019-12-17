using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Lab2_3
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Enter username");
            string username = Console.ReadLine();
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            IModel channel = factory.CreateConnection().CreateModel();
            channel.ExchangeDeclare("chat", "fanout");
            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queueName, "chat", "");
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body);
                if (message.Split(':')[0] != username)
                    Console.WriteLine(message);
            };
            channel.BasicConsume(queueName, true, consumer);
            Console.Clear();
            Console.WriteLine("Welcome, " + username);
            while (true)
            {
                string message = Console.ReadLine();
                var body = Encoding.UTF8.GetBytes(username + ": " + message);
                channel.BasicPublish("chat", "", null, body);
            }
        }
    }
}
