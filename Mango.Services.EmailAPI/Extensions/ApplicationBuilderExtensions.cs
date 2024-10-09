using Mango.Services.EmailAPI.Messaging;

namespace Mango.Services.EmailAPI.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseAwsListener(this IApplicationBuilder app)
    {
        var bus = app.ApplicationServices.GetRequiredService<IBusMessageReceiver>();
        var applicationHostLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

        applicationHostLifetime.ApplicationStarted.Register(OnStart);
        applicationHostLifetime.ApplicationStopping.Register(OnStop);

        return app;

        void OnStop() => bus.Stop();
        async void OnStart()
        {
            try
            {
                await bus.Start();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation was canceled.");
            }
        }
    }
}