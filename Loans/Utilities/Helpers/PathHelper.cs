using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePACSLoans.Utilities.Helpers
{
    public class PathHelper
    {
        public string getProjectPath()
        {
            string currentDir = Directory.GetCurrentDirectory();
            string projectPath = Path.GetFullPath(Path.Combine(currentDir, "..", "..", ".."));
            return projectPath;
        }
    }
}
