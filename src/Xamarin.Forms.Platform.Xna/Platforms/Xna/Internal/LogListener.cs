using System.Diagnostics;

namespace Xamarin.Forms.Platforms.Xna
{
    class LogListener : Xamarin.Forms.LogListener
    {
        public override void Warning(string category, string message)
        {
            Debug.WriteLine(message, category);
        }
    }
}
