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
        Texture2D _image;
        Microsoft.Xna.Framework.Rectangle _renderArea;

        public ImageRenderer()
        {
            PropertyTracker.AddHandler(Image.SourceProperty, Handle_Source);
        }

        public override SizeRequest Measure(Size availableSize)
        {
            if (_image == null || !IsVisible)
                return default(SizeRequest);

            if (double.IsPositiveInfinity(availableSize.Width))
                availableSize.Width = _image.Width;
            if (double.IsPositiveInfinity(availableSize.Height))
                availableSize.Height = _image.Height;

            switch (Model.Aspect)
            {
                case Aspect.Fill:
                    return new SizeRequest(availableSize, default(Size));
                case Aspect.AspectFit:
                    var scaleFit = Math.Min(availableSize.Width / (float)_image.Width, availableSize.Height / (float)_image.Height);
                    return new SizeRequest(new Size(_image.Width * scaleFit, _image.Height * scaleFit), default(Size));
                case Aspect.AspectFill:
                    var scaleFill = Math.Max(availableSize.Width / (float)_image.Width, availableSize.Height / (float)_image.Height);
                    return new SizeRequest(new Size(_image.Width * scaleFill, _image.Height * scaleFill), default(Size));
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
            if (_image == null)
                return;
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
                    _image = await handler.LoadImageAsync(Model.Source, _imageLoadCancellation.Token);
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
