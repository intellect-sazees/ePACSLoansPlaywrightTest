using Allure.Net.Commons;
using System.IO;

namespace ePACSLoans.Utilities.Helpers
{
    public static class AllureVideoHelper
    {
        static string currentDir = Directory.GetCurrentDirectory();
        static string basePath = Path.GetFullPath(Path.Combine(currentDir, "..", "..", ".."));
        private static string VideoDir = Path.Combine(basePath, "TestReports", "TestExecutionVideos");
        //private const string VideoDir = @"D:\ePACSLoans\Test Reports\Test Execution Videos\";

        public static void AttachVideo()
        {
            try
            {
                var latestVideo = GetLatestVideo();

                if (latestVideo != null)
                {
                    // Read the file as byte array
                    byte[] videoBytes = File.ReadAllBytes(latestVideo);

                    // Attach to the currently running test
                    //AllureLifecycle.Instance.AddAttachment("Test Execution Video", "video/webm", videoBytes);
                    AllureApi.AddAttachment("Test Execution Video", "video/webm", videoBytes,"webm");
                }
            }
            catch (Exception ex)
            {
            }
        }

        private static string GetLatestVideo()
        {
            try
            {
                var files = Directory.GetFiles(VideoDir, "*.webm");

                if (files.Length == 0)
                    return null;

                // Get the latest file
                return new FileInfo(files[^1]).FullName;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
