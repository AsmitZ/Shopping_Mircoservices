namespace Mango.Services.EmailAPI.Messaging;

public interface IBusMessageReceiver
{
   /// <summary>
   /// A method to start the message receiver
   /// </summary>
   /// <returns></returns>
   Task Start();
   
   /// <summary>
   /// A method to stop the message receiver
   /// </summary>
   void Stop();
}