namespace Xamarin.Forms.Platforms.Xna
{
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using XnaColor = Microsoft.Xna.Framework.Color;
    using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
    using Microsoft.Xna.Framework;

    public class NinePatch : IRenderElement
    {
        public class Range
        {
            private readonly int _start, _end, _limit;

            public Range(int start, int end, int limit)
            {
                _start = start;
                _end = end;
                _limit = limit;
            }

            public int Start { get { return _start; } }
            public int End { get { return _end; } }
            public int Margin { get { return _start + (_limit - _end); } }

            public int Size { get { return _end - _start; } }
        }

        public class Area
        {
            readonly Range _horizontal, _vertical;
            //readonly Rectangle _rectangle;
            public Area(Range horizontal, Range vertical)
            {
                _horizontal = horizontal;
                _vertical = vertical;
                //_rectangle = new Rectangle(_horizontal.Start, _vertical.Start, _horizontal.Size, _vertical.Size);
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
            XnaColor[] data = new XnaColor[texture.Width * texture.Height];
            texture.GetData(data);
            Stretch = new Area(
                vertical: GetLine(texture, data, 1, 0, 0, 0),
                horizontal: GetLine(texture, data, 0, 1, 0, 0));
            Content = new Area(
                vertical: GetLine(texture, data, 0, 1, _texture.Width - 1, 0),
                horizontal: GetLine(texture, data, 1, 0, 0, _texture.Height - 1));

            Width = _texture.Width - 2;
            Height = _texture.Height - 2;

            _leftTop = new XnaRectangle(1, 1, Stretch.Horizontal.Start, Stretch.Vertical.Start);
            _leftCenter = new XnaRectangle(1, Stretch.Vertical.Start, Stretch.Horizontal.Start, Stretch.Vertical.Size);
            _leftBottom = new XnaRectangle(1, Stretch.Vertical.End, Stretch.Horizontal.Start, texture.Height - 1 - Stretch.Vertical.End);
            _centerTop = new XnaRectangle(Stretch.Horizontal.Start, 1, Stretch.Horizontal.Size, Stretch.Vertical.Start);
            _center = new XnaRectangle(Stretch.Horizontal.Start, Stretch.Vertical.Start, Stretch.Horizontal.Size, Stretch.Vertical.Size);
            _centerBottom = new XnaRectangle(Stretch.Horizontal.Start, Stretch.Vertical.End, Stretch.Horizontal.Size, texture.Height - 1 - Stretch.Vertical.End);
            _rightTop = new XnaRectangle(Stretch.Horizontal.End, 1, texture.Width - 1 - Stretch.Horizontal.End, Stretch.Vertical.Start);
            _rightCenter = new XnaRectangle(Stretch.Horizontal.End, Stretch.Vertical.Start, texture.Width - 1 - Stretch.Horizontal.End, Stretch.Vertical.Size);
            _rightBottom = new XnaRectangle(Stretch.Horizontal.End, Stretch.Vertical.End, texture.Width - 1 - Stretch.Horizontal.End, texture.Height - 1 - Stretch.Vertical.End);
        }

        static Range GetLine(Texture2D texture, XnaColor[] data, int dx, int dy, int x, int y)
        {
            int start = -1, length = 0;

            int size = texture.Width * dx + texture.Height * dy;
            for (int i = 0; i < size; i++)
            {
                XnaColor color = data[(y + i) * dy * texture.Width + (x + i) * dx];
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
                return new Range(start, start + length, size);
            return null;
        }

        public void Draw(SpriteBatch spriteBatch, XnaRectangle position, XnaColor color)
        {
            int rowTopHeight = _centerTop.Height;
            int rowBottomHeight = _centerBottom.Height;
            int rowCenterHeight = position.Height - rowTopHeight - rowBottomHeight;

            int startY = position.Top;
            int rowCenterTop = startY + rowTopHeight;
            int rowBottomTop = startY + position.Height - rowBottomHeight;

            int colLeftWidth = _leftCenter.Width;
            int colRightWidth = _rightCenter.Width;
            int colCenterWidth = position.Width - colLeftWidth - colRightWidth;

            int startX = position.Left;
            int colCenterLeft = startX + colLeftWidth;
            int colRightLeft = startX + position.Width - colRightWidth;

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

        public XnaRectangle GetContentArea(XnaRectangle area)
        {
            return new XnaRectangle(
                area.Left + Content.Horizontal.Start,
                area.Top + Content.Vertical.Start,
                area.Width - Content.Horizontal.Margin,
                area.Height - Content.Vertical.Margin);
        }

        public SizeRequest Measure(Size availableSize)
        {
            if (double.IsNaN(availableSize.Width) || double.IsInfinity(availableSize.Width))
                availableSize.Width = Width;
            if (double.IsNaN(availableSize.Height) || double.IsInfinity(availableSize.Height))
                availableSize.Height = Height;
            return new SizeRequest(availableSize, new Size(Stretch.Horizontal.Margin, Stretch.Vertical.Margin));
        }

        public void Draw(SpriteBatch spriteBatch, XnaRectangle area)
        {
            spriteBatch.Draw(this, area, XnaColor.White);
        }
    }
}
