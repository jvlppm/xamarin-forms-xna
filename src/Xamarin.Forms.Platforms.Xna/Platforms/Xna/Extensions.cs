﻿using System.Collections.Generic;
using Xamarin.Forms.Platforms.Xna.Renderers;
using System.Linq;

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

        public static Plane Transform(this Plane plane, Matrix matrix)
        {
            Plane p;
            Matrix m;
            Matrix.Invert(ref matrix, out m);
            float x = plane.Normal.X;
            float y = plane.Normal.Y;
            float z = plane.Normal.Z;
            float d = plane.D;

            p.Normal.X = (x * m.M11) + (y * m.M12) + (z * m.M13) + (d * m.M14);
            p.Normal.Y = (x * m.M21) + (y * m.M22) + (z * m.M23) + (d * m.M24);
            p.Normal.Z = (x * m.M31) + (y * m.M32) + (z * m.M33) + (d * m.M34);
            p.D = (x * m.M41) + (y * m.M42) + (z * m.M43) + (d * m.M44);
            return p;
        }

        public static IEnumerable<IVisualElementRenderer> FlattenHierarchy(this IVisualElementRenderer renderer)
        {
            yield return renderer;

            foreach (var child in renderer.Children)
                foreach (var sub in FlattenHierarchy(child))
                    yield return sub;
        }
    }
}
