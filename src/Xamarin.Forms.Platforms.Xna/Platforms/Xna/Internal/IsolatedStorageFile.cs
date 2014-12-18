#if !PORTABLE
namespace Xamarin.Forms.Platforms.Xna
{
    using System;
    using System.Threading.Tasks;

    class IsolatedStorageFile : Xamarin.Forms.IIsolatedStorageFile
    {
        System.IO.IsolatedStorage.IsolatedStorageFile file;

        public IsolatedStorageFile(System.IO.IsolatedStorage.IsolatedStorageFile file)
        {
            this.file = file;
        }

        public Task CreateDirectoryAsync(string path)
        {
            return Task.Factory.StartNew(() => { file.CreateDirectory(path); });
        }

        public System.Threading.Tasks.Task<bool> GetDirectoryExistsAsync(string path)
        {
            return Task.Factory.StartNew(() => file.DirectoryExists(path));
        }

        public System.Threading.Tasks.Task<bool> GetFileExistsAsync(string path)
        {
            return Task.Factory.StartNew(() => file.FileExists(path));
        }

        public System.Threading.Tasks.Task<DateTimeOffset> GetLastWriteTimeAsync(string path)
        {
            return Task.Factory.StartNew(() => file.GetLastWriteTime(path));
        }

        public System.Threading.Tasks.Task<System.IO.Stream> OpenFileAsync(string path, FileMode mode, FileAccess access, FileShare share)
        {
            return Task.Factory.StartNew(() => (System.IO.Stream)file.OpenFile(path, (System.IO.FileMode)mode, (System.IO.FileAccess)access, (System.IO.FileShare)share));
        }

        public System.Threading.Tasks.Task<System.IO.Stream> OpenFileAsync(string path, FileMode mode, FileAccess access)
        {
            return Task.Factory.StartNew(() => (System.IO.Stream)file.OpenFile(path, (System.IO.FileMode)mode, (System.IO.FileAccess)access));
        }
    }
}
#endif
