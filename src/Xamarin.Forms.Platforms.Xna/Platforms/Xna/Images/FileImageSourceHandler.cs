[assembly: Xamarin.Forms.Platforms.Xna.Images.ExportImageSourceHandler(
    typeof(Xamarin.Forms.FileImageSource),
    typeof(Xamarin.Forms.Platforms.Xna.Images.FileImageSourceHandler))]
namespace Xamarin.Forms.Platforms.Xna.Images
{
    using Controls;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    public class FileImageSourceHandler : IImageSourceHandler
    {
        readonly static Dictionary<string, IControl> _cachedImages = new Dictionary<string, IControl>();

        public async Task<IControl> GetImageAsync(ImageSource imageSource, ImageFormat format, CancellationToken cancellationToken)
        {
            var fileSource = (FileImageSource)imageSource;

            IControl cached;
            if (_cachedImages.TryGetValue(fileSource.File, out cached))
                return cached;

            var path = Path.Combine(Forms.Game.Content.RootDirectory, fileSource.File);
            if (format == ImageFormat.Unknown)
                format = ImageFactory.DetectFormat(fileSource.File);

            IControl image;
            if (File.Exists(path))
                image = await ImageFactory.CreateFromStream(File.OpenRead(path), format, cancellationToken);
            else
            {
                var texture = Forms.Game.Content.Load<Texture2D>(fileSource.File);
                image = ImageFactory.CreateFromTexture(texture, format);
            }

            _cachedImages.Add(fileSource.File, image);
            return image;
        }
    }
}
