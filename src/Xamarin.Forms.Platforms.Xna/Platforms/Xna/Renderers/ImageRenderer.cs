[assembly: Xamarin.Forms.Platforms.Xna.ExportRenderer(
    typeof(Xamarin.Forms.Image),
    typeof(Xamarin.Forms.Platforms.Xna.Renderers.ImageRenderer))]
namespace Xamarin.Forms.Platforms.Xna.Renderers
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;

    public class ImageRenderer : VisualElementRenderer<Image>
    {
        CancellationTokenSource _imageLoadCancellation;
        IImageSource _imageSource;
        Texture2D _image;
        Microsoft.Xna.Framework.Rectangle _renderArea;

        public ImageRenderer()
        {
            PropertyTracker.AddHandler(Image.SourceProperty, Handle_Source);
        }

        public override SizeRequest Measure(Size availableSize)
        {
            if (_imageSource == null || !IsVisible)
                return default(SizeRequest);

            SizeRequest measuredSize = _imageSource.Measure(availableSize);

            if (double.IsPositiveInfinity(availableSize.Width))
                availableSize.Width = measuredSize.Request.Width;
            if (double.IsPositiveInfinity(availableSize.Height))
                availableSize.Height = measuredSize.Request.Height;

            switch (Model.Aspect)
            {
                case Aspect.Fill:
                    return new SizeRequest(availableSize, measuredSize.Minimum);
                case Aspect.AspectFit:
                    var scaleFit = Math.Min(availableSize.Width / (float)measuredSize.Request.Width, availableSize.Height / (float)measuredSize.Request.Height);
                    return new SizeRequest(new Size(measuredSize.Request.Width * scaleFit, measuredSize.Request.Height * scaleFit), measuredSize.Minimum);
                case Aspect.AspectFill:
                    var scaleFill = Math.Max(availableSize.Width / (float)measuredSize.Request.Width, availableSize.Height / (float)measuredSize.Request.Height);
                    return new SizeRequest(new Size(measuredSize.Request.Width * scaleFill, measuredSize.Request.Height * scaleFill), measuredSize.Minimum);
            }

            throw new NotImplementedException();
        }

        protected override void Arrange()
        {
            base.Arrange();
            _renderArea = new Microsoft.Xna.Framework.Rectangle(0, 0, (int)Model.Bounds.Width, (int)Model.Bounds.Height);
        }

        protected override void LocalDraw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (_imageSource == null)
                return;
            if (_image == null)
                _image = _imageSource.GetImage(new Size(_renderArea.Width, _renderArea.Height));
            if (_image != null)
                SpriteBatch.Draw(_image, _renderArea, Microsoft.Xna.Framework.Color.White);
        }

        #region Property Handlers
        async void Handle_Source(BindableProperty prop)
        {
            if (_imageLoadCancellation != null)
                _imageLoadCancellation.Cancel();
            _imageLoadCancellation = new CancellationTokenSource();

            IImageSourceHandler handler = Registrar.Registered.GetHandler<IImageSourceHandler>(Model.Source.GetType());
            if (handler != null)
            {
                Model.IsLoading = true;
                try
                {
                    _imageSource = await handler.LoadImageAsync(Model.Source, _imageLoadCancellation.Token);
                    _imageLoadCancellation = null;
                    InvalidateMeasure();
                }
                catch { }
                Model.IsLoading = false;
            }
        }
        #endregion
    }
}
