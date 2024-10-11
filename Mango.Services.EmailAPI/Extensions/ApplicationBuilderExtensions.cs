using Mango.Services.EmailAPI.Messaging;

namespace Mango.Services.EmailAPI.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void UseAwsListener(this IApplicationBuilder app)
    {
        var bus = app.ApplicationServices.GetRequiredService<IBusMessageReceiver>();
        var applicationHostLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

        applicationHostLifetime.ApplicationStarted.Register(ShoppingQueueListener);
        applicationHostLifetime.ApplicationStopping.Register(UserRegisteredQueueListener);
        applicationHostLifetime.ApplicationStopping.Register(OnStop);

        return;

        void OnStop() => bus.Stop();

        async void ShoppingQueueListener()
        {
            try
            {
                await bus.ReceiveFromShoppingQueue();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation was canceled.");
            }
        }

        async void UserRegisteredQueueListener()
        {
            try
            {
                await bus.ReceiveFromUserRegisteredQueue();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation was canceled.");
            }
        }
    }
}