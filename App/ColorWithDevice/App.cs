using ColorWithDevice.Hub;
using Microsoft.AspNet.SignalR.Client;
using Robotics.Mobile.Core.Bluetooth.LE;
using System.Diagnostics;
using Xamarin.Forms;

namespace ColorWithDevice
{
    public class App : Application
    {
        public static IAdapter Adapter { get; private set; }

        public HubConnection HubConnection { get; private set; }

        public ColorHubProxy ColorHubProxy { get; private set; }

        public App(IAdapter adapter)
        {
            Adapter = adapter;
            CreateHubConnection();
            MainPage = new Pages.MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        protected void CreateHubConnection()
        {
			// please input your url
            HubConnection = new HubConnection("http://localhost:5000/");
            HubConnection.StateChanged += HubConnection_StateChanged;
            ColorHubProxy = new ColorHubProxy(HubConnection);
            HubConnection.Start();
        }

        private void HubConnection_StateChanged(StateChange obj)
        {
            Debug.WriteLine(obj);
        }
    }
}
