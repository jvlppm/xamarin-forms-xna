[assembly: Xamarin.Forms.Platforms.Xna.Controls.ExportSourceHandler(
    typeof(Xamarin.Forms.FileImageSource),
    typeof(Xamarin.Forms.Platforms.Xna.Controls.Images.FileImageSourceHandler))]
namespace Xamarin.Forms.Platforms.Xna.Controls.Images
{
    using Microsoft.Xna.Framework.Graphics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    class FileImageSourceHandler : IImageSourceHandler
    {
        static readonly ConcurrentDictionary<string, Task<IControl>> _cachedImages = new ConcurrentDictionary<string, Task<IControl>>();

        public Task<IControl> GetImageAsync(ImageSource imageSource, ImageFormat format, CancellationToken cancellationToken)
        {
            var fileSource = (FileImageSource)imageSource;
            string asset = fileSource.File;

            if (format == ImageFormat.Unknown)
                format = ImageFactory.DetectFormat(asset);

            string cacheKey = format.ToString() + "|" + asset;

            return _cachedImages.GetOrAdd(cacheKey, k => GetImage(fileSource, format));
        }

        public async Task<IControl> GetImage(FileImageSource source, ImageFormat format)
        {
            var path = Path.Combine(Forms.Game.Content.RootDirectory, source.File);
            if (File.Exists(path))
            {
                using (var stream = File.OpenRead(path))
                    return await ImageFactory.CreateFromStream(stream, format, CancellationToken.None);
            }

            var texture = Forms.Game.Content.Load<Texture2D>(source.File);
            return ImageFactory.CreateFromTexture(texture, format);
        }
    }
}
