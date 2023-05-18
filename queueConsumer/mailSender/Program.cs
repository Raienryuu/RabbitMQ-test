namespace mailSender
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string rabbitHost = "localhost";
            string user = "guest";
            string pass = "guest";
            string vhost = "/";
            MailQueueConsumer mailConsumer = new(rabbitHost, user, pass, vhost);

            
            mailConsumer.Consume("mailQueue");
            Console.WriteLine("Press Enter to stop the application.");
            Console.ReadLine();
            

        }
    }
}