using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace SeekAndArchive
{
    class Program
    {
        static List<FileInfo> foundFiles;
        static List<FileSystemWatcher> watchers;
        static List<DirectoryInfo> archiveDirs;

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

        // We identify the sender of the event.
        // This will determine, which file has been changed. 
        // After that we find the index of that file, 
        // and call the previous ArchiveFile method on them. 
        static void WatcherChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                Console.WriteLine("{0} has been changed.", e.FullPath);

                FileSystemWatcher senderWatcher = (FileSystemWatcher)sender;
                int index = watchers.IndexOf(senderWatcher, 0);
                //now that we have the index, we can archive the file 
                ArchiveFile(archiveDirs[index], foundFiles[index]);
            }
        }


        static void ArchiveFile(DirectoryInfo archiveDir, FileInfo fileToArchive)
        {
            FileStream input = fileToArchive.OpenRead();
            FileStream output = File.Create(archiveDir.FullName + @"\" + fileToArchive.Name + ".gz");
            GZipStream Compressor = new GZipStream(output, CompressionMode.Compress);
            int b = input.ReadByte();
            while (b != -1)
            {
                Compressor.WriteByte((byte)b);

                b = input.ReadByte();
            }
            Compressor.Close();
            input.Close();
            output.Close();
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

            // archive files
            archiveDirs = new List<DirectoryInfo>();
            //create archive directories
            for (int i = 0; i < foundFiles.Count; i++)
            {
                archiveDirs.Add(Directory.CreateDirectory("archive" + i.ToString()));
            }




        }
    }
}
