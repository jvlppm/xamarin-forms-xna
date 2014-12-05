namespace Xamarin.Forms.Platforms.Xna
{
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using XnaColor = Microsoft.Xna.Framework.Color;
    using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

    public class NinePatch
    {
        public class Range
        {
            private readonly int _start, _end;

            public Range(int start, int end)
            {
                _start = start;
                _end = end;
            }

            public int Start { get { return _start; } }
            public int End { get { return _end; } }
        }

        public class Area
        {
            readonly Range _horizontal, _vertical;
            public Area(Range horizontal, Range vertical)
            {
                _horizontal = horizontal;
                _vertical = vertical;
            }

            public Range Horizontal { get { return _horizontal; } }
            public Range Vertical { get { return _vertical; } }
        }

        public readonly Area Stretch;
        public readonly Area Content;
        public readonly int Width;
        public readonly int Height;

        readonly Texture2D _texture;
        readonly XnaRectangle _leftTop;
        readonly XnaRectangle _leftCenter;
        readonly XnaRectangle _leftBottom;
        readonly XnaRectangle _centerTop;
        readonly XnaRectangle _center;
        readonly XnaRectangle _centerBottom;
        readonly XnaRectangle _rightTop;
        readonly XnaRectangle _rightCenter;
        readonly XnaRectangle _rightBottom;

        public NinePatch(Texture2D texture)
        {
            _texture = texture;
            Stretch = new Area(
                vertical: GetLine(texture, 1, 0, 0, 0),
                horizontal: GetLine(texture, 0, 1, 0, 0));
            Content = new Area(
                vertical: GetLine(texture, 0, 1, _texture.Width - 1, 0),
                horizontal: GetLine(texture, 1, 0, 0, _texture.Height - 1));

            Width = _texture.Width - 2;
            Height = _texture.Height - 2;

            _leftTop = new XnaRectangle(1, 1, Stretch.Horizontal.Start, Stretch.Vertical.Start);
            _leftCenter = new XnaRectangle(1, Stretch.Vertical.Start, Stretch.Horizontal.Start, Stretch.Vertical.End);
            _leftBottom = new XnaRectangle(1, Stretch.Vertical.End, Stretch.Horizontal.Start, texture.Height - 1);
            _centerTop = new XnaRectangle(Stretch.Horizontal.Start, 1, Stretch.Horizontal.End, Stretch.Vertical.Start);
            _center = new XnaRectangle(Stretch.Horizontal.Start, Stretch.Vertical.Start, Stretch.Horizontal.End, Stretch.Vertical.End);
            _centerBottom = new XnaRectangle(Stretch.Horizontal.Start, Stretch.Vertical.End, Stretch.Horizontal.End, texture.Height - 1);
            _rightTop = new XnaRectangle(Stretch.Horizontal.End, 1, texture.Width - 1, Stretch.Vertical.Start);
            _rightCenter = new XnaRectangle(Stretch.Horizontal.End, Stretch.Vertical.Start, texture.Width - 1, Stretch.Vertical.End);
            _rightBottom = new XnaRectangle(Stretch.Horizontal.End, Stretch.Vertical.End, texture.Width - 1, texture.Height - 1);
        }

        static Range GetLine(Texture2D texture, int dx, int dy, int x, int y)
        {
            XnaColor[] data = new XnaColor[dx * texture.Width + dy * texture.Height];

            int start = -1, length = 0;
            texture.GetData(0, new XnaRectangle(0, 0, dx == 0 ? 1 : texture.Width, dy == 0 ? 1 : texture.Height), data, 0, data.Length);

            for (int i = 0; i < data.Length; i++)
            {
                XnaColor color = data[i];
                if (color == XnaColor.Black)
                {
                    length++;
                    if (start < 0)
                        start = i;
                }
                else if (color == XnaColor.Transparent)
                {
                    if (start >= 0)
                        break;
                }
                else throw new ArgumentException("Invalid 9-patch image " + texture.Name, "texture");
            }
            if (start >= 0)
                return new Range(start, start + length);
            return null;
        }

        public void Draw(SpriteBatch spriteBatch, XnaRectangle position, XnaColor color)
        {
            int rowTopHeight = _centerTop.Height;
            int rowBottomHeight = _centerBottom.Height;
            int rowCenterHeight = position.Height - rowTopHeight - rowBottomHeight;

            int startY = position.Top;
            int rowCenterTop = startY + rowTopHeight;
            int rowBottomTop = startY + Height - rowBottomHeight;

            int colLeftWidth = _leftCenter.Width;
            int colRightWidth = _rightCenter.Width;
            int colCenterWidth = Width - colLeftWidth - colRightWidth;

            int startX = position.Left;
            int colCenterLeft = startX + colLeftWidth;
            int colRightLeft = startX + Width - colRightWidth;

            spriteBatch.Draw(_texture, new XnaRectangle(startX, startY, colLeftWidth, rowTopHeight), _leftTop, color);
            spriteBatch.Draw(_texture, new XnaRectangle(startX, rowCenterTop, colLeftWidth, rowCenterHeight), _leftCenter, color);
            spriteBatch.Draw(_texture, new XnaRectangle(startX, rowBottomTop, colLeftWidth, rowBottomHeight), _leftBottom, color);
            spriteBatch.Draw(_texture, new XnaRectangle(colCenterLeft, startY, colCenterWidth, rowTopHeight), _centerTop, color);
            spriteBatch.Draw(_texture, new XnaRectangle(colCenterLeft, rowCenterTop, colCenterWidth, rowCenterHeight), _center, color);
            spriteBatch.Draw(_texture, new XnaRectangle(colCenterLeft, rowBottomTop, colCenterWidth, rowBottomHeight), _centerBottom, color);
            spriteBatch.Draw(_texture, new XnaRectangle(colRightLeft, startY, colRightWidth, rowTopHeight), _rightTop, color);
            spriteBatch.Draw(_texture, new XnaRectangle(colRightLeft, rowCenterTop, colRightWidth, rowCenterHeight), _rightCenter, color);
            spriteBatch.Draw(_texture, new XnaRectangle(colRightLeft, rowBottomTop, colRightWidth, rowBottomHeight), _rightBottom, color);
        }
    }
}
