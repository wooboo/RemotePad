using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RemotePad.HostApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Any())
            {
                await ProcessCallback(e.Args.First());
                this.Shutdown();
            }
            else
            {
                MainWindow wnd = new MainWindow();

                wnd.Show();
            }
        }

        private static async Task ProcessCallback(string args)
        {
            var response = new AuthorizeResponse(args);
            if (!String.IsNullOrWhiteSpace(response.State))
            {
                Console.WriteLine($"Found state: {response.State}");
                var callbackManager = new CallbackManager(response.State);
                await callbackManager.RunClient(args);
            }
            else
            {
                Console.WriteLine("Error: no state on response");
            }
        }
    }
}
