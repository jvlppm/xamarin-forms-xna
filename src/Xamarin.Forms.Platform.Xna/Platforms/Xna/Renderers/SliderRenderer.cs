[assembly: Xamarin.Forms.Platforms.Xna.ExportRenderer(
    typeof(Xamarin.Forms.Slider),
    typeof(Xamarin.Forms.Platforms.Xna.Renderers.SliderRenderer))]
namespace Xamarin.Forms.Platforms.Xna.Renderers
{
    using Controls;
    using Input;
    using Microsoft.Xna.Framework;

    class SliderRenderer : VisualElementRenderer<Slider>
    {
        static SliderRenderer()
        {
            ImageSource.FromResource("Xamarin.Forms.SliderEnd.png")
                .LoadAsync().ContinueWith(t => SliderEnd = t.Result);
            ImageSource.FromResource("Xamarin.Forms.SliderTrack.png")
                .LoadAsync().ContinueWith(t => SliderTrack = t.Result);
            ImageSource.FromResource("Xamarin.Forms.SliderThumb.png")
                .LoadAsync().ContinueWith(t => SliderThumb = t.Result);
        }

        static IControl SliderEnd;
        static IControl SliderTrack;
        static IControl SliderThumb;

        public SliderRenderer()
        {
            OnMouseMove += SliderRenderer_OnMouseMove;
        }

        public override SizeRequest Measure(Size availableSize)
        {
            var endMeasure = SliderEnd.Measure(VisualState, default(Size), default(SizeRequest));
            int endWidth = (int)endMeasure.Request.Width;
            int endHeight = (int)endMeasure.Request.Height;
            var trackMeasure = SliderTrack.Measure(VisualState, default(Size), default(SizeRequest));
            int trackWidth = (int)trackMeasure.Request.Width;
            int trackHeight = (int)trackMeasure.Request.Height;
            var thumbMeasure = SliderThumb.Measure(VisualState, default(Size), default(SizeRequest));
            int thumbWidth = (int)thumbMeasure.Request.Width;
            int thumbHeight = (int)thumbMeasure.Request.Height;

            return new SizeRequest(new Size(thumbWidth + endWidth * 2 + trackWidth, thumbHeight), new Size(thumbWidth, thumbHeight));
        }

        protected override void LocalDraw(GameTime gameTime, Rectangle area)
        {
            if (SliderEnd != null && SliderTrack != null && SliderThumb != null)
            {
                var endMeasure = SliderEnd.Measure(VisualState, default(Size), default(SizeRequest));
                int endWidth = (int)endMeasure.Request.Width;
                int endHeight = (int)endMeasure.Request.Height;
                int trackHeight = (int)SliderTrack.Measure(VisualState, default(Size), default(SizeRequest)).Request.Height;
                var thumbMeasure = SliderThumb.Measure(VisualState, default(Size), default(SizeRequest));
                int thumbWidth = (int)thumbMeasure.Request.Width;
                int thumbHeight = (int)thumbMeasure.Request.Height;

                SliderEnd.Draw(VisualState, SpriteBatch, new Rectangle(area.X + thumbWidth / 2, (area.Y + area.Height - endHeight) / 2, endWidth, endHeight), Color.White);
                SliderTrack.Draw(VisualState, SpriteBatch, new Rectangle(area.X + endWidth + thumbWidth / 2, (area.Y + area.Height - trackHeight) / 2, area.Width - endWidth * 2 - thumbWidth, trackHeight), Color.White);
                SliderEnd.Draw(VisualState, SpriteBatch, new Rectangle(area.Right - endWidth - thumbWidth / 2, (area.Y + area.Height - endHeight) / 2, endWidth, endHeight), Color.White);

                var value = (Model.Value - Model.Minimum) / (Model.Maximum - Model.Minimum);

                SliderThumb.Draw(VisualState, SpriteBatch, new Rectangle(thumbWidth / 2 + (int)((area.Width - thumbWidth) * value) - thumbWidth / 2, (area.Height - thumbHeight) / 2, thumbWidth, thumbHeight), Color.White);
            }
            base.LocalDraw(gameTime, area);
        }

        void SliderRenderer_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (VisualState.Contains(Mouse.Pressed))
            {
                var thumbMeasure = SliderThumb.Measure(VisualState, default(Size), default(SizeRequest));
                int thumbWidth = (int)thumbMeasure.Request.Width;

                Model.Value = MathHelper.Lerp((float)Model.Minimum, (float)Model.Maximum, (e.Position.Value.X - thumbWidth / 2) / ((float)Model.Bounds.Width - thumbWidth));
                InvalidateVisual();
            }
        }

        public override bool HandleMouseDown(MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
                AddVisualState(Mouse.Pressed);
                return true;
            }
            return base.HandleMouseDown(e);
        }

        public override void OnMouseLeave(MouseEventArgs e)
        {
            RemoveVisualState(Mouse.Pressed);
            base.OnMouseLeave(e);
        }

        public override bool HandleMouseUp(MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
                RemoveVisualState(Mouse.Pressed);
                return true;
            }
            return base.HandleMouseUp(e);
        }
    }
}
