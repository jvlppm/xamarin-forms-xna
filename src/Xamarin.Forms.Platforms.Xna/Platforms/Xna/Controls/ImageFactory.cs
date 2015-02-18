namespace Xamarin.Forms.Platforms.Xna.Controls
{
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    public static class ImageFactory
    {
        public static async Task<IControl> CreateFromStream(Stream stream, ImageFormat format, CancellationToken cancellationToken)
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

        public static IControl CreateFromTexture(Texture2D texture, ImageFormat format)
        {
            switch (format)
            {
                case ImageFormat.NinePatch:
                    return new NinePatchImage(texture);

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
