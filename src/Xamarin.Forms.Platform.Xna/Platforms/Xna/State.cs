using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Forms.Platforms.Xna
{
    public class State
    {
        static IDictionary<string, State> _registeredStates = new Dictionary<string, State>();

        public string Name { get; private set; }

        private State(string name)
        {
            Name = name;
        }

        public static State Register(string name)
        {
            var registerName = name.ToLowerInvariant();
            if (_registeredStates.ContainsKey(registerName))
                throw new InvalidOperationException();

            var state = new State(name);
            _registeredStates.Add(registerName, state);
            return state;
        }

        public static State ByName(string name)
        {
            var registerName = name.ToLowerInvariant();
            return _registeredStates[registerName];
        }
    }
}
