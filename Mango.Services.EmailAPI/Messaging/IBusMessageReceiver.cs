namespace Mango.Services.EmailAPI.Messaging;

public interface IBusMessageReceiver
{
   Task Start();
   void Stop();
}