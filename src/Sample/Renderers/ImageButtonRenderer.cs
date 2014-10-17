[assembly: Xamarin.Forms.Platforms.Xna.ExportRenderer(
    typeof(Sample.Controls.ImageButton),
    typeof(Sample.Renderers.ImageButtonRenderer))]
namespace Sample.Renderers
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Sample.Controls;
    using System;
    using Xamarin.Forms.Platforms.Xna.Renderers;

    public class ImageButtonRenderer : LabelRenderer, IClickableRenderer
    {
        Texture2D _image;
        bool? _mouseState;

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

        public override void Update(GameTime gameTime)
        {
            try
            {
                var mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
                var region = this.GetArea();
                if (Model.IsEnabled && region.Contains(new Xamarin.Forms.Point(mouse.X, mouse.Y)))
                {
                    if (mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && _mouseState == false)
                    {
                        if (Model.State != ImageButtonState.Pressed &&
                            Model.State != ImageButtonState.Pressing)
                        {
                            Model.FireClicked();
                            Model.State = ImageButtonState.Pressed;
                        }
                        else
                        {
                            Model.State = ImageButtonState.Pressing;
                            if (Model.ContinuousClick)
                                Model.FireClicked();
                        }
                    }
                    else
                    {
                        Model.State = ImageButtonState.Over;
                    }
                }
                else
                {
                    Model.State = ImageButtonState.Normal;
                }

                _mouseState = mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
            }
            catch (InvalidOperationException invalidOpEx)
            {
                System.Console.WriteLine(invalidOpEx.Message);
            }

            base.Update(gameTime);
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

        public override void Disappeared()
        {
            _mouseState = null;
        }
    }
}

