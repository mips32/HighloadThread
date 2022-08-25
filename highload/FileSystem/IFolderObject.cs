using System.Collections.Generic;

namespace highload.FileSystem
{
    public interface IFolderObject
    {
        List<IFileObject> DataFiles { get; }
    }
}