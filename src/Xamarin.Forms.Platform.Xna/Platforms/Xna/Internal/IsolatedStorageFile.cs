#if !PORTABLE
namespace Xamarin.Forms.Platforms.Xna
{
    using System;
    using System.Threading.Tasks;

    class IsolatedStorageFile : IIsolatedStorageFile
    {
        System.IO.IsolatedStorage.IsolatedStorageFile file;

        public IsolatedStorageFile(System.IO.IsolatedStorage.IsolatedStorageFile file)
        {
            this.file = file;
        }

        public Task CreateDirectoryAsync(string path)
        {
            return Task.Run(() => { file.CreateDirectory(path); });
        }

        public Task<bool> GetDirectoryExistsAsync(string path)
        {
            return Task.Run(() => file.DirectoryExists(path));
        }

        public Task<bool> GetFileExistsAsync(string path)
        {
            return Task.Run(() => file.FileExists(path));
        }

        public Task<DateTimeOffset> GetLastWriteTimeAsync(string path)
        {
            return Task.Run(() => file.GetLastWriteTime(path));
        }

        public Task<System.IO.Stream> OpenFileAsync(string path, FileMode mode, FileAccess access, FileShare share)
        {
            return Task.Run(() => (System.IO.Stream)file.OpenFile(path, (System.IO.FileMode)mode, (System.IO.FileAccess)access, (System.IO.FileShare)share));
        }

        public Task<System.IO.Stream> OpenFileAsync(string path, FileMode mode, FileAccess access)
        {
            return Task.Run(() => (System.IO.Stream)file.OpenFile(path, (System.IO.FileMode)mode, (System.IO.FileAccess)access));
        }
    }
}
#endif
