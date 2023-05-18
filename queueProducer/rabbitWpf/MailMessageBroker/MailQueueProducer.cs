using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;


namespace rabbitWpf.MessageBroker
{
    internal class MailQueueProducer
    {
        public IModel channel;
        public MailQueueProducer(string rabbitHost, string user, string pass, string vhost)
        {
            var factory = new ConnectionFactory { HostName = rabbitHost, UserName = user, Password = pass, VirtualHost = vhost };

            var connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(queue: "mailQueue",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);
        }

        public void SendMessage(MailModel msg)
        {
            var jsonData = JsonConvert.SerializeObject(msg);
            var body = Encoding.UTF8.GetBytes(jsonData);

            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: "mailQueue",
                                 basicProperties: null,
                                 body: body);
        }
    }
}
