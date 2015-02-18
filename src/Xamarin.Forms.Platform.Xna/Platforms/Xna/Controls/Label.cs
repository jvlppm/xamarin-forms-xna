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
        public float Scale = 1;
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
            var textOffset = new Vector2(
                area.Left + GetAlignOffset(XAlign, (float)textMeasure.X * Scale, area.Width),
                area.Top + GetAlignOffset(YAlign, (float)textMeasure.Y * Scale, area.Height));

            spriteBatch.DrawString(Font, Text, textOffset, color, 0, Vector2.Zero, Scale, SpriteEffects.None, 1);
        }

        public SizeRequest Measure(ISet<State> states, Size availableSize, SizeRequest contentSize)
        {
            if (Font == null || Text == null)
                return default(SizeRequest);

            var textMeasure = Font.MeasureString(Text);
            return new SizeRequest(new Size(textMeasure.X * Scale, textMeasure.Y * Scale));
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
