using ColorWithDevice.Hub;
using ColorWithDevice.Model;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;

namespace ColorWithDevice.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        Bluetooth.GadgeteerConnector connector;
        ColorHubProxy HubProxy;
        LedColor rawColor;

        public MainViewModel()
        {
            connector = new Bluetooth.GadgeteerConnector(App.Adapter);
            connector.ConnectionStatusChanged += connector_ConnectionStatusChanged;
            connector.ColorChanged += connector_ColorChanged;

            HubProxy = (Application.Current as App).ColorHubProxy;
            HubProxy.ReceiveColor += HubProxy_ReceiveColor;

            ConnectCommand = new Command(
                () => connector.RequestConnect(),
                () => connector.ConnectionStatus == Bluetooth.ConnectionStatus.Disconnected);
            DisconnectCommand = new Command(
                () => connector.RequestDisconnect(),
                () => connector.ConnectionStatus == Bluetooth.ConnectionStatus.Connected);
        }

        void connector_ConnectionStatusChanged(object sender, EventArgs e)
        {
            var color = connector.Color;
            Device.BeginInvokeOnMainThread(() =>
            {
                (ConnectCommand as Command).ChangeCanExecute();
                (DisconnectCommand as Command).ChangeCanExecute();
                RaisePropertyChanged("StatusText");
                RawColor = color;
            });
        }

        void connector_ColorChanged(object sender, EventArgs e)
        {
            var color = connector.Color;
            if (RawColor != color)
            {
                HubProxy.PublishColor(color);
            }
            Device.BeginInvokeOnMainThread(() => RawColor = color);
        }

        private void HubProxy_ReceiveColor(object sender, ReceiveColorEventArgs e)
        {
            Debug.WriteLine("ReceiveColor: {0}", e.Color);
            if (connector.ConnectionStatus != Bluetooth.ConnectionStatus.Connected)
            {
                Device.BeginInvokeOnMainThread(() => RawColor = e.Color);
            }
        }

        public string StatusText
        {
            get
            {
                switch (connector.ConnectionStatus)
                {
                    case Bluetooth.ConnectionStatus.Disconnected:
                        return "Device Disconnected";
                    case Bluetooth.ConnectionStatus.DeviceDiscovering:
                        return "Device Discovering...";
                    case Bluetooth.ConnectionStatus.Connecting:
                        return "Device Connecting...";
                    case Bluetooth.ConnectionStatus.ServiceDiscovering:
                        return "Device Service Discovering...";
                    case Bluetooth.ConnectionStatus.Connected:
                        return "Device Connected";
                    default:
                        return "Device Status Unknown";
                }
            }
        }

        public Color Color
        {
            get { return RawColor.ToFormColor(); }
        }

        private LedColor RawColor
        {
            get { return rawColor; }
            set
            {
                rawColor = value;
                RaisePropertyChanged("Color");
            }
        }

        public bool AllowDevice
        {
            get { return true; }
        }

        #region Commands 

        public ICommand ConnectCommand { get; private set; }
        public ICommand DisconnectCommand { get; private set; }

        #endregion

        #region PropetyChanged Support

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
