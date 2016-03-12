using ColorWithDevice.Model;
using Robotics.Mobile.Core.Bluetooth.LE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ColorWithDevice.Bluetooth
{
    class GadgeteerConnector
    {
        /*
        The main GATT UUID is: 2AC94B65-C8F4-48A4-804A-C03BC6960B80
        The RX UUID is : 4FD800F8-D3B6-48F9-B232-29E95984F76D
        The TX UUID is : 50E03F22-B496-4A73-9E85-335482ED4B12
         */

        static readonly Guid ServiceGuid = new Guid("2AC94B65-C8F4-48A4-804A-C03BC6960B80");
        static readonly Guid ReadGuid = new Guid("4FD800F8-D3B6-48F9-B232-29E95984F76D");
        static readonly Guid WriteGuid = new Guid("50E03F22-B496-4A73-9E85-335482ED4B12");

        IAdapter Adapter { get; set; }

        public GadgeteerConnector(IAdapter adapter)
        {
            Adapter = adapter;
            Adapter.DeviceDiscovered += Adapter_DeviceDiscovered;
            Adapter.DeviceConnected += Adapter_DeviceConnected;
            Adapter.DeviceDisconnected += Adapter_DeviceDisconnected;
            Adapter.ScanTimeoutElapsed += Adapter_ScanTimeoutElapsed;
        }


        public void RequestConnect()
        {
            if (ConnectionStatus == Bluetooth.ConnectionStatus.Disconnected)
            {
                ConnectionStatus = Bluetooth.ConnectionStatus.DeviceDiscovering;
                Adapter.StartScanningForDevices(ServiceGuid);
            }
        }

        public void RequestDisconnect()
        {
            if (ConnectionStatus != Bluetooth.ConnectionStatus.Disconnected)
            {
                ConnectedDevice = null;
                Adapter.StopScanningForDevices();
            }
        }

        private async void ProcessConnect(IDevice device)
        {
            try
            {
                ConnectionStatus = Bluetooth.ConnectionStatus.ServiceDiscovering;
                Debug.WriteLine("GetServiceAsync");
                var service = await device.GetServiceAsync(ServiceGuid);
                Debug.WriteLine("GetCharacteristicAsync(Read)");
                var readCharacteristic = await service.GetCharacteristicAsync(ReadGuid);
                Debug.WriteLine("GetCharacteristicAsync(Write)");
                var writeCharacteristic = await service.GetCharacteristicAsync(WriteGuid);

                ConnectedDevice = new ConnectedDevice(device, service, readCharacteristic, writeCharacteristic);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: {0}", ex);
            }
        }

        void Adapter_DeviceDiscovered(object sender, DeviceDiscoveredEventArgs e)
        {
            Debug.WriteLine("****** DeviceDiscovered");
            if (ConnectionStatus == Bluetooth.ConnectionStatus.DeviceDiscovering)
            {
                if (!string.IsNullOrWhiteSpace(e.Device.Name) && e.Device.Name.StartsWith("Gadgeteer")) { 
                    Adapter.StopScanningForDevices();
                    ConnectionStatus = Bluetooth.ConnectionStatus.Connecting;
                    Adapter.ConnectToDevice(e.Device);
                }
            }
        }

        void Adapter_DeviceConnected(object sender, DeviceConnectionEventArgs e)
        {
            Debug.WriteLine("****** DeviceConnected");
            if (ConnectionStatus == Bluetooth.ConnectionStatus.Connecting)
            {
                ProcessConnect(e.Device);
            } 
        }

        void Adapter_DeviceDisconnected(object sender, DeviceConnectionEventArgs e)
        {
            Debug.WriteLine("****** DeviceDisconnected");
            ConnectedDevice = null;
            Adapter.StopScanningForDevices();
        }

        void Adapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            if (ConnectionStatus == Bluetooth.ConnectionStatus.DeviceDiscovering && ConnectedDevice == null)
            {
                ConnectionStatus = Bluetooth.ConnectionStatus.Disconnected;
            }
        }

        void ConnectedDevice_ColorChanged(object sender, EventArgs e)
        {
            if (ColorChanged != null)
            {
                ColorChanged(this, EventArgs.Empty);
            }
        }

        ConnectedDevice ConnectedDevice
        {
            get { return connectedDevice; }
            set
            {
                if (connectedDevice != null)
                {
                    var rawDevice = connectedDevice.Device;
                    connectedDevice.Dispose();
                    connectedDevice.ColorChanged -= ConnectedDevice_ColorChanged;
                    Adapter.DisconnectDevice(rawDevice);
                }
                connectedDevice = value;
                if (connectedDevice == null)
                {
                    ConnectionStatus = Bluetooth.ConnectionStatus.Disconnected;
                }
                else
                {
                    connectedDevice.ColorChanged += ConnectedDevice_ColorChanged;
                    ConnectionStatus = Bluetooth.ConnectionStatus.Connected;
                }
                if (ColorChanged != null)
                {
                    ColorChanged(this, EventArgs.Empty);
                }
            }
        }
        ConnectedDevice connectedDevice;

        public ConnectionStatus ConnectionStatus {
            get { return connectionStatus; }
            set
            {
                connectionStatus = value;
                if (ConnectionStatusChanged != null)
                {
                    ConnectionStatusChanged(this, EventArgs.Empty);
                }
            }
        }
        ConnectionStatus connectionStatus;
        public event EventHandler ConnectionStatusChanged;

        public LedColor Color
        {
            get
            {
                if (ConnectedDevice != null)
                {
                    return ConnectedDevice.Color;
                }
                else
                {
                    return LedColor.Off;
                }
            }
        }
        public event EventHandler ColorChanged;
    }
}
