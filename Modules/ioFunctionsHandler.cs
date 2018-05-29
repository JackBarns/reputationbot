using System.IO;
using System.Reflection;

namespace new_bot.Modules
{
    class ioFunctionsHandler
    {
        public bool useFileInfo()
        {
            createConfigFile(); // Doesn't create if aleady exists, handled in function
            string[] lines = File.ReadAllLines($@"{System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\cfg\sqlInfo.txt");
            foreach (string line in lines)
            {
                switch (line)
                {
                    case "useFileValues?=Y":
                        return true;
                    case "useFileValues?=N":
                        return false;
                }
            }
            //This shouldn't happen.
            return false;
        }
        public string getSqlParam(string param)
        {
            string[] lines = File.ReadAllLines($@"{System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\cfg\sqlInfo.txt");
            foreach (string line in lines)
            {
                if (line.StartsWith($"{param}="))
                {
                    return line.Substring(param.Length + 1);
                }
            }
            return "error in getSqlParam";
        }
        public void createConfigFile()
        {
            if (!(File.Exists($@"{System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\cfg\sqlInfo.txt")))
            {
                if (!(Directory.Exists(($@"{System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\cfg"))))
                {
                    Directory.CreateDirectory(($@"{System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\cfg"));
                }
                File.Create($@"{System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\cfg\sqlInfo.txt");
            }
        }
    }
}
