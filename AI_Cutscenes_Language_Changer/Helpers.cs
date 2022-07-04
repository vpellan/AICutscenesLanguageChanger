using System.Diagnostics;

namespace AI_Cutscenes_Language_Changer
{
    public static class Helpers
    {
        public static void ExecuteProcess(string fileName, string arguments, bool redirect=false)
        {
            var startInfo = new ProcessStartInfo()
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = redirect,
                UseShellExecute = false,
            };

            var process = new Process
            {
                StartInfo = startInfo
            };

            process.Start();
            process.WaitForExit();
        }
    }
}