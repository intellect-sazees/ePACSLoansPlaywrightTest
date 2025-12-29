using System;
using System.Diagnostics;
using System.IO;

namespace ePACSLoans.Utilities
{
    public static class AllureReportGenerator
    {
        static string currentDir = Directory.GetCurrentDirectory();
        static string basePath = Path.GetFullPath(Path.Combine(currentDir, "..", "..", ".."));
        private static string FinalDir = Path.Combine(basePath, "TestReports", "TestOutputReport");
        //private const string FinalDir = @"D:\ePACSLoans\Test Reports\Test Output Report\";

        public static void GenerateHtmlReport()
        {
            if (!Directory.Exists(FinalDir))
                Directory.CreateDirectory(FinalDir);

            var baseDir = AppContext.BaseDirectory;
            var resultsDir = Path.Combine(baseDir, "allure-results");
            var reportDir = Path.Combine(baseDir, "allure-report");

            if (!Directory.Exists(resultsDir))
                throw new Exception($"Allure results directory not found: {resultsDir}");

            var command = $"allure generate \"{resultsDir}\" --clean -o \"{reportDir}\"";
            ExecuteCommand(command);

            var indexFile = Path.Combine(reportDir, "index.html");
            if (!File.Exists(indexFile))
                throw new Exception("Allure report generation failed. index.html not found.");

            var outputFile = Path.Combine(
                FinalDir,
                $"TestOutputReport_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.html"
            );
            File.Copy(indexFile, outputFile, true);
        }



        private static void ExecuteCommand(string command)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", $"/c {command}")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using Process process = Process.Start(psi)!;

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    //throw new Exception($"Allure command failed: {error}");
                }
            }
            catch(Exception ex)
            {

            }
        }
    }
}
