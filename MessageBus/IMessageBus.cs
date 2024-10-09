namespace MessageBus;

public interface IMessageBus
{
    Task PublishMessage(object message);
}