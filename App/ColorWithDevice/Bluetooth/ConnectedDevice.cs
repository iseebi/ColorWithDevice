using ColorWithDevice.Model;
using Robotics.Mobile.Core.Bluetooth.LE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWithDevice.Bluetooth
{
    class ConnectedDevice : IDisposable
    {
        public IDevice Device { get; private set; }
        public IService Service { get; private set; }
        public ICharacteristic ReadCharacteristic { get; private set; }
        public ICharacteristic WriteCharacteristic { get; private set; }

        public ConnectedDevice(IDevice connectedDevice, IService connectedService, ICharacteristic readCharacteristic, ICharacteristic writeCharacteristic)
        {
            Device = connectedDevice;
            Service = connectedService;
            ReadCharacteristic = readCharacteristic;
            WriteCharacteristic = writeCharacteristic;

            ReadCharacteristic.ValueUpdated += ReadCharacteristic_ValueUpdated;
            ReadCharacteristic.StartUpdates();
            Xamarin.Forms.Device.OnPlatform(null, async () =>
            {
                while (ReadCharacteristic != null)
                {
                    await Task.Delay(1000);
                    var c = ReadCharacteristic;
                    if (c != null)
                    {
                        await ReadCharacteristic.ReadAsync();
                    }
                }
            });
        }

        void ReadCharacteristic_ValueUpdated(object sender, Robotics.Mobile.Core.Bluetooth.LE.CharacteristicReadEventArgs e)
        {
            Debug.WriteLine("ReadCharacteristics ValueUpdated enter");
            if (e.Characteristic.Value == null || e.Characteristic.Value.Length == 0) return;
            Debug.WriteLine("ReadCharacteristics ValueUpdated: {0}", e.Characteristic.StringValue);

            int colorCode;
            if (int.TryParse(e.Characteristic.StringValue, out colorCode))
            {
                Color = (LedColor)colorCode;
            }
        }

        public void Dispose()
        {
            if (ReadCharacteristic != null)
            {
                ReadCharacteristic.ValueUpdated -= ReadCharacteristic_ValueUpdated;
                ReadCharacteristic.StopUpdates();
                System.Threading.Tasks.Task.Delay(1000).Wait();
                ReadCharacteristic = null;
            }
            if (WriteCharacteristic != null)
            {
                WriteCharacteristic = null;
            }
            if (Service != null)
            {
                Service = null;
            }
            if (Device != null)
            {
                Device = null;
            }
        }

        public LedColor Color
        {
            get { return color; }
            private set
            {
                color = value;
                if (ColorChanged != null)
                {
                    ColorChanged(this, EventArgs.Empty);
                }
            }
        }
        LedColor color;

        public void UpdateColor(LedColor color)
        {
            var val = color.ToString() + ".";
            WriteCharacteristic.Write(UTF8Encoding.UTF8.GetBytes(val));
        }

        public event EventHandler ColorChanged;
    }
}
