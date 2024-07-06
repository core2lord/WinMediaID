using System;
using System.Diagnostics;
using System.Text;

namespace WinMediaID
{
    static class MediaIdCommand
    {
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

                var trimmedResult = result.Remove(0, 122);

                StringBuilder stringBuilder = new();
                stringBuilder.AppendLine("Index,Name,Size,");
                string[] splitStrings = trimmedResult.Split(":", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var item in splitStrings)
                {
                    UIStatus.UpdateConsoleText(item);
                }

                // Display the command output.
                // UIStatus.UpdateAllText(trimmedResult, "Results shown in window.", "Finished");
                UIStatus.ProgressRing.Stop();
            }
            catch (Exception e)
            {
                e = new();
                UIStatus.UpdateAllText($"{e.InnerException}\n{e.Message}", "Error", "Error Occured", true);
            }
        }
    }
}