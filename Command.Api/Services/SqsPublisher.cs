using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using Contracts.Messages;
using System.Text.Json;

namespace Command.Api.Services
{
    public class SqsPublisher
    {
        private IAmazonSQS AmazonSqs { get; set; }
        private IAmazonDynamoDB DynamoDB { get; set; }
        public SqsPublisher(IAmazonSQS amazonSqs, IAmazonDynamoDB dynamoDb)
        {
            AmazonSqs = amazonSqs;
            DynamoDB = dynamoDb;
        }

        //Publish messages of any type so it would be anonymous type or generic type.
        public async Task PublishAsync<T>(string queueName, string tidTableName, T message) where T : IMessage
        {
            var queue = await AmazonSqs.GetQueueUrlAsync(queueName);
            var createdDateTimestamp = DateTime.UtcNow.ToString();
            var request = new SendMessageRequest
            {
                QueueUrl = queue.QueueUrl,
                MessageBody = JsonSerializer.Serialize(message),
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                {
                    {
                        nameof(IMessage.MessageType),
                        new MessageAttributeValue
                        {
                            StringValue = message.MessageType,
                            DataType = "String"
                        }
                    },
                    {
                        "timestamp",
                        new MessageAttributeValue
                        {
                            StringValue = createdDateTimestamp,
                            DataType = "String"
                        }
                    },
                    {
                        "version",
                        new MessageAttributeValue
                        {
                            StringValue = "v1",
                            DataType = "String"
                        }
                    }
                }
            };
            var messageResponse = await AmazonSqs.SendMessageAsync(request);
        }
    }
}
