namespace Xamarin.Forms.Platforms.Xna.Images
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;

    class StateImage
    {
        public StateImage(IImage image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            States = new Dictionary<State, bool>();
            Image = image;
        }

        public StateImage(IImage image, IEnumerable<State> enabled = null, IEnumerable<State> disabled = null)
            : this(image)
        {
            if (enabled != null)
            {
                foreach (var state in enabled)
                    States.Add(state, true);
            }

            if (disabled != null)
            {
                foreach (var state in disabled)
                    States.Add(state, false);
            }
        }

        public IDictionary<State, bool> States { get; private set; }
        public IImage Image { get; private set; }
    }

    class StateList : List<StateImage>, IImage
    {
        public static Task<StateList> FromXml(XmlTextReader reader)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        break;
                }
            }
            throw new NotImplementedException();
        }

        IImage _currentImage;

        public void SetState(ISet<State> states)
        {
            var selectedState = this.LastOrDefault(t => t.States.All(s => s.Value == states.Contains(s.Key)));
            _currentImage = selectedState == null ? null : selectedState.Image;
        }

        public SizeRequest Measure(Size availableSize, SizeRequest contentSize)
        {
            if (_currentImage == null)
                return default(SizeRequest);
            return _currentImage.Measure(availableSize, contentSize);
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Microsoft.Xna.Framework.Rectangle area, Microsoft.Xna.Framework.Color color)
        {
            if (_currentImage != null)
                _currentImage.Draw(spriteBatch, area, color);
        }

        public Microsoft.Xna.Framework.Rectangle GetContentArea(Microsoft.Xna.Framework.Rectangle area)
        {
            return area;
        }
    }
}
