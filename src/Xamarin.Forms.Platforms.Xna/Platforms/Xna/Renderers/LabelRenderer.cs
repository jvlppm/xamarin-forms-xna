[assembly: Xamarin.Forms.Platforms.Xna.ExportRenderer(
    typeof(Xamarin.Forms.Label),
    typeof(Xamarin.Forms.Platforms.Xna.Renderers.LabelRenderer))]
namespace Xamarin.Forms.Platforms.Xna.Renderers
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using SpriteFont = Microsoft.Xna.Framework.Graphics.SpriteFont;
    using Color = Microsoft.Xna.Framework.Color;
    using Vector2 = Microsoft.Xna.Framework.Vector2;
    using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

    public class LabelRenderer : VisualElementRenderer<Label>
    {
        public static SpriteFont DefaultFont;
        public static Color DefaultTextColor = Color.Black;
        float _scale = 1f;

        SpriteFont _font;
        Color _textColor;
        Vector2? _textOffset;
        SizeRequest _measuredSize;
        Rectangle? _lastArea;

        public LabelRenderer()
        {
            PropertyTracker.AddHandler(Label.TextColorProperty, Handle_TextColor);
            PropertyTracker.AddHandler(Label.FontProperty, Handle_Font);

            PropertyTracker.AddHandler(Label.FontProperty, Handle_MeasureProperty);
            PropertyTracker.AddHandler(Label.TextProperty, Handle_MeasureProperty);
        }

        public override SizeRequest Measure(Size availableSize)
        {
            var font = _font ?? DefaultFont;

            if (font == null || Model.Text == null || !IsVisible)
                return base.Measure(availableSize);

            var textMeasure = font.MeasureString(Model.Text);
            _measuredSize = new SizeRequest(new Size(textMeasure.X * _scale, textMeasure.Y * _scale));
            return _measuredSize;
        }

        protected override void LocalDraw(Microsoft.Xna.Framework.GameTime gameTime, Rectangle area)
        {
            var font = _font ?? DefaultFont;
            if (font == null || Model.Text == null)
                return;

            if (_textOffset == null || _lastArea != area)
                UpdateTextAlignment(area);

            SpriteBatch.DrawString(font, Model.Text, _textOffset.Value, _textColor, 0, Vector2.Zero, _scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 1);
        }

        void UpdateTextAlignment(Rectangle area)
        {
            _lastArea = area;
            Measure(area.Size);
            _textOffset = new Vector2(
                (float)area.Left + GetAlignOffset(Model.XAlign, (float)_measuredSize.Request.Width, (float)area.Width),
                (float)area.Top + GetAlignOffset(Model.YAlign, (float)_measuredSize.Request.Height, (float)area.Height));
        }

        #region Property Handlers

        void Handle_TextColor(BindableProperty property)
        {
            if (Model.TextColor == default(Xamarin.Forms.Color))
                _textColor = LabelRenderer.DefaultTextColor;
            else
                _textColor = Model.TextColor.ToXnaColor();
        }

        void Handle_Font(BindableProperty property)
        {
            if (Model.Font.FontFamily != null)
                _font = Forms.Game.Content.Load<SpriteFont>(Model.Font.FontFamily);
            else
                _font = null;

            switch (Model.Font.NamedSize)
            {
                case NamedSize.Micro: _scale = 0.5f; break;
                case NamedSize.Small: _scale = 0.75f; break;
                case NamedSize.Medium: _scale = 1f; break;
                case NamedSize.Large: _scale = 1.5f; break;
            }
        }

        void Handle_MeasureProperty(BindableProperty property)
        {
            _textOffset = null;
            InvalidateMeasure();
        }

        static float GetAlignOffset(TextAlignment alignment, float textSize, float renderSize)
        {
            switch (alignment)
            {
                case TextAlignment.Start:
                    return 0;
                case TextAlignment.Center:
                    return (renderSize - textSize) * 0.5f;
                case TextAlignment.End:
                    return (renderSize - textSize);
            }
            throw new System.NotImplementedException("Unsupported TextAlignment");
        }

        #endregion
    }
}
