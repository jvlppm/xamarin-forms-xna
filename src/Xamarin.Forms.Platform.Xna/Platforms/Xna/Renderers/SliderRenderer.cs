[assembly: Xamarin.Forms.Platforms.Xna.ExportRenderer(
    typeof(Xamarin.Forms.Slider),
    typeof(Xamarin.Forms.Platforms.Xna.Renderers.SliderRenderer))]
namespace Xamarin.Forms.Platforms.Xna.Renderers
{
    using Controls;
    using Input;
    using Microsoft.Xna.Framework;
    using System.Threading.Tasks;

    public class SliderRenderer : VisualElementRenderer<Slider>
    {
        #region Attached Properties
        public static BindableProperty ThumbImageProperty = BindableProperty.CreateAttached<SliderRenderer, ImageSource>(
                r => GetThumbImage(r),
                "pack://application/Xamarin.Forms.Platform.WP8;component/Xamarin.Forms.SliderThumb.png");

        public static BindableProperty EndImageProperty = BindableProperty.CreateAttached<SliderRenderer, ImageSource>(
                r => GetEndImage(r),
                "pack://application/Xamarin.Forms.Platform.WP8;component/Xamarin.Forms.SliderEnd.png");

        public static BindableProperty TrackImageProperty = BindableProperty.CreateAttached<SliderRenderer, ImageSource>(
                r => GetTrackImage(r),
                "pack://application/Xamarin.Forms.Platform.WP8;component/Xamarin.Forms.SliderTrack.png");

        public static ImageSource GetThumbImage(BindableObject obj)
        {
            return (ImageSource)obj.GetValue(ThumbImageProperty);
        }

        public static ImageSource GetEndImage(BindableObject obj)
        {
            return (ImageSource)obj.GetValue(EndImageProperty);
        }

        public static ImageSource GetTrackImage(BindableObject obj)
        {
            return (ImageSource)obj.GetValue(TrackImageProperty);
        }
        #endregion

        IControl ThumbImage;
        IControl EndImage;
        IControl TrackImage;

        public SliderRenderer()
        {
            PropertyTracker.AddHandler(ThumbImageProperty, Handle_ThumbImage);
            PropertyTracker.AddHandler(TrackImageProperty, Handle_TrackImage);
            PropertyTracker.AddHandler(EndImageProperty, Handle_EndImage);
            OnMouseMove += SliderRenderer_OnMouseMove;
        }

        async void Handle_ThumbImage(BindableProperty property)
        {
            await LoadResourcesAsync(thumb: GetThumbImage(Model));
        }

        async void Handle_TrackImage(BindableProperty property)
        {
            await LoadResourcesAsync(track: GetTrackImage(Model));
        }

        async void Handle_EndImage(BindableProperty property)
        {
            await LoadResourcesAsync(end: GetEndImage(Model));
        }

        public async Task LoadResourcesAsync(ImageSource thumb = null, ImageSource end = null, ImageSource track = null)
        {
            if (thumb != null)
                ThumbImage = await thumb.LoadAsync();
            if (end != null)
                EndImage = await end.LoadAsync();
            if(track != null)
                TrackImage = await track.LoadAsync();
            InvalidateMeasure();
        }

        #region Render

        public override SizeRequest Measure(Size availableSize)
        {
            var endMeasure = EndImage?.Measure(VisualState, default(Size), default(SizeRequest)) ?? default(SizeRequest);
            int endWidth = (int)endMeasure.Request.Width;
            int endHeight = (int)endMeasure.Request.Height;
            var trackMeasure = TrackImage?.Measure(VisualState, default(Size), default(SizeRequest)) ?? default(SizeRequest);
            int trackWidth = (int)trackMeasure.Request.Width;
            int trackHeight = (int)trackMeasure.Request.Height;
            var thumbMeasure = ThumbImage?.Measure(VisualState, default(Size), default(SizeRequest)) ?? default(SizeRequest);
            int thumbWidth = (int)thumbMeasure.Request.Width;
            int thumbHeight = (int)thumbMeasure.Request.Height;

            return new SizeRequest(new Size(thumbWidth + endWidth * 2 + trackWidth, thumbHeight), new Size(thumbWidth, thumbHeight));
        }

        protected override void LocalDraw(GameTime gameTime, Rectangle area)
        {
            var endMeasure = EndImage?.Measure(VisualState, default(Size), default(SizeRequest)) ?? default(SizeRequest);
            int endWidth = (int)endMeasure.Request.Width;
            int endHeight = (int)endMeasure.Request.Height;

            var trackMeasure = TrackImage?.Measure(VisualState, default(Size), default(SizeRequest)) ?? default(SizeRequest);
            int trackWidth = (int)trackMeasure.Request.Width;
            int trackHeight = (int)trackMeasure.Request.Height;

            var thumbMeasure = ThumbImage?.Measure(VisualState, default(Size), default(SizeRequest)) ?? default(SizeRequest);
            int thumbWidth = (int)thumbMeasure.Request.Width;
            int thumbHeight = (int)thumbMeasure.Request.Height;

            if (EndImage != null)
            {
                EndImage.Draw(VisualState, SpriteBatch, new Rectangle(area.X + thumbWidth / 2, (area.Y + area.Height - endHeight) / 2, endWidth, endHeight), Color.White);
                EndImage.Draw(VisualState, SpriteBatch, new Rectangle(area.Right - endWidth - thumbWidth / 2, (area.Y + area.Height - endHeight) / 2, endWidth, endHeight), Color.White);
            }

            TrackImage?.Draw(VisualState, SpriteBatch, new Rectangle(area.X + endWidth + thumbWidth / 2, (area.Y + area.Height - trackHeight) / 2, area.Width - endWidth * 2 - thumbWidth, trackHeight), Color.White);

            if (ThumbImage != null)
            {
                var value = (Model.Value - Model.Minimum) / (Model.Maximum - Model.Minimum);
                ThumbImage.Draw(VisualState, SpriteBatch, new Rectangle(thumbWidth / 2 + (int)((area.Width - thumbWidth) * value) - thumbWidth / 2, (area.Height - thumbHeight) / 2, thumbWidth, thumbHeight), Color.White);
            }
        }

        #endregion

        void SliderRenderer_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (VisualState.Contains(Mouse.Pressed))
            {
                var thumbMeasure = ThumbImage.Measure(VisualState, default(Size), default(SizeRequest));
                int thumbWidth = (int)thumbMeasure.Request.Width;

                Model.Value = MathHelper.Lerp((float)Model.Minimum, (float)Model.Maximum, (e.Position.Value.X - thumbWidth / 2) / ((float)Model.Bounds.Width - thumbWidth));
                InvalidateVisual();
            }
        }
    }
}
