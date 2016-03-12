using ColorWithDevice.Model;
using Microsoft.AspNet.SignalR.Client;
using System;

namespace ColorWithDevice.Hub
{
    public class ColorHubProxy
    {
        public event EventHandler<ReceiveColorEventArgs> ReceiveColor;

        public HubConnection Connection { get; private set; }

        public IHubProxy HubProxy { get; private set; }

        public ColorHubProxy(HubConnection connection)
        {
            Connection = connection;
            HubProxy = connection.CreateHubProxy("ColorHub");
            HubProxy.On<LedColor>("ReceiveColor", OnReceiveColor);
        }

        public void PublishColor(LedColor color)
        {
            if (Connection.State == ConnectionState.Connected)
            {
                HubProxy.Invoke("PublishColor", color);
            }
        }
        private void OnReceiveColor(LedColor color)
        {
            if (ReceiveColor != null)
            {
                ReceiveColor(this, new ReceiveColorEventArgs(color));
            }
        }
    }

    public class ReceiveColorEventArgs : EventArgs
    {
        public LedColor Color { get; private set; }

        public ReceiveColorEventArgs(LedColor color)
        {
            Color = color;
        }
    }
}
