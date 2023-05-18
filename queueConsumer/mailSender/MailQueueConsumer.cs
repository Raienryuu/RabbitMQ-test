using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace mailSender
{
    internal class MailQueueConsumer
    {
        private IModel channel { get; set; }
        private EventingBasicConsumer consumer { get; set; }
        public MailQueueConsumer(string rabbitHost, string user, string pass, string vhost)
        {
            var factory = new ConnectionFactory { HostName = rabbitHost, UserName = user, Password = pass, VirtualHost = vhost };

            var connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(queue: "mailQueue",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

            InitializeConsumingFunction();
        }

        private void InitializeConsumingFunction()
        {
            consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                string jsonObject = "";
                var messageBytes = ea.Body.ToArray();
                jsonObject = Encoding.UTF8.GetString(messageBytes);
                try
                {
                    var message = JsonConvert.DeserializeObject<MailModel>(jsonObject);
                    ProcessMessageAsync(message);
                    
                } catch (JsonSerializationException e)
                {
                    Console.WriteLine(e.Message);
                    channel.BasicReject(deliveryTag: ea.DeliveryTag, false);
                }
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
        }

        private void ProcessMessageAsync(MailModel message)
        {
            Console.WriteLine($"Received message:");
            foreach (var address in message.Recipients){
                Console.WriteLine(address);
            }
            Console.WriteLine(message.Subject); Console.WriteLine(message.Body);
            foreach (var attachment in message.Attachments)
            {
                Console.WriteLine(attachment);
            }

            switch (message.PlatformType) {
                case "Smtp":
                    sendMailThroughSmtp(message);
                    break;
                case "Mailgun":
                    sendMailThroughMailgun(message);
                    break;
                case "SendGrid":
                    sendMailThroughSendGrid(message);
                    break;
                    }
        }

        private void sendMailThroughMailgun(MailModel message)
        {
            throw new NotImplementedException();
        }

        private void sendMailThroughSendGrid(MailModel message)
        {
            throw new NotImplementedException();
        }

        private List<Attachment> GetAttachments(List<AttachmentData> attachmentDatas)
        {
            List<Attachment> attachments = new List<Attachment>();

            foreach (var attachmentData in attachmentDatas)
            {
                var contentStream = new MemoryStream(attachmentData.Content);
                attachments.Add(new Attachment(contentStream, attachmentData.FileName));
            }
            return attachments;
        }

        private void sendMailThroughSmtp(MailModel message)
        {
            var attachments = GetAttachments(message.Attachments);
            SmtpClient smtpClient = new SmtpClient("localhost", 25);
            smtpClient.EnableSsl = false;
            
            foreach (var recipient in message.Recipients)
            {
                Console.WriteLine("recipient: {0}", recipient);
                MailMessage mail = new MailMessage(message.Sender, recipient, message.Subject, message.Body);
                foreach (var attachment in attachments)
                {
                    mail.Attachments.Add(attachment);
                }
                try
                {
                    smtpClient.Send(mail);
                    Console.WriteLine("Email sent successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to send email: " + ex.Message);
                }
            } 
        }

        public void Consume(string queueName)
        {
                channel.BasicConsume(queue: queueName,
                     consumer: consumer);
        }
    }
}
