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

    public class ImageButtonRenderer : LabelRenderer
    {
        Texture2D _image;

        public new ImageButton Model { get { return (ImageButton)base.Model; } }

        public ImageButtonRenderer()
        {
            PropertyTracker.AddHandler(ImageButton.ImageProperty, HandleImage);
        }

        void HandleImage(Xamarin.Forms.BindableProperty prop)
        {
            _image = Model.Image == null ? null : Xamarin.Forms.Forms.Game.Content.Load<Texture2D>(Model.Image);
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

                var scaleFit = Math.Min(
                                   availableSize.Width / (float)_image.Width,
                                   availableSize.Height / (float)_image.Height);

                return new Xamarin.Forms.SizeRequest(
                    new Xamarin.Forms.Size(_image.Width * scaleFit, _image.Height * scaleFit),
                    default(Xamarin.Forms.Size));
            }
            return lblSize;
        }

        public override bool HandleClick()
        {
            Model.FireClicked();
            return true;
        }

        protected override void LocalDraw(GameTime gameTime)
        {
            if (_image != null)
            {
                var drawArea = new Rectangle(0, 0, (int)Model.Bounds.Width, (int)Model.Bounds.Height);
                SpriteBatch.Draw(_image, drawArea, new Color(Color.White, Model.ImageOpacity));
            }
            base.LocalDraw(gameTime);
        }
    }
}

