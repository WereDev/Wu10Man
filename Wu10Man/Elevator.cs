using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;

namespace WereDev.Utils.Wu10Man
{
    static class Elevator
    {
        public static bool IsElevated
        {
            get
            {
                return new WindowsPrincipal
                    (WindowsIdentity.GetCurrent()).IsInRole
                    (WindowsBuiltInRole.Administrator);
            }
        }

        public static void Elevate()
        {
            var info = new ProcessStartInfo(
            Assembly.GetEntryAssembly().Location)
            {
                Verb = "runas", // indicates to elevate privileges
            };

            var process = new Process
            {
                EnableRaisingEvents = true, // enable WaitForExit()
                StartInfo = info
            };

            process.Start();
            process.WaitForExit();
        }
    }
}
