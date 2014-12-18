using System.Collections.Generic;
using Xamarin.Forms.Platforms.Xna.Renderers;
using System.Linq;

namespace Xamarin.Forms.Platforms.Xna
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Threading.Tasks;
    using System.Threading;
    using System.IO;
    using System;
    using System.Text.RegularExpressions;
    using XnaColor = Microsoft.Xna.Framework.Color;

    public static class Extensions
    {
        class TextureImageSource : IRenderElement
        {
            Texture2D _texture;

            public TextureImageSource(Texture2D texture)
            {
                _texture = texture;
            }

            public SizeRequest Measure(Size availableSize, SizeRequest contentSize)
            {
                return new SizeRequest(new Size(_texture.Width, _texture.Height), default(Size));
            }

            public void Draw(SpriteBatch spriteBatch, Rectangle area, XnaColor color)
            {
                spriteBatch.Draw(_texture, area, color);
            }


            public Rectangle GetContentArea(Rectangle area)
            {
                return area;
            }
        }

        public static Color ToXnaColor(this Xamarin.Forms.Color color)
        {
            return new Color((float)color.R, (float)color.G, (float)color.B, (float)color.A);
        }

        public static Xamarin.Forms.Color ToXFormsColor(this Color color)
        {
            return new Xamarin.Forms.Color((float)color.R / 255, (float)color.G / 255, (float)color.B / 255, (float)color.A / 255);
        }

        public static Rectangle ToXnaRectangle(this Xamarin.Forms.Rectangle rectangle)
        {
            return new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
        }

        public static Xamarin.Forms.Rectangle ToXFormsRectangle(this Rectangle rectangle)
        {
            return new Xamarin.Forms.Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public static Plane Transform(this Plane plane, Matrix matrix)
        {
            Plane p;
            Matrix m;
            Matrix.Invert(ref matrix, out m);
            float x = plane.Normal.X;
            float y = plane.Normal.Y;
            float z = plane.Normal.Z;
            float d = plane.D;

            p.Normal.X = (x * m.M11) + (y * m.M12) + (z * m.M13) + (d * m.M14);
            p.Normal.Y = (x * m.M21) + (y * m.M22) + (z * m.M23) + (d * m.M24);
            p.Normal.Z = (x * m.M31) + (y * m.M32) + (z * m.M33) + (d * m.M34);
            p.D = (x * m.M41) + (y * m.M42) + (z * m.M43) + (d * m.M44);
            return p;
        }

        public static IEnumerable<VisualElementRenderer> FlattenHierarchy(this VisualElementRenderer renderer)
        {
            yield return renderer;

            foreach (var child in renderer.Children)
                foreach (var sub in FlattenHierarchy(child))
                    yield return sub;
        }

        public static IEnumerable<VisualElementRenderer> FlattenHierarchyReverse(this VisualElementRenderer renderer)
        {
            foreach (var child in renderer.Children.Reverse())
                foreach (var sub in FlattenHierarchyReverse(child))
                    yield return sub;

            yield return renderer;
        }

        public static void Draw(this SpriteBatch spriteBatch, NinePatch ninePatch, Rectangle rectangle, Color color)
        {
            ninePatch.Draw(spriteBatch, rectangle, color);
        }

        public static async Task<IRenderElement> LoadAsync(this ImageSource imageSource, CancellationToken cancellationToken)
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

            var texture = getTexture != null ? await getTexture : Texture2D.FromStream(Forms.Game.GraphicsDevice, await getStream);

            if (Regex.IsMatch(fileSource.File, @"\.9(\.[^\.]+)?$"))
                return new NinePatch(texture);

            return new TextureImageSource(texture);
        }
    }
}
