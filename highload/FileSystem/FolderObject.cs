using System.Collections.Generic;
using System.IO;

namespace highload.FileSystem
{
    public class FolderObject : IFolderObject
    {
        public string SourcePath { get; private set; }

        public List<IFileObject> DataFiles { get; private set; }

        private FolderObject()
        {
            this.DataFiles = new List<IFileObject>(1024);
        }

        private FolderObject(string sourcePath) : this()
        {
            this.SourcePath = sourcePath;
        }

        public void AddSubfolder(IFolderObject folderObject)
        {
            foreach (IFileObject fileObject in folderObject.DataFiles)
            {
                this.DataFiles.Add(fileObject);
            }
        }

        public static IFolderObject ReadFilesInfo(string rootSourcePath)
        {
            FolderObject res = new FolderObject(rootSourcePath);

            DirectoryInfo dir = new DirectoryInfo(rootSourcePath);
            FileInfo[] files = dir.GetFiles("*.txt");

            foreach (FileInfo file in files)
            {
                IFileObject fileObject = new FileObject(file.FullName, file.Length);
                res.DataFiles.Add(fileObject);
            }
            
            foreach (string directoryPath in Directory.GetDirectories(rootSourcePath))
            {
                res.AddSubfolder(ReadFilesInfo(directoryPath));
            }

            return res;
        }


    }
}