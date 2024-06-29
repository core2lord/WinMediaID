using Microsoft.UI.Dispatching;
using System;
using System.Diagnostics;

namespace WinMediaID
{
    static class MediaIdCommand
    {
        private static DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private static GlobalProperties globalProperties = App.Properties;

        public static void Run(string validatedSourcePath)
        {
            try
            {
                // create the ProcessStartInfo Incidentally, /c tells
                // WindowsConsole that we want it to execute the command that
                // follows, and then exit.
                ProcessStartInfo proccessStartCommandString =
                new("cmd", "/c " + $"DISM /Get-WimInfo /wimfile:\"{validatedSourcePath}\"")
                {
                    // The following commands are needed to redirect the
                    // standard output to Process.StandardOutput StreamReader.
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    ErrorDialog = true,
                    WindowStyle = ProcessWindowStyle.Hidden,

                    // Do not create a new Console Window.
                    CreateNoWindow = true,
                };

                // Now we create a process, assign its ProcessStartInfo and
                // start it
                using Process activeProcess = new()
                {
                    StartInfo = proccessStartCommandString
                };
                activeProcess.Start();

                // Get the output into a string
                string result = activeProcess.StandardOutput.ReadToEnd();

                // Display the command output.
       //         WriteStatusUpdate(result, "Finished", "Results shown in window.", true);
            }
            catch (Exception)
            {
           //     WriteStatus.Update($"{e.InnerException}\n{e.Message}", "Error", "Error Occured", true);
            }
        }

    }
}