namespace Mango.Services.EmailAPI.Messaging;

public interface IBusMessageReceiver
{
   /// <summary>
   /// A method to start the message receiver
   /// </summary>
   /// <returns></returns>
   Task ReceiveFromShoppingQueue();
   
   /// <summary>
   /// A method to start the message receiver
   /// </summary>
   /// <returns></returns>
   Task ReceiveFromUserRegisteredQueue();
   
   /// <summary>
   /// A method to stop the message receiver
   /// </summary>
   void Stop();
}