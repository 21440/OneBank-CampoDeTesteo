using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureServiceBusReceiver
{
    class Program
    {
        static QueueClient queueClient;
        static void Main(string[] args)
        {
            string sbConnectionString = "Endpoint=sb://onebank-integration.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=+9zZZhyhStmzCt60yD8pMeHLHtw9OakOn+mRF4PuZRk=";
            string sbQueueName = "main-queue";
            //string sbQueueName = "main-queue/$DeadLetterQueue";

            try
            {
                queueClient = new QueueClient(sbConnectionString, sbQueueName);

                var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
                {
                    MaxConcurrentCalls = 1,
                    AutoComplete = false
                };
                queueClient.RegisterMessageHandler(ReceiveMessagesAsync, messageHandlerOptions);
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

        static async Task ReceiveMessagesAsync(Message message, CancellationToken token)
        {
            try
            {
                var DecodedMessage = Encoding.UTF8.GetString(message.Body).Split('\n');
                Console.WriteLine($"to: {message.Label}");
                foreach (var line in DecodedMessage)
                {
                    Console.WriteLine(line);
                }

                //UNCOMMENT BELOW CODE TO GENERATE EXCEPTION, SO THAT MESSSAGE WILL BE ADDED IN DEAD LETTER QUEUE
                //int i = 0;
                //i = i / Convert.ToInt32(message);

                await queueClient.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
                await queueClient.AbandonAsync(message.SystemProperties.LockToken);
            }
        }

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine(exceptionReceivedEventArgs.Exception);
            return Task.CompletedTask;
        }
    }
}
