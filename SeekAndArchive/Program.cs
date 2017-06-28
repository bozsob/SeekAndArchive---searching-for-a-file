using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SeekAndArchive
{
    class Program
    {
        static List<FileInfo> foundFiles;
        static List<FileSystemWatcher> watchers;

        static void RecursiveSearch
            (List<FileInfo> foundFiles, string fileName, DirectoryInfo currentDirectory)
        {
            // enumerate all the files in the current directory
            foreach (FileInfo file in currentDirectory.GetFiles())
            {
                if (file.Name == fileName)
                {
                    foundFiles.Add(file);
                }
            }
            // continue the search recursively
            foreach (DirectoryInfo dir in currentDirectory.GetDirectories())
            {
                RecursiveSearch(foundFiles, fileName, dir);
            }
        }

        
            // for file watching we need a method to handle the change events
            static void WatcherChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                Console.WriteLine("{0} has been changed.", e.FullPath);
            }
        }



        static void Main(string[] args)
        {

            string fileName = args[0];
            string directoryName = args[1];

            foundFiles = new List<FileInfo>();

            // check if the directory exists
            DirectoryInfo rootDir = new DirectoryInfo(directoryName);

            if (!rootDir.Exists)
            {
                Console.WriteLine("The specified directory does not exist.");
                return;
            }
            // search recursively for the mathing files
            RecursiveSearch(foundFiles, fileName, rootDir);

            // list the found files
            Console.WriteLine("Found {0} files", foundFiles.Count);
            foreach(FileInfo file in foundFiles)
            {
                Console.WriteLine("{0}", file.FullName);
            }
            Console.ReadLine();

            // File WATCHING
            watchers = new List<FileSystemWatcher>();

            foreach (FileInfo file in foundFiles)
            {
                FileSystemWatcher newWatcher = new FileSystemWatcher(file.DirectoryName, file.Name);
                newWatcher.Changed += new FileSystemEventHandler(WatcherChanged);
                newWatcher.EnableRaisingEvents = true;
                watchers.Add(newWatcher);
            }


        }
    }
}
