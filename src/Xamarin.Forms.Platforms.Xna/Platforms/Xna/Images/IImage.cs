namespace Xamarin.Forms.Platforms.Xna.Images
{
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using XnaColor = Microsoft.Xna.Framework.Color;
    using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

    public interface IImage
    {
        SizeRequest Measure(ISet<State> states, Size availableSize, SizeRequest contentSize);
        void Draw(ISet<State> states, SpriteBatch spriteBatch, XnaRectangle area, XnaColor color);
        XnaRectangle GetContentArea(ISet<State> states, XnaRectangle area);
    }

    public static class ImageFactory
    {
        public static async Task<IImage> CreateFromStream(Stream stream, ImageFormat format, CancellationToken cancellationToken)
        {
            switch (format)
            {
                case ImageFormat.StateList:
                    return await StateList.FromXml(new XmlTextReader(stream), cancellationToken);
                case ImageFormat.Unknown:
                case ImageFormat.Default:
                case ImageFormat.NinePatch:
                    var texture = Texture2D.FromStream(Forms.Game.GraphicsDevice, stream);
                    return CreateFromTexture(texture, format);
                default:
                    throw new ArgumentException("Invalid image format", "format");
            }
        }

        public static IImage CreateFromTexture(Texture2D texture, ImageFormat format)
        {
            switch (format)
            {
                case ImageFormat.NinePatch:
                    return new NinePatch(texture);

                case ImageFormat.Unknown:
                case ImageFormat.Default:
                    return new SimpleImage(texture);
                default:
                    throw new ArgumentException("Invalid image format", "format");
            }
        }

        public static ImageFormat DetectFormat(string assetName)
        {
            if (assetName.EndsWith(".xml"))
                return ImageFormat.StateList;
            if (Regex.IsMatch(assetName, @"\.9(\.[^\.]+)?"))
                return ImageFormat.NinePatch;
            return ImageFormat.Unknown;
        }
    }
}
