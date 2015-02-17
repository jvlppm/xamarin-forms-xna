namespace Xamarin.Forms.Platforms.Xna.Images
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;

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

        public static async Task<StateImage> FromImageSource(ImageSource source, ImageFormat format, IEnumerable<State> enabled = null, IEnumerable<State> disabled = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var image = await source.LoadAsync(cancellationToken, format);
            return new StateImage(image, enabled, disabled);
        }

        public IDictionary<State, bool> States { get; private set; }
        public IImage Image { get; private set; }
    }

    class StateList : List<StateImage>, IImage
    {
        public static async Task<StateList> FromXml(XmlTextReader reader, CancellationToken cancellation)
        {
            var statePrefix = "state_";

            var xml = XDocument.Load(reader);
            var getImages = from item in xml.Root.Elements("item")
                            let states = from attr in item.Attributes()
                                         where attr.Name.ToString().StartsWith(statePrefix)
                                         select new
                                         {
                                             State = State.ByName(attr.Name.ToString().Substring(statePrefix.Length)),
                                             Enabled = bool.Parse(attr.Value)
                                         }
                            select StateImage.FromImageSource(
                                source: (string)item.Attribute("image"),
                                format: ImageFormat.Unknown,
                                enabled: from state in states
                                         where state.Enabled
                                         select state.State,
                                disabled: from state in states
                                          where !state.Enabled
                                          select state.State,
                                cancellationToken: cancellation);

            var result = new StateList();
            result.AddRange(await Task.WhenAll(getImages));
            return result;
        }

        IImage FromState(ISet<State> states)
        {
            var selectedState = this.FirstOrDefault(t => t.States.All(s => s.Value == states.Contains(s.Key)));
            return selectedState == null ? null : selectedState.Image;
        }

        public SizeRequest Measure(ISet<State> states, Size availableSize, SizeRequest contentSize)
        {
            var image = FromState(states);
            if (image == null)
                return default(SizeRequest);
            return image.Measure(states, availableSize, contentSize);
        }

        public void Draw(ISet<State> states, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Microsoft.Xna.Framework.Rectangle area, Microsoft.Xna.Framework.Color color)
        {
            var image = FromState(states);
            if (image != null)
                image.Draw(states, spriteBatch, area, color);
        }

        public Microsoft.Xna.Framework.Rectangle GetContentArea(ISet<State> states, Microsoft.Xna.Framework.Rectangle area)
        {
            var image = FromState(states);
            if (image != null)
                return image.GetContentArea(states, area);

            return area;
        }
    }
}
