[assembly: Xamarin.Forms.Platforms.Xna.ExportRenderer(
    typeof(Xamarin.Forms.Label),
    typeof(Xamarin.Forms.Platforms.Xna.Renderers.LabelRenderer))]
namespace Xamarin.Forms.Platforms.Xna.Renderers
{
    using Xamarin.Forms;
    using Color = Microsoft.Xna.Framework.Color;
    using SpriteFont = Microsoft.Xna.Framework.Graphics.SpriteFont;
    using Vector2 = Microsoft.Xna.Framework.Vector2;
    using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

    public class LabelRenderer : VisualElementRenderer<Label>
    {
        #region Static
        static SpriteFont DefaultFont;
        static Color DefaultTextColor = Color.Black;
        static LabelRenderer()
        {
            DefaultFont = Forms.EmbeddedContent.Load<SpriteFont>("DefaultFont");
        }
        #endregion

        #region Attributes
        float _scale = 1f;
        SpriteFont _font;
        Color _textColor;
        Vector2? _textOffset;
        SizeRequest _measuredSize;
        XnaRectangle? _lastArea;
        #endregion

        #region Constructors
        public LabelRenderer()
        {
            PropertyTracker.AddHandler(Label.TextColorProperty, Handle_TextColor);
            PropertyTracker.AddHandler(Label.FontProperty, Handle_Font);

            PropertyTracker.AddHandler(Label.FontProperty, Handle_MeasureProperty);
            PropertyTracker.AddHandler(Label.TextProperty, Handle_MeasureProperty);
        }
        #endregion

        #region VisualElementRenderer
        public override SizeRequest Measure(Size availableSize)
        {
            var font = _font ?? DefaultFont;

            if (font == null || Model.Text == null || !IsVisible)
                return base.Measure(availableSize);

            var textMeasure = font.MeasureString(Model.Text);
            _measuredSize = new SizeRequest(new Size(textMeasure.X * _scale, textMeasure.Y * _scale));
            return _measuredSize;
        }

        protected override void LocalDraw(Microsoft.Xna.Framework.GameTime gameTime, XnaRectangle area)
        {
            var font = _font ?? DefaultFont;
            if (font == null || Model.Text == null)
                return;

            if (_textOffset == null || _lastArea != area)
                UpdateTextAlignment(area);

            SpriteBatch.DrawString(font, Model.Text, _textOffset.Value, _textColor, 0, Vector2.Zero, _scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 1);
        }
        #endregion

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
            if (Model.FontFamily != null)
                _font = Forms.Game.Content.Load<SpriteFont>(Model.FontFamily);
            else
                _font = null;

            _scale = (float)Model.FontSize / 14f;
        }

        void Handle_MeasureProperty(BindableProperty property)
        {
            _textOffset = null;
            InvalidateMeasure();
        }

        #endregion

        #region Private Methods
        void UpdateTextAlignment(XnaRectangle area)
        {
            _lastArea = area;
            Measure(new Size(area.Width, area.Height));
            _textOffset = new Vector2(
                area.Left + GetAlignOffset(Model.XAlign, (float)_measuredSize.Request.Width, area.Width),
                area.Top + GetAlignOffset(Model.YAlign, (float)_measuredSize.Request.Height, area.Height));
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
