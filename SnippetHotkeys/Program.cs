using System;
using System.Windows.Forms;

namespace SnippetHotkeys
{
    internal static class AppInfo
    {
        public const string Version = "1.3";
    }

    internal static class Program
    {
        /// ***************************************************************** ///
        /// Function:   Main
        /// Summary:    Application entry point
        /// Returns:    
        /// ***************************************************************** ///
        [STAThread]
        static void Main()
        {
            // Initialize WinForms environment (visual styles, DPI settings, etc.)
            ApplicationConfiguration.Initialize();

            // Start the application with the main window
            // This begins the WinForms message loop
            Application.Run(new SnippetHotkeysMW());
        }
    }
}