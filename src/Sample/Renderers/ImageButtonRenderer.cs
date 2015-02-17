[assembly: Xamarin.Forms.Platforms.Xna.ExportRenderer(
    typeof(Sample.Controls.ImageButton),
    typeof(Sample.Renderers.ImageButtonRenderer))]
namespace Sample.Renderers
{
    using Controls;
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System.Threading;
    using Xamarin.Forms.Platforms.Xna;
    using Xamarin.Forms.Platforms.Xna.Images;
    using Xamarin.Forms.Platforms.Xna.Input;
    using Xamarin.Forms.Platforms.Xna.Renderers;
    using XnaColor = Microsoft.Xna.Framework.Color;
    using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

    public class ImageButtonRenderer : LabelRenderer
    {
        CancellationTokenSource _imageLoadCancellation;
        IImage _image;

        public new ImageButton Model { get { return (ImageButton)base.Model; } }

        public ImageButtonRenderer()
        {
            PropertyTracker.AddHandler(ImageButton.ImageProperty, HandleImage);
            PropertyTracker.AddHandler(ImageButton.TextProperty, p => InvalidateVisual());
        }

        async void HandleImage(Xamarin.Forms.BindableProperty prop)
        {
            if (_imageLoadCancellation != null)
                _imageLoadCancellation.Cancel();
            _imageLoadCancellation = new CancellationTokenSource();
            _image = await Model.Image.LoadAsync(_imageLoadCancellation.Token);
            InvalidateMeasure();
        }

        public override Xamarin.Forms.SizeRequest Measure(Xamarin.Forms.Size availableSize)
        {
            var lblSize = base.Measure(availableSize);
            if (_image != null)
            {
                return _image.Measure(VisualState, availableSize, lblSize);
            }
            return lblSize;
        }

        public override bool HandleClick()
        {
            Model.FireClicked();
            return true;
        }

        protected override void LocalDraw(GameTime gameTime, XnaRectangle area)
        {
            if (_image != null)
            {
                _image.Draw(VisualState, SpriteBatch, area, new XnaColor(XnaColor.White, Model.ImageOpacity));
                base.LocalDraw(gameTime, _image.GetContentArea(VisualState, area));
            }
            else base.LocalDraw(gameTime, area);
        }

        public override void OnMouseEnter()
        {
            Model.State = ImageButtonState.Over;
            base.OnMouseEnter();
        }

        public override void OnMouseLeave()
        {
            RemoveVisualState(Mouse.Pressed);
            Model.State = ImageButtonState.Normal;
            base.OnMouseLeave();
        }

        public override bool HandleMouseDown(Mouse.Button button)
        {
            if (button == Mouse.Button.Left)
            {
                AddVisualState(Mouse.Pressed);
                Model.State = ImageButtonState.Pressed;
            }
            return base.HandleMouseDown(button);
        }

        public override bool HandleMouseUp(Mouse.Button button)
        {
            if (button == Mouse.Button.Left)
            {
                RemoveVisualState(Mouse.Pressed);
                Model.State = ImageButtonState.Over;
            }
            return base.HandleMouseUp(button);
        }
    }
}
