﻿[assembly: Xamarin.Forms.Platforms.Xna.Controls.ExportImageSourceHandler(
    typeof(Xamarin.Forms.StreamImageSource),
<<<<<<< Updated upstream:src/Xamarin.Forms.Platform.Xna/Platforms/Xna/Controls/ImageSource/StreamImageSourceHandler.cs
    typeof(Xamarin.Forms.Platforms.Xna.Controls.StreamImageSourceHandler))]
namespace Xamarin.Forms.Platforms.Xna.Controls
=======
    typeof(Xamarin.Forms.Platforms.Xna.Controls.Images.StreamImageSourceHandler))]
namespace Xamarin.Forms.Platforms.Xna.Controls.Images
>>>>>>> Stashed changes:src/Xamarin.Forms.Platform.Xna/Platforms/Xna/Controls/Images/StreamImageSourceHandler.cs
{
    using Context;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    class StreamImageSourceHandler : IImageSourceHandler
    {
        public async Task<IControl> GetImageAsync(ImageSource imageSource, ImageFormat format, CancellationToken cancellationToken)
        {
            var streamSorce = (StreamImageSource)imageSource;
            var stream = await Forms.UpdateContext.Wait(streamSorce.Stream(cancellationToken));
            return await ImageFactory.CreateFromStream(stream, format, cancellationToken);
        }
    }
}
