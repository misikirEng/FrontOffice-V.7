using CNET.API.Manager;
using ProcessManager;

namespace ERP.EventManagement
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //To customize application configuration such as set high DPI settings or default font,
            //see https://aka.ms/applicationconfiguration.
            //ApplicationConfiguration.Initialize();

            HttpSinglton.BaseAddressValue = "http://localhost:5181";
            LocalBuffer.LocalBuffer.LoadLocalBuffer();
            LocalBuffer.LocalBuffer.CurrentLoggedInUser = LocalBuffer.LocalBuffer.UserBufferList.FirstOrDefault(x => x.Id == 71);

            //Application.Run(new EventMgtForm());
        }
    }
}