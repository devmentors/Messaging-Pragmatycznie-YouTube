using System.Collections.Concurrent;

namespace Filo.Services.Archive.Storage;

public sealed class FilesStorage
{
    private readonly ConcurrentDictionary<string, string> _files = new();

    public void AddFile(string fileName, string absolutePath)
    {
        var isSucceeded = _files.TryAdd(fileName, absolutePath);

        if (!isSucceeded)
        {
            throw new InvalidOperationException($"File {fileName} already exists");
        }
    }
    
    public void RenameFile(string oldFileName, string newFileName)
    {
        var isSucceeded = _files.TryGetValue(oldFileName, out var absolutePath);

        if (!isSucceeded)
        {
            throw new InvalidOperationException($"File {oldFileName} not exists");
        }

        _files.Remove(oldFileName, out absolutePath);
        _files.TryAdd(newFileName, absolutePath);
    }
}