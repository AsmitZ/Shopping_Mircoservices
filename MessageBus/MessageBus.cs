using System.Text.Json;
using Amazon.Runtime;
using Amazon.SQS;

namespace MessageBus;

public class MessageBus(AwsOptions awsOptions) : IMessageBus
{
    public async Task PublishMessage(object message, string queueName)
    {
        var basicAwsCredentials = new BasicAWSCredentials(awsOptions.AccessKey, awsOptions.SecretKey);
        using var client = new AmazonSQSClient(basicAwsCredentials, Amazon.RegionEndpoint.APSouth1);
        var queueUrl = await client.GetQueueUrlAsync(queueName);
        _ = await client.SendMessageAsync(queueUrl.QueueUrl, JsonSerializer.Serialize(message));
    }
}