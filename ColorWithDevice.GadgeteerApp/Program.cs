using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Modules.IngenuityMicro;

namespace ColorWithDevice.GadgeteerApp
{
    public partial class Program
    {
        // This method is run when the mainboard is powered up or reset.   

        enum Direction
        {
            Newtral,
            Positive,
            Negative
        }

        void ProgramStarted()
        {
            /*******************************************************************************************
            Modules added in the Program.gadgeteer designer view are used by typing 
            their name followed by a period, e.g.  button.  or  camera.
            
            Many modules generate useful events. Type +=<tab><tab> to add a handler to an event, e.g.:
                button.ButtonPressed +=<tab><tab>
            
            If you want to do something periodically, use a GT.Timer and handle its Tick event, e.g.:
                GT.Timer timer = new GT.Timer(1000); // every second (1000ms)
                timer.Tick +=<tab><tab>
                timer.Start();
            *******************************************************************************************/

            bluetoothSmart.DataReceived += bluetoothSmart_DataReceived;
            bluetoothSmart.Delimiter = ".";

            var timer = new GT.Timer(500);
            timer.Tick += timer_Tick;
            timer.Start();

            joystick.JoystickPressed += (s, e) => SetColor(LED7C.Color.Off);

            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");
        }

        void timer_Tick(GT.Timer timer)
        {
            var pos = joystick.GetPosition();
            var x = pos.X < -0.5 ? Direction.Negative : pos.X > 0.5 ? Direction.Positive : Direction.Newtral;
            var y = pos.Y < -0.5 ? Direction.Negative : pos.Y > 0.5 ? Direction.Positive : Direction.Newtral;
            if (x == Direction.Newtral && y == Direction.Newtral)
            {
            }
            else if (x == Direction.Negative && y == Direction.Negative)
            {
                SetColor(LED7C.Color.Red);
            }
            else if (x == Direction.Newtral && y == Direction.Negative)
            {
                SetColor(LED7C.Color.White);
            }
            else if (x == Direction.Positive && y == Direction.Negative)
            {
                SetColor(LED7C.Color.Yellow);
            }
            else if (x == Direction.Negative && y == Direction.Newtral)
            {
                SetColor(LED7C.Color.Green);
            }
            else if (x == Direction.Positive && y == Direction.Newtral)
            {
                SetColor(LED7C.Color.Cyan);
            }
            else if (x == Direction.Negative && y == Direction.Positive)
            {
                SetColor(LED7C.Color.Blue);
            }
            else if (x == Direction.Newtral && y == Direction.Positive)
            {
                SetColor(LED7C.Color.Magenta);
            }
            else if (x == Direction.Positive && y == Direction.Positive)
            {
                SetColor(LED7C.Color.Yellow);
            }
        }

        void bluetoothSmart_DataReceived(string val)
        {
            switch (val)
            {
                case "1": case "b": SetColor(LED7C.Color.Blue); break;
                case "2": case "g": SetColor(LED7C.Color.Green); break;
                case "3": case "c": SetColor(LED7C.Color.Cyan); break;
                case "4": case "r": SetColor(LED7C.Color.Red); break;
                case "5": case "m": SetColor(LED7C.Color.Magenta); break;
                case "6": case "y": SetColor(LED7C.Color.Yellow); break;
                case "7": case "w": SetColor(LED7C.Color.White); break;
                default:
                    {
                        SetColor(LED7C.Color.Off);
                        break;
                    } 
            }
        }

        void SetColor(LED7C.Color color)
        {
            Debug.Print("Set color to" + color.ToString());
            led7C.SetColor(color);
            bluetoothSmart.SendString(color.ToString(), false);
        }
    }
}
