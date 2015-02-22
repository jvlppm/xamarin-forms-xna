[assembly: Xamarin.Forms.Platforms.Xna.ExportRenderer(
    typeof(Xamarin.Forms.Slider),
    typeof(Xamarin.Forms.Platforms.Xna.Renderers.SliderRenderer))]
namespace Xamarin.Forms.Platforms.Xna.Renderers
{
    using Controls;
    using Input;
    using Microsoft.Xna.Framework;
    using System;

    public class SliderRenderer : VisualElementRenderer<Slider>
    {
        #region Default Style
        static Color DefaultBackgroundColor = Color.White;
        #endregion

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
        Color BackgroundColor;

        public SliderRenderer()
        {
            PropertyTracker.AddHandler(ThumbImageProperty, Handle_ThumbImage);
            PropertyTracker.AddHandler(TrackImageProperty, Handle_TrackImage);
            PropertyTracker.AddHandler(EndImageProperty, Handle_EndImage);
            OnMouseMove += SliderRenderer_OnMouseMove;
        }

        #region Render

        public override SizeRequest Measure(Size availableSize)
        {
            var thumb = ThumbImage.Measure(VisualState);
            var track = TrackImage.Measure(VisualState);
            var end = EndImage.Measure(VisualState);
            return new SizeRequest(new Size(thumb.Width + end.Width * 2 + track.Width, MathHelper.Max((float)thumb.Height, MathHelper.Max((float)end.Height, (float)track.Height))), thumb);
        }

        protected override void LocalDraw(GameTime gameTime, Rectangle area)
        {
            var thumb = ThumbImage.Measure(VisualState);
            var track = TrackImage.Measure(VisualState);
            var end = EndImage.Measure(VisualState);

            var value = (Model.Value - Model.Minimum) / (Model.Maximum - Model.Minimum);
            var endColor = Color.White;
            var startColor = Model.Value > Model.Minimum ? BackgroundColor : endColor;

            if (EndImage != null)
            {
                EndImage.Draw(VisualState, SpriteBatch, new Rectangle((int)(area.X + thumb.Width / 2), (int)((area.Y + area.Height - end.Height) / 2), (int)end.Width, (int)end.Height), startColor);
                EndImage.Draw(VisualState, SpriteBatch, new Rectangle((int)(area.Right - end.Width - thumb.Width / 2), (int)((area.Y + area.Height - end.Height) / 2), (int)end.Width, (int)end.Height), endColor);
            }

            if (TrackImage != null)
            {
                var trackWidth = (area.Width - end.Width * 2 - thumb.Width);
                TrackImage.Draw(VisualState, SpriteBatch, new Rectangle((int)(area.X + end.Width + thumb.Width / 2), (int)((area.Y + area.Height - track.Height) / 2), (int)(trackWidth * value), (int)track.Height), startColor);
                TrackImage.Draw(VisualState, SpriteBatch, new Rectangle((int)Math.Floor(area.X + end.Width + thumb.Width / 2 + (trackWidth * value)), (int)((area.Y + area.Height - track.Height) / 2), (int)Math.Ceiling(trackWidth * (1 - value)), (int)track.Height), endColor);
            }

            if (ThumbImage != null)
            {
                ThumbImage.Draw(VisualState, SpriteBatch, new Rectangle((int)(thumb.Width / 2 + (int)((area.Width - thumb.Width) * value) - thumb.Width / 2), (int)((area.Height - thumb.Height) / 2), (int)thumb.Width, (int)thumb.Height), BackgroundColor);
            }
        }

        #endregion

        void SliderRenderer_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (VisualState.Contains(Mouse.Pressed))
            {
                var thumb = ThumbImage.Measure(VisualState);
                var end = EndImage.Measure(VisualState);

                var value = (e.Position.Value.X - thumb.Width / 2 - end.Width) / (Model.Bounds.Width - thumb.Width - end.Width * 2);

                Model.Value = MathHelper.Lerp((float)Model.Minimum, (float)Model.Maximum, (float)value);
                InvalidateVisual();
            }
        }

        #region Property Handlers
        protected override void Handle_BackgroundColor(BindableProperty prop)
        {
            if (Model.BackgroundColor != default(Xamarin.Forms.Color))
                BackgroundColor = Model.BackgroundColor.ToXnaColor();
            else
                BackgroundColor = DefaultBackgroundColor;
            InvalidateVisual();
        }

        async void Handle_ThumbImage(BindableProperty property)
        {
            ThumbImage = await GetThumbImage(Model).LoadAsync();
            InvalidateMeasure();
        }

        async void Handle_TrackImage(BindableProperty property)
        {
            TrackImage = await GetTrackImage(Model).LoadAsync();
            InvalidateMeasure();
        }

        async void Handle_EndImage(BindableProperty property)
        {
            EndImage = await GetEndImage(Model).LoadAsync();
            InvalidateMeasure();
        }
        #endregion
    }
}
