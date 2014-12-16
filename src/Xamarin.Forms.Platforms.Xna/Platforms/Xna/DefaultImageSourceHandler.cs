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
        class TextureImageSource : IRenderElement
        {
            Texture2D _texture;

            public TextureImageSource(Texture2D texture)
            {
                _texture = texture;
            }

            public SizeRequest Measure(Size availableSize)
            {
                return new SizeRequest(new Size(_texture.Width, _texture.Height), default(Size));
            }

            public void Draw(SpriteBatch spriteBatch, XnaRectangle area)
            {
                spriteBatch.Draw(_texture, area, XnaColor.White);
            }
        }

        public async Task<IRenderElement> LoadImageAsync(ImageSource imageSource, CancellationToken cancellationToken)
        {
            Task<Stream> getStream = null;
            Task<Texture2D> getTexture = null;

            var streamSource = imageSource as StreamImageSource;
            var uriSource = imageSource as UriImageSource;

            var fileSource = imageSource as FileImageSource;
            if (fileSource != null)
            {
#if !PORTABLE
                if (File.Exists(fileSource.File))
                    getStream = Task.FromResult((Stream)File.OpenRead(fileSource.File));
                else
#endif
                {
                    getTexture = Task.FromResult(Forms.Game.Content.Load<Texture2D>(fileSource.File));
                }
            }
            else if (streamSource != null)
                getStream = streamSource.Stream(cancellationToken);
            else if (uriSource != null)
            {
                var uri = uriSource.Uri;
                if (uri.Scheme == "content")
                {
                    if (uri.Host != string.Empty && uri.Host != "localhost")
                        throw new ArgumentException("Unsupported image source HOST " + uri.Host + ". Did you mean content:///?", "imageSource");

                    var asset = uriSource.Uri.PathAndQuery.TrimStart('/');
                    getTexture = Task.FromResult(Forms.Game.Content.Load<Texture2D>(asset));
                }
                else
                    getStream = uriSource.GetStreamAsync(cancellationToken);
            }

            if (getTexture == null && getStream == null)
                throw new InvalidOperationException("Not supported image source");

            var texture = getTexture != null? await getTexture : Texture2D.FromStream(Forms.Game.GraphicsDevice, await getStream);

            if (Regex.IsMatch(fileSource.File, @"\.9(\.[^\.]+)?$"))
                return new NinePatch(texture);

            return new TextureImageSource(texture);
        }
    }
}
