using IdentityModel.Client;
using IdentityModel.OidcClient;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RemotePad.HostApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HubConnection connection;
        private LoginResult loginResult;
        private Guid id = Guid.NewGuid();
        public MainWindow()
        {
            InitializeComponent();
            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5001/padshub", options =>
                {
                    options.AccessTokenProvider = GetAccessToken;
                })
                .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };
            connection.On("requestHost", async () =>
            {
                await connection.SendAsync("SendHost", connection.ConnectionId, new PadHost
                {
                    Name = "Dupa"
                });
            });


        }

        const string CustomUriScheme = "remote-pad-wpf-host";
        const string Authority = "https://localhost:5001";
        const string ClientId = "RemotePad.HostApp";
        private async Task<string> GetAccessToken()
        {
            if (this.loginResult == null)
            {
                new RegistryConfig(CustomUriScheme).Configure();
                var httpClient = new HttpClient()
                {
                    BaseAddress = new Uri(Authority)
                };

                var response = await httpClient.GetAsync($"/_configuration/{ClientId}");
                var json = await response.Content.ReadAsStringAsync();
                var conf = JsonConvert.DeserializeObject<JObject>(json);
                // create a redirect URI using the custom redirect uri
                string redirectUri = string.Format(CustomUriScheme + "://callback");
                Console.WriteLine("redirect URI: " + redirectUri);

                var options = new OidcClientOptions
                {
                    Authority = conf["authority"].ToString(),
                    ClientId = conf["client_id"].ToString(),
                    Scope = conf["scope"].ToString(),
                    RedirectUri = redirectUri,
                };


                var client = new OidcClient(options);
                var state = await client.PrepareLoginAsync();

                Console.WriteLine($"Start URL: {state.StartUrl}");

                var callbackManager = new CallbackManager(state.State);

                // open system browser to start authentication
                Process.Start(new ProcessStartInfo(state.StartUrl)
                {
                    UseShellExecute = true
                });

                Console.WriteLine("Running callback manager");
                var result = await callbackManager.RunServer();
                this.loginResult = await client.ProcessResponseAsync(result, state);
                this.BringIntoView();
            }
            return loginResult.AccessToken;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await connection.StartAsync();
            await connection.SendAsync("SendHost", connection.ConnectionId, new PadHost
            {
                Name = "Dupa"
            });
        }
    }
    public class PadHost
    {
        public string Name { get; set; }
    }
}
