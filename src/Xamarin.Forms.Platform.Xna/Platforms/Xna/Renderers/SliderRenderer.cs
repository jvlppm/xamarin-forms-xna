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
                                                                "pack://application/Xamarin.Forms.Platform.WP8;component/SliderThumb.png");

        public static BindableProperty EndImageProperty = BindableProperty.CreateAttached<SliderRenderer, ImageSource>(
                                                              r => GetEndImage(r),
                                                              "pack://application/Xamarin.Forms.Platform.WP8;component/SliderEnd.png");

        public static BindableProperty TrackImageProperty = BindableProperty.CreateAttached<SliderRenderer, ImageSource>(
                                                                r => GetTrackImage(r),
                                                                "pack://application/Xamarin.Forms.Platform.WP8;component/SliderTrack.png");

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

        public static void SetThumbImage(BindableObject obj, ImageSource value)
        {
            obj.SetValue(ThumbImageProperty, value);
        }

        public static void SetEndImage(BindableObject obj, ImageSource value)
        {
            obj.SetValue(EndImageProperty, value);
        }

        public static void SetTrackImage(BindableObject obj, ImageSource value)
        {
            obj.SetValue(TrackImageProperty, value);
        }

        #endregion

        readonly Button Thumb;
        readonly VisualElementRenderer ThumbRenderer;
        IControl EndImage;
        IControl TrackImage;
        Color BackgroundColor = Color.White;

        public SliderRenderer()
        {
            Thumb = new Button { Text = "Slider Button" };
            AddElement(Thumb);
            ThumbRenderer = GetRenderer(Thumb);
            ThumbRenderer.OnMouseMove += Thumb_OnMouseMove;
            OnMouseClick += SliderRenderer_OnMouseClick;

            PropertyTracker.AddHandler(ThumbImageProperty, Handle_ThumbImage);
            PropertyTracker.AddHandler(TrackImageProperty, Handle_TrackImage);
            PropertyTracker.AddHandler(EndImageProperty, Handle_EndImage);
        }

        #region Render

        public override SizeRequest Measure(Size availableSize)
        {
            var thumb = Thumb.GetSizeRequest(availableSize.Width, availableSize.Height).Request;
            var track = TrackImage.Measure(VisualState);
            var end = EndImage.Measure(VisualState);
            return new SizeRequest(
                new Size(thumb.Width + end.Width * 2 + track.Width, MathHelper.Max((float)thumb.Height, MathHelper.Max((float)end.Height, (float)track.Height))),
                thumb);
        }

        protected override void LocalDraw(GameTime gameTime, Rectangle area)
        {
            var thumb = Thumb.GetSizeRequest(area.Width, area.Height).Request;
            var track = TrackImage.Measure(VisualState);
            var end = EndImage.Measure(VisualState);

            var trackStart = (int)(area.X + thumb.Width / 2);
            var trackWidth = (area.Width - end.Width - thumb.Width);

            var value = (Model.Value - Model.Minimum) / (Model.Maximum - Model.Minimum);
            var endColor = Model.Value >= Model.Maximum ? BackgroundColor : new Color(Color.White, 0.8f * BackgroundColor.A / 255.0f);
            var startColor = Model.Value > Model.Minimum ? BackgroundColor : endColor;

            if (EndImage != null)
            {
                EndImage.Draw(VisualState, SpriteBatch, new Rectangle((int)(trackStart - end.Width / 2), (int)((area.Y + area.Height - end.Height) / 2), (int)end.Width, (int)end.Height), startColor);
                EndImage.Draw(VisualState, SpriteBatch, new Rectangle((int)(trackStart + trackWidth + end.Width / 2), (int)((area.Y + area.Height - end.Height) / 2), (int)end.Width, (int)end.Height), endColor);
            }

            if (TrackImage != null)
            {
                TrackImage.Draw(VisualState, SpriteBatch, new Rectangle((int)(trackStart + end.Width / 2), (int)((area.Y + area.Height - track.Height) / 2), (int)(trackWidth * value), (int)track.Height), startColor);
                TrackImage.Draw(VisualState, SpriteBatch, new Rectangle((int)Math.Floor(trackStart + end.Width / 2 + ((trackWidth) * value)), (int)((area.Y + area.Height - track.Height) / 2), (int)Math.Ceiling(trackWidth * (1 - value)), (int)track.Height), endColor);
            }

            Thumb.Layout(new Xamarin.Forms.Rectangle(
                    (trackWidth + end.Width) * value,
                    ((area.Height - thumb.Height) / 2), thumb.Width, thumb.Height));
        }

        #endregion

        #region Event Handlers

        void Thumb_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (ThumbRenderer.VisualState.Contains(Mouse.Pressed) && e.Position != null)
            {
                var newPosition = Thumb.Bounds.Location.ToXnaVector2() + e.Position.Value;
                Model.Value = GetValue(newPosition);
                InvalidateVisual();
            }
        }

        void SliderRenderer_OnMouseClick(object sender, MouseEventArgs e)
        {
            var newValue = GetValue(e.Position.Value);
            var range = Model.Maximum - Model.Minimum;
            if (newValue < Model.Value)
                Model.Value = Math.Max(Model.Minimum, Model.Value - range / 10);
            else if (newValue > Model.Value)
                Model.Value = Math.Min(Model.Maximum, Model.Value + range / 10);
        }

        #endregion

        #region Property Handlers

        void Handle_ThumbImage(BindableProperty property)
        {
            ButtonRenderer.SetBackgroundImage(Thumb, GetThumbImage(Model));
        }

        protected override void Handle_BackgroundColor(BindableProperty prop)
        {
            Thumb.BackgroundColor = Model.BackgroundColor;

            if (Model.BackgroundColor != default(Xamarin.Forms.Color))
                BackgroundColor = Model.BackgroundColor.ToXnaColor();
            else
                BackgroundColor = DefaultBackgroundColor;
            InvalidateVisual();
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

        #region Private Methods

        double GetValue(Vector2 mousePosition)
        {
            var end = EndImage.Measure(VisualState);

            var trackStart = end.Width + Thumb.Width / 2;
            var trackWidth = (Model.Bounds.Width - end.Width * 2 - Thumb.Width);
            var mouseX = (mousePosition.X - trackStart);

            var value = mouseX / trackWidth;
            return MathHelper.Lerp((float)Model.Minimum, (float)Model.Maximum, (float)value);
        }

        #endregion
    }
}
