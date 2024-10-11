using System.Text.Json;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Mango.Services.EmailAPI.Models.Dto;
using Mango.Services.EmailAPI.Services;
using MessageBus;
using Microsoft.Extensions.Options;

namespace Mango.Services.EmailAPI.Messaging;

public class BusMessageReceiver : IBusMessageReceiver
{
    private readonly AwsOptions _awsOptions;
    private readonly IEmailService _emailService;
    private readonly AmazonSQSClient _sqsClient;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private string? _shoppingQueueUrl;
    private string? _userRegisteredQueueUrl;

    public BusMessageReceiver(IOptions<AwsOptions> awsOptions, IEmailService emailService)
    {
        ArgumentNullException.ThrowIfNull(awsOptions);
        ArgumentNullException.ThrowIfNull(emailService);
        _awsOptions = awsOptions.Value;
        _emailService = emailService;
        _cancellationTokenSource = new CancellationTokenSource();

        var basicAwsCredentials = new BasicAWSCredentials(_awsOptions.AccessKey, _awsOptions.SecretKey);
        _sqsClient = new AmazonSQSClient(basicAwsCredentials, Amazon.RegionEndpoint.APSouth1);
    }

    public async Task ReceiveFromShoppingQueue()
    {
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            _shoppingQueueUrl ??= (await _sqsClient.GetQueueUrlAsync(_awsOptions.ShoppingQueue)).QueueUrl;
            await ReceiveMessageAsync<CartDto>(_shoppingQueueUrl);
            await Task.Delay(TimeSpan.FromSeconds(2), _cancellationTokenSource.Token);
        }
    }

    public async Task ReceiveFromUserRegisteredQueue()
    {
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            _userRegisteredQueueUrl ??= (await _sqsClient.GetQueueUrlAsync(_awsOptions.UserRegistrationQueue)).QueueUrl;
            // TODO: Receive message for user registered
            await Task.Delay(TimeSpan.FromSeconds(2), _cancellationTokenSource.Token);
        }
    }

    public void Stop()
    {
        Console.WriteLine("Stopping SQS listener...");
        _cancellationTokenSource.Cancel();
        _sqsClient.Dispose();
    }

    private async Task ReceiveMessageAsync<T>(string queueUrl)
    {
        try
        {
            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = 10,
            };

            var response =
                await _sqsClient.ReceiveMessageAsync(receiveMessageRequest, _cancellationTokenSource.Token);

            if (response.Messages.Count > 0)
            {
                foreach (var message in response.Messages)
                {
                    var messageBody = JsonSerializer.Deserialize<T>(message.Body);
                    if (messageBody == null)
                    {
                        Console.WriteLine("Error deserializing the message");
                        continue;
                    }

                    var logEmail = await _emailService.SendAndLogEmail(messageBody);

                    if (logEmail)
                    {
                        // After processing, delete the message
                        await _sqsClient.DeleteMessageAsync(queueUrl, message.ReceiptHandle);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving messages from SQS with exception: {ex.Message}");
        }
    }
}