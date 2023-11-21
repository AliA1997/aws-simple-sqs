using Amazon.SQS;
using Amazon.SQS.Model;

namespace DeltaReceiver.Api.Services
{
    public class SqsService: BackgroundService
    {
        private IAmazonSQS AmazonSQS { get; }

        public SqsService(IAmazonSQS amazonSQS)
        {
            AmazonSQS = amazonSQS;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var queue = await AmazonSQS.GetQueueUrlAsync("command-notifications");

            var receiveRequest = new ReceiveMessageRequest
            {
                QueueUrl = queue.QueueUrl,
                MessageAttributeNames = new List<string> { "All" },
                AttributeNames = new List<string>{"All"}
            };

            while(!stoppingToken.IsCancellationRequested)
            {
                var receivedResponse = await AmazonSQS.ReceiveMessageAsync(receiveRequest, stoppingToken);
                if(receivedResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine(receivedResponse.HttpStatusCode);
                    return;
                }

                foreach(var msg in receivedResponse.Messages)
                {
                    Console.WriteLine(msg.Body);
                    await AmazonSQS.DeleteMessageAsync(queue.QueueUrl, msg.ReceiptHandle);
                }
            }


        }
    }
}
