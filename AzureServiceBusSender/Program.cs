using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureServiceBusSender
{
    class Program
    {
        static QueueClient queueClient;
        static void Main(string[] args)
        {
            string sbConnectionString = "Endpoint=sb://onebank-integration.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=+9zZZhyhStmzCt60yD8pMeHLHtw9OakOn+mRF4PuZRk=";
            string sbQueueName = "main-queue";

            try
            {

                /*Console.Write("Input your message: ");
                string messageBody = Console.ReadLine();
                Console.Write("Input a label: ");
                string messageLabel = Console.ReadLine();*/

                var message = new Message { 
                    Label = "core",
                    Body = Encoding.UTF8.GetBytes("from: internet-banking\n" +
                                                  "operation: deposit\n" +
                                                  "data: Monto=1503.40\n" +
                                                  "user-id: 10001223574")};
                queueClient = new QueueClient(sbConnectionString, sbQueueName);

                Console.WriteLine($"Message Added in Queue:\nto: {message.Label}\n{Encoding.UTF8.GetString(message.Body)}");

                //var message = new Message(Encoding.UTF8.GetBytes(messageBody + "|" + messageLabel));
                //Console.WriteLine($"Message Added in Queue: {messageBody}");
                queueClient.SendAsync(message);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ReadKey();
                queueClient.CloseAsync();
            }
        }
    }
}
