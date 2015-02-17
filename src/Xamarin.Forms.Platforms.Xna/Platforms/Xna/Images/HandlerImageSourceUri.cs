﻿[assembly: Xamarin.Forms.Platforms.Xna.Images.ExportImageSourceHandler(
    typeof(Xamarin.Forms.UriImageSource),
    typeof(Xamarin.Forms.Platforms.Xna.Images.HandlerImageSourceUri))]
namespace Xamarin.Forms.Platforms.Xna.Images
{
    using Context;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    public class HandlerImageSourceUri : IImageSourceHandler
    {
        static readonly Dictionary<Uri, IImage> _cachedImages = new Dictionary<Uri, IImage>();

        public async Task<IImage> GetImageAsync(ImageSource imageSource, ImageFormat format, CancellationToken cancellationToken)
        {
            var uriSource = (UriImageSource)imageSource;

            IImage cached;
            if (_cachedImages.TryGetValue(uriSource.Uri, out cached))
                return cached;

            using (var stream = await Forms.UpdateContext.Wait(uriSource.GetStreamAsync(cancellationToken)))
            {
                if (stream == null)
                    throw new ArgumentException("Resource not found");
                if (format == ImageFormat.Unknown)
                    format = ImageFactory.DetectFormat(uriSource.Uri.ToString());
                var image = await ImageFactory.CreateFromStream(stream, format, cancellationToken);
                _cachedImages.Add(uriSource.Uri, image);
                return image;
            }
        }
    }
}
