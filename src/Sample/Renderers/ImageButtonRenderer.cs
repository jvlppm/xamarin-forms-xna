[assembly: Xamarin.Forms.Platforms.Xna.ExportRenderer(
    typeof(Sample.Controls.ImageButton),
    typeof(Sample.Renderers.ImageButtonRenderer))]
namespace Sample.Renderers
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Controls;
    using System;
    using Xamarin.Forms.Platforms.Xna.Renderers;
    using Xamarin.Forms.Platforms.Xna;
    using Xamarin.Forms.Platforms.Xna.Input;
    using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

    public class ImageButtonRenderer : LabelRenderer
    {
        NinePatch _image;

        public new ImageButton Model { get { return (ImageButton)base.Model; } }

        public ImageButtonRenderer()
        {
            PropertyTracker.AddHandler(ImageButton.ImageProperty, HandleImage);
            PropertyTracker.AddHandler(ImageButton.TextProperty, p => InvalidateVisual());
        }

        void HandleImage(Xamarin.Forms.BindableProperty prop)
        {
            _image = Model.Image == null ? null : new NinePatch(Xamarin.Forms.Forms.Game.Content.Load<Texture2D>(Model.Image));
            InvalidateMeasure();
        }

        public override Xamarin.Forms.SizeRequest Measure(Xamarin.Forms.Size availableSize)
        {
            var lblSize = base.Measure(availableSize);
            if (_image != null)
            {
                if (double.IsPositiveInfinity(availableSize.Width))
                    availableSize.Width = _image.Width;
                if (double.IsPositiveInfinity(availableSize.Height))
                    availableSize.Height = _image.Height;

                var minSize = new Xamarin.Forms.Size(
                    (_image.Width - _image.Stretch.Horizontal.End) + _image.Stretch.Horizontal.Start,
                    (_image.Height - _image.Stretch.Vertical.End) + _image.Stretch.Vertical.Start
                );

                var scaleFit = Math.Min(
                                   availableSize.Width / (float)_image.Width,
                                   availableSize.Height / (float)_image.Height);

                var requestSize = new Xamarin.Forms.Size(
                    width: Model.HorizontalOptions.Alignment == Xamarin.Forms.LayoutAlignment.Fill ?
                        _image.Width * scaleFit : lblSize.Request.Width + minSize.Width,

                    height: Model.VerticalOptions.Alignment == Xamarin.Forms.LayoutAlignment.Fill ?
                        _image.Height * scaleFit : lblSize.Request.Height + minSize.Height
                );

                return new Xamarin.Forms.SizeRequest(requestSize, minSize);
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
                SpriteBatch.Draw(_image, area, new Color(Color.White, Model.ImageOpacity));
                base.LocalDraw(gameTime, _image.GetContentArea(area));
            }
            else base.LocalDraw(gameTime, area);
        }

        public override void OnMouseEnter()
        {
            Model.State = ImageButtonState.Over;
            base.OnMouseEnter();
        }

        public override bool HandleMouseDown(Mouse.Button button)
        {
            Model.State = ImageButtonState.Pressed;
            return base.HandleMouseDown(button);
        }

        public override bool HandleMouseUp(Mouse.Button button)
        {
            Model.State = ImageButtonState.Over;
            return base.HandleMouseUp(button);
        }

        public override void OnMouseLeave()
        {
            Model.State = ImageButtonState.Normal;
            base.OnMouseLeave();
        }
    }
}

