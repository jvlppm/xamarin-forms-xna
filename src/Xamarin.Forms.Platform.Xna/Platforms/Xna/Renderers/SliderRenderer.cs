[assembly: Xamarin.Forms.Platforms.Xna.ExportRenderer(
    typeof(Xamarin.Forms.Slider),
    typeof(Xamarin.Forms.Platforms.Xna.Renderers.SliderRenderer))]
namespace Xamarin.Forms.Platforms.Xna.Renderers
{
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Xna.Framework;
    using System.Diagnostics;

    class SliderRenderer : VisualElementRenderer<Slider>
    {
        Texture2D _whiteTexture;
        double pos;

        public SliderRenderer()
        {
            _whiteTexture = new Texture2D(SpriteBatch.GraphicsDevice, 1, 1);
            var data = new[] { Color.White };
            _whiteTexture.SetData<Color>(data);

            OnMouseMove += SliderRenderer_OnMouseMove;
        }

        public override SizeRequest Measure(Size availableSize)
        {
            return new SizeRequest(new Size(100, 8), new Size(0, 8));
        }

        protected override void LocalDraw(GameTime gameTime, Rectangle area)
        {
            Debug.WriteLine("SliderDraw");
            SpriteBatch.Draw(_whiteTexture, new Rectangle(area.X, area.Y, (int)(area.Width * pos), area.Height), Color.Blue);
            base.LocalDraw(gameTime, area);
        }

        void SliderRenderer_OnMouseMove(object sender, Input.MouseEventArgs e)
        {
            pos = e.Position.Value.X / Model.Bounds.Width;
            InvalidateVisual();
        }
    }
}
