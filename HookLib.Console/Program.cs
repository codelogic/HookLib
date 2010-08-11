using System;
using System.Linq;
using HookLib;
using HookLib.Windows;

namespace TimeSlice.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var mgr = new HookManager();
            mgr.KeyDown += GlobalKeyDown;
            mgr.MouseMove += GlobalMouseMove;

            System.Console.WriteLine("Connected.");

            var messagePump = new WindowsMessagePump();
            messagePump.Run();

            System.Console.WriteLine("Finished.");
            System.Console.ReadKey();
        }

        static void GlobalMouseMove(object sender, GlobalMouseEventHandlerArgs args)
        {
            System.Console.WriteLine(args.Point);
        }

        static void GlobalKeyDown(object sender, GlobalKeyEventHandlerArgs args)
        {
            if (args.ScanCode == 30)
                args.Handled = true;
            System.Console.WriteLine(args.ScanCode);
        }
    }
}