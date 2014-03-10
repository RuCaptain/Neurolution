using System;
using System.Runtime.InteropServices;

namespace Neurolution.Helpers
{
    class Winapi
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public static bool IsActive(IntPtr handle)
        {
            var activeHandle = GetForegroundWindow();
            return (activeHandle == handle);
        }
    }
}
