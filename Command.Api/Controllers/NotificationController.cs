using Amazon.DynamoDBv2;
using Amazon.SQS;
using Command.Api.Services;
using Contracts.Messages;
using DeltaReceiver.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Command.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(ILogger<NotificationController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> PostCommand()
    {
        var sqsClient = new AmazonSQSClient(Amazon.RegionEndpoint.USEast2);
        var dynamoDbClient = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USEast2);
        var publisher = new SqsPublisher(sqsClient, dynamoDbClient);

        await publisher.PublishAsync<NotificationMessage>("command-notifications", "command_notifications_tids", new NotificationMessage
        {
            MessageType = "Command",
            Id = Guid.NewGuid(),
            Notification = "Command is initiated.",
            Type = 1,

        });

        return Ok();
    }
}
