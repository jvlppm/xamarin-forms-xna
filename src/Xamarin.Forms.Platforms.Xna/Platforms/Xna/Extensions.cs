namespace Xamarin.Forms.Platforms.Xna
{
    using Microsoft.Xna.Framework;

    public static class Extensions
    {
        public static Color ToXnaColor(this Xamarin.Forms.Color color)
        {
            return new Color((float)color.R, (float)color.G, (float)color.B, (float)color.A);
        }

        public static Xamarin.Forms.Color ToXFormsColor(this Color color)
        {
            return new Xamarin.Forms.Color((float)color.R / 255, (float)color.G / 255, (float)color.B / 255, (float)color.A / 255);
        }

        public static Rectangle ToXnaRectangle(this Xamarin.Forms.Rectangle rectangle)
        {
            return new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
        }

        public static Xamarin.Forms.Rectangle ToXFormsRectangle(this Rectangle rectangle)
        {
            return new Xamarin.Forms.Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }
    }
}
