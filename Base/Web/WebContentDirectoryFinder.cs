using System;
using System.IO;
using System.Linq;
using Web.Api.Base.Extensions;

namespace Web.Api.Base.Web
{
    /// <summary>
    /// This class is used to find root path of the web project in;
    /// unit tests (to find views) and entity framework core command line commands (to find conn string).
    /// </summary>
    public static class WebContentDirectoryFinder
    {
        public static string CalculateContentRootFolder()
        {
            var baseAssemblyDirectoryName = Path.GetDirectoryName(typeof(WebContentDirectoryFinder).GetAssembly().Location);
            if (baseAssemblyDirectoryName == null)
            {
                throw new Exception("Could not find location of Base assembly!");
            }

            var directoryInfo = new DirectoryInfo(baseAssemblyDirectoryName);
            while (!DirectoryContains(directoryInfo.FullName, "Core Web App.sln"))
            {
                if (directoryInfo.Parent == null)
                {
                    throw new Exception("Could not find content root folder!");
                }

                directoryInfo = directoryInfo.Parent;
            }

            var webMvcFolder = Path.Combine(directoryInfo.FullName, "Client");
            if (Directory.Exists(webMvcFolder))
            {
                return webMvcFolder;
            }

            var webHostFolder = Path.Combine(directoryInfo.FullName, "Data");
            if (Directory.Exists(webHostFolder))
            {
                return webHostFolder;
            }

            throw new Exception("Could not find root folder of the web project!");
        }

        private static bool DirectoryContains(string directory, string fileName)
        {
            return Directory.GetFiles(directory).Any(filePath => string.Equals(Path.GetFileName(filePath), fileName));
        }
    }
}