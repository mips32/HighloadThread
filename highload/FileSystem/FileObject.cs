namespace highload.FileSystem
{
    public class FileObject : IFileObject
    {
        public string Path { get; private set; }
        public long Size { get; private set; }

        private FileObject()
        {
        }

        public FileObject(string path, long size) : this()
        {
            this.Path = path;
            this.Size = size;
        }

    }
    
}