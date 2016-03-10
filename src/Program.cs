using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PPS
{
    public class Program
    {
        private const int SwHide = 0;
        private const uint SpiGetmouse = 0x0003;
        private const uint SpiSetmouse = 0x0004;        

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo", SetLastError = true)]
        public static extern bool SystemParametersInfoGet(uint action, uint param, IntPtr vparam, Spif fWinIni);        

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo", SetLastError = true)]
        public static extern bool SystemParametersInfoSet(uint action, uint param, IntPtr vparam, Spif fWinIni);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        static void Main(string[] args)
        {
            var handle = GetConsoleWindow();            
            ShowWindow(handle, SwHide);

            var mouseParams = GetCurrentMouseParams();
            mouseParams[2] = IsPointerPrecisionEnabled(mouseParams) ? 0 : 1;
            SystemParametersInfoSet(SpiSetmouse, 0, GCHandle.Alloc(mouseParams, GCHandleType.Pinned).AddrOfPinnedObject(), Spif.SpifSendchange);

            var onOffMessage = IsPointerPrecisionEnabled(mouseParams) ? "on" : "off";
            MessageBox.Show(onOffMessage, "Enhance pointer precision");
        }

        private static int[] GetCurrentMouseParams()
        {
            var mouseParams = new int[3];
            SystemParametersInfoGet(SpiGetmouse, 0, GCHandle.Alloc(mouseParams, GCHandleType.Pinned).AddrOfPinnedObject(), 0);

            return mouseParams;
        }

        private static bool IsPointerPrecisionEnabled(IReadOnlyList<int> mouseParams)
        {
            return mouseParams[2] != 0;
        }
    }
}
