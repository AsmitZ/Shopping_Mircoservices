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
    private readonly IOptions<AwsOptions> _awsOptions;
    private readonly IEmailService _emailService;
    private readonly AmazonSQSClient _sqsClient;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private string? _queueUrl;

    public BusMessageReceiver(IOptions<AwsOptions> awsOptions, IEmailService emailService)
    {
        ArgumentNullException.ThrowIfNull(awsOptions);
        ArgumentNullException.ThrowIfNull(emailService);
        _awsOptions = awsOptions;
        _emailService = emailService;
        _cancellationTokenSource = new CancellationTokenSource();

        var basicAwsCredentials = new BasicAWSCredentials(_awsOptions.Value.AccessKey, _awsOptions.Value.SecretKey);
        _sqsClient = new AmazonSQSClient(basicAwsCredentials, Amazon.RegionEndpoint.APSouth1);
    }

    public async Task Start()
    {
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            _queueUrl ??= (await _sqsClient.GetQueueUrlAsync(_awsOptions.Value.QueueName)).QueueUrl;
            try
            {
                var receiveMessageRequest = new ReceiveMessageRequest
                {
                    QueueUrl = _queueUrl,
                    MaxNumberOfMessages = 10,
                };

                var response =
                    await _sqsClient.ReceiveMessageAsync(receiveMessageRequest, _cancellationTokenSource.Token);

                if (response.Messages.Count > 0)
                {
                    foreach (var message in response.Messages)
                    {
                        var cartDto = JsonSerializer.Deserialize<CartDto>(message.Body);
                        if (cartDto == null)
                        {
                            Console.WriteLine("Error deserializing the message");
                            continue;
                        }
                        var logEmail = await _emailService.SendAndLogEmail(cartDto);

                        if (logEmail)
                        {
                            // After processing, delete the message
                            await _sqsClient.DeleteMessageAsync(_queueUrl, message.ReceiptHandle);    
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving messages from SQS with exception: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(2), _cancellationTokenSource.Token);
        }
    }

    public void Stop()
    {
        Console.WriteLine("Stopping SQS listener...");
        _cancellationTokenSource.Cancel();
        _sqsClient.Dispose();
    }
}