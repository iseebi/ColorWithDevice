using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColorWithDevice.Model;

namespace ColorWithDevice.Server
{
    public class ColorHub : Hub
    {
        public void PublishColor(LedColor color)
        {
            Console.WriteLine("receive new color: {0}", color);
            Clients.All.ReceiveColor(color);
        }
    }
}
