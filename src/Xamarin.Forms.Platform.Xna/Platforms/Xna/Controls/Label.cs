namespace Xamarin.Forms.Platforms.Xna.Controls
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    class Label : IControl
    {
        public SpriteFont Font;
        public string Text;
        public float FontSize = 12;
        public TextAlignment XAlign, YAlign;

        public Rectangle GetContentArea(ISet<State> states, Rectangle area)
		{
			return area;
		}

        public void Draw(ISet<State> states, SpriteBatch spriteBatch, Rectangle area, Color color)
        {
            if (Font == null || Text == null)
                return;

            var textMeasure = Font.MeasureString(Text);
            var originalFontSize = textMeasure.Y * 72 / 96.0f;
            var scale = FontSize / originalFontSize;

            var textOffset = new Vector2(
                area.Left + GetAlignOffset(XAlign, textMeasure.X * scale, area.Width),
                area.Top + GetAlignOffset(YAlign, textMeasure.Y * scale, area.Height));

            spriteBatch.DrawString(Font, Text, textOffset, color, 0, Vector2.Zero, scale, SpriteEffects.None, 1);
        }

        public SizeRequest Measure(ISet<State> states, Size availableSize, SizeRequest contentSize)
        {
            if (Font == null || Text == null)
                return default(SizeRequest);

            var textMeasure = Font.MeasureString(Text);
            var originalFontSize = textMeasure.Y * 72 / 96.0f;
            var scale = FontSize / originalFontSize;

            return new SizeRequest(new Size(textMeasure.X * scale, textMeasure.Y * scale));
        }

        #region Private Methods

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
            throw new ArgumentException("Unsupported TextAlignment", "alignment");
        }
        #endregion
    }
}
