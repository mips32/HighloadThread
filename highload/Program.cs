using System;
using System.Collections.Generic;
using System.Linq;
using highload.FileSystem;
using highload.Settings;
using highload.Threading;

namespace highload
{
    static class Program
    {

        static void Main(string[] args)
        {
            // Read settings
            CurrentSettings settings = CurrentSettings.ParseSettings(args);
            if (settings == null)
            {
                return;
            }
            
            Console.WriteLine("Path: \"{0}\"", settings.DataPath);
            Console.WriteLine("Number of threads: \"{0}\"", settings.NumberOfThreads);
            Console.WriteLine("Minimum word length: \"{0}\"", settings.MinimumWordLength);

            // Read folder info
            IFolderObject folder = FolderObject.ReadFilesInfo(settings.DataPath);

            // Parse files
            ThreadManager tm = new ThreadManager(settings, folder);
            List<KeyValuePair<string, int>> res = tm.Go();

            for (int i = 0; i < res.Count(); i++)
            {
                Console.WriteLine("{0} - {1}", res[i].Key, res[i].Value);
            }

        }
        
    }

}