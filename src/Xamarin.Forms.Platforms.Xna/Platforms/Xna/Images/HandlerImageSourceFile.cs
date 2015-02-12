[assembly: Xamarin.Forms.Platforms.Xna.Images.ExportImageSourceHandler(
    typeof(Xamarin.Forms.FileImageSource),
    typeof(Xamarin.Forms.Platforms.Xna.Images.HandlerImageSourceFile))]
namespace Xamarin.Forms.Platforms.Xna.Images
{
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    public class HandlerImageSourceFile : IImageSourceHandler
    {
        // Acha o arquivo, verifica extensao, carrega Image
        // Verificar cash para resources de disco
        public async Task<IImage> GetImageAsync(ImageSource imageSource, CancellationToken cancellationToken)
        {
            var fileSource = (FileImageSource)imageSource;
            var format = ImageFactory.DetectFormat(fileSource.File);
            var path = Path.Combine(Forms.Game.Content.RootDirectory, fileSource.File);

            if (File.Exists(path))
            {
                return await ImageFactory.CreateFromStream(File.OpenRead(path), format);
            }

            var texture = Forms.Game.Content.Load<Texture2D>(fileSource.File);
            return ImageFactory.CreateFromTexture(texture, format);
        }
    }
}
