using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace HardwareSpoofer
{
    internal class Program
    {
        private static int adapternum;

        private static void SetConsoleFont(string fontName = "Lucida Console")
        {
            unsafe
            {
                var hnd = WinAPI.GetStdHandle(WinAPI.STD_OUTPUT_HANDLE);
                if (hnd != WinAPI.INVALID_HANDLE_VALUE)
                {
                    var info = new WinAPI.CONSOLE_FONT_INFO_EX();
                    info.cbSize = (uint) Marshal.SizeOf(info);
                    var newInfo = new WinAPI.CONSOLE_FONT_INFO_EX();
                    newInfo.cbSize = (uint) Marshal.SizeOf(newInfo);
                    newInfo.FontFamily = WinAPI.TMPF_TRUETYPE;
                    var ptr = new IntPtr(newInfo.FaceName);
                    Marshal.Copy(fontName.ToCharArray(), 0, ptr, fontName.Length);
                    newInfo.dwFontSize = new WinAPI.COORD(info.dwFontSize.X, info.dwFontSize.Y);
                    newInfo.FontWeight = info.FontWeight;
                    WinAPI.SetCurrentConsoleFontEx(hnd, false, ref newInfo);
                }
            }
        }

        private static void Main(string[] args)
        {
            Console.Title = "MAC Address Spoofer";
            SetConsoleFont();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("MAC Address Spoofer");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Version 1.3 | Made By Cryental");
            Console.ResetColor();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Determining Network Adapters...");
            Console.ResetColor();
            Console.WriteLine();

            var netBox = new ComboBox();
            foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces().Where(
                a => Adapter.IsValidMac(a.GetPhysicalAddress().GetAddressBytes(), true)
            ).OrderByDescending(a => a.Speed))
            {
                adapternum++;
                Console.WriteLine(new Adapter(adapter));
                netBox.Items.Add(new Adapter(adapter));
            }

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Generating and Changing to New MAC Address...");
            Console.ResetColor();
            Console.WriteLine();

            for (var i = 0; i < adapternum; i++)
            {
                netBox.SelectedIndex = i;
                var netBoxSelectedItem = netBox.SelectedItem as Adapter;
                var ss = Adapter.GetNewMac();
                if (netBoxSelectedItem.SetRegistryMac(ss))
                    Console.WriteLine("[Network " + (i + 1) + " Changed] " + ss);
                else
                    Console.WriteLine("[Network " + (i + 1) + " Not Changed] " + netBoxSelectedItem.Mac);

                Thread.Sleep(217);
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Task Finished");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}