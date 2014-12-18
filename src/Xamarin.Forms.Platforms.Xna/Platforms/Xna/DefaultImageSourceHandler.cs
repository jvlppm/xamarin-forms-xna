[assembly: Xamarin.Forms.Platforms.Xna.ExportImageSourceHandler(
    typeof(Xamarin.Forms.StreamImageSource),
    typeof(Xamarin.Forms.Platforms.Xna.DefaultImageSourceHandler))]
[assembly: Xamarin.Forms.Platforms.Xna.ExportImageSourceHandler(
    typeof(Xamarin.Forms.UriImageSource),
    typeof(Xamarin.Forms.Platforms.Xna.DefaultImageSourceHandler))]
#if !PORTABLE
[assembly: Xamarin.Forms.Platforms.Xna.ExportImageSourceHandler(
    typeof(Xamarin.Forms.FileImageSource),
    typeof(Xamarin.Forms.Platforms.Xna.DefaultImageSourceHandler))]
#endif
namespace Xamarin.Forms.Platforms.Xna
{
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
    using XnaColor = Microsoft.Xna.Framework.Color;
    using System.Text.RegularExpressions;

    public class DefaultImageSourceHandler : IImageSourceHandler
    {
        public Task<IRenderElement> LoadImageAsync(ImageSource imageSource, CancellationToken cancellationToken)
        {
            return imageSource.LoadAsync(cancellationToken);
        }
    }
}
