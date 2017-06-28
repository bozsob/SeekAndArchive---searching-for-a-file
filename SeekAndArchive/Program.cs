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
            Console.ReadKey();
        }
    }
}
