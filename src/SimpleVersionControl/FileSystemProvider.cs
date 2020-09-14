// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;

namespace SimpleVersionControl
{
    public class FileSystemProvider : IFileProvider, IFileSaver
    {
        public FileSystemProvider(string rootPath)
        {
            RootPath = rootPath;
        }

        public string RootPath { get; set; }

        public async Task<string> GetFile(string relativePath)
        {
            await Task.CompletedTask;

            string completePath = Path.Combine(RootPath, relativePath);
            if (!File.Exists(completePath))
            {
                return null;
            }

            try
            {
                StreamReader reader = new StreamReader(completePath);
                string contents = reader.ReadToEnd();
                reader.Close();
                return contents;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> SaveFile(string relativePath, string content)
        {
            await Task.CompletedTask;

            if (content == null)
            {
                return false;
            }

            string completePath = Path.Combine(RootPath, relativePath);

            try
            {
                string dir = Path.GetDirectoryName(completePath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                StreamWriter writer = new StreamWriter(completePath, false);
                writer.Write(content);
                writer.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> PublishFromTemp()
        {
            if (!(await ClearDirectory(RootPath, false)))
            {
                return false;
            }

            string tempPath = Path.Combine(RootPath, "temp");
            if (!Directory.Exists(tempPath))
            {
                return true;
            }

            DirectoryInfo dirInfo = new DirectoryInfo(tempPath);

            foreach (FileInfo fileInfo in dirInfo.EnumerateFiles())
            {
                string outpath = Path.Combine(RootPath, GetRelativePath(fileInfo.FullName, tempPath));
                fileInfo.MoveTo(outpath);
            }

            foreach (DirectoryInfo subdirInfo in dirInfo.EnumerateDirectories())
            {
                string outpath = Path.Combine(RootPath, GetRelativePath(subdirInfo.FullName, tempPath));
                subdirInfo.MoveTo(outpath);
            }

            Directory.Delete(tempPath);
            return true;
        }

        public async Task CancelPublish()
        {
            await Task.CompletedTask;

            string tempPath = Path.Combine(RootPath, "temp");
            if (!Directory.Exists(tempPath))
            {
                return;
            }

            Directory.Delete(tempPath);
        }

        private async Task<bool> ClearDirectory(string relativePath, bool removeEmptyDirectory)
        {
            await Task.CompletedTask;
            string dir = Path.Combine(RootPath, relativePath);

            if (!Directory.Exists(dir))
            {
                return false;
            }

            try
            {
                bool tempFound = false;
                DirectoryInfo dirInfo = new DirectoryInfo(dir);

                foreach (FileInfo fileInfo in dirInfo.EnumerateFiles())
                {
                    fileInfo.Delete();
                }

                foreach (DirectoryInfo subdirInfo in dirInfo.EnumerateDirectories())
                {
                    if (subdirInfo.Name == "temp")
                    {
                        tempFound = true;
                        continue;
                    }

                    subdirInfo.Delete(true);
                }

                if (removeEmptyDirectory && !tempFound)
                {
                    dirInfo.Delete();
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private string GetRelativePath(string filespec, string folder)
        {
            Uri pathUri = new Uri(filespec);

            // Folders must end in a slash
            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                folder += Path.DirectorySeparatorChar;
            }

            Uri folderUri = new Uri(folder);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
