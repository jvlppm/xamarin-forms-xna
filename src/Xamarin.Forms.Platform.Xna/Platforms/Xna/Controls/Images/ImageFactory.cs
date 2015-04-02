namespace Xamarin.Forms.Platforms.Xna.Controls.Images
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
                    PreMultiplyAlphas(texture);
                    return CreateFromTexture(texture, format);
                default:
                    throw new ArgumentException("Invalid image format", "format");
            }
        }

        private static void PreMultiplyAlphas(Texture2D texture)
        {
            var data = new Microsoft.Xna.Framework.Color[texture.Width * texture.Height];
            texture.GetData(data);
            for (int i = 0; i != data.Length; ++i)
                data[i] = Microsoft.Xna.Framework.Color.FromNonPremultiplied(data[i].ToVector4());
            texture.SetData(data);
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
