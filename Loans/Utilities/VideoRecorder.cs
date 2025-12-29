using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace ePACSLoans.Utilities
{
    public class VideoRecorder
    {
        private readonly string _videoDirectory;

        public VideoRecorder(string videoDir)
        {
            _videoDirectory = videoDir;

            if (!Directory.Exists(_videoDirectory))
                Directory.CreateDirectory(_videoDirectory);
        }

        public async Task SaveVideo(IPage page, string testName)
        {
            var video = page?.Video;
            if (video == null)
                return;

            string path = await video.PathAsync();
            string newPath = Path.Combine(_videoDirectory, $"{testName}.webm");

            File.Move(path, newPath, overwrite: true);
        }
    }
}
