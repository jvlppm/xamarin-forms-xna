#if !INTERNAL_CONTEXT
namespace Xamarin.Forms.Platforms.Xna.Context
{
    public class Reference<T> where T : struct
    {
        public static implicit operator T(Reference<T> value)
        {
            return value.Value;
        }
        public static implicit operator Reference<T>(T value)
        {
            return new Reference<T>(value);
        }

        public Reference(T value)
        {
            Value = value;
        }

        public T Value;
    }
}
#endif
