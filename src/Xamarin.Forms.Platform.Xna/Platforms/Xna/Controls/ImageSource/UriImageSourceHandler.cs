[assembly: Xamarin.Forms.Platforms.Xna.Controls.ExportImageSourceHandler(
    typeof(Xamarin.Forms.UriImageSource),
    typeof(Xamarin.Forms.Platforms.Xna.Controls.UriImageSourceHandler))]
namespace Xamarin.Forms.Platforms.Xna.Controls
{
    using Context;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    class UriImageSourceHandler : IImageSourceHandler
    {
        static readonly ConcurrentDictionary<string, Task<IControl>> _cachedImages = new ConcurrentDictionary<string, Task<IControl>>();

        public Task<IControl> GetImageAsync(ImageSource imageSource, ImageFormat format, CancellationToken cancellationToken)
        {
            var uriSource = (UriImageSource)imageSource;
            if (uriSource.Uri.Scheme == "pack")
                uriSource.CachingEnabled = false;
            string asset = uriSource.Uri.ToString();

            if (format == ImageFormat.Unknown)
                format = ImageFactory.DetectFormat(asset);

            string cacheKey = format.ToString() + "|" + asset;

            return _cachedImages.GetOrAdd(cacheKey, k => GetImage(uriSource, format));
        }

        public async Task<IControl> GetImage(UriImageSource source, ImageFormat format)
        {
            using (var stream = await Forms.UpdateContext.Wait(source.GetStreamAsync()))
            {
                if (stream == null)
                    throw new ArgumentException("Resource not found", "source");
                return await ImageFactory.CreateFromStream(stream, format, CancellationToken.None);
            }
        }
    }
}
