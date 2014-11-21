using System.Collections.Generic;

namespace Xamarin.Forms.Platforms.Xna.Input
{
    using System;
    using System.Linq;
    using Renderers;
    using XnaMouse = Microsoft.Xna.Framework.Input.Mouse;
    using XnaVector2 = Microsoft.Xna.Framework.Vector2;
    using XnaVector3 = Microsoft.Xna.Framework.Vector3;
    using XnaPlane = Microsoft.Xna.Framework.Plane;
    using XnaMatrix = Microsoft.Xna.Framework.Matrix;
    using XnaMouseState = Microsoft.Xna.Framework.Input.MouseState;
    using XnaButtonState = Microsoft.Xna.Framework.Input.ButtonState;

    public static class Mouse
    {
        public enum Button
        {
            Left,
            Right,
            Middle,
            XButton1,
            XButton2
        }

        static VisualElementRenderer Pressing;
        static VisualElementRenderer Over;

        static IDictionary<Button, XnaButtonState?> buttonState = new Dictionary<Button, XnaButtonState?>
        {
            { Button.Left, null },
            { Button.Right, null },
            { Button.Middle, null },
            { Button.XButton1, null },
            { Button.XButton2, null },
        };

        public static void Update(VisualElementRenderer renderer)
        {
            XnaMouseState state;
            try
            {
                state = XnaMouse.GetState();
            }
            catch (InvalidOperationException) { return; }

            var reallyOver = renderer.FlattenHierarchyReverse().Select(c =>
                {
                    var pos = DetectPosition(c, state.X, state.Y);
                    if (pos != null &&
                        pos.Value.X >= 0 && pos.Value.X < c.Model.Bounds.Width &&
                        pos.Value.Y >= 0 && pos.Value.Y < c.Model.Bounds.Height)
                        return new { Element = c, Position = pos };
                    return null;
                }).FirstOrDefault(p => p != null);

            var newOver = reallyOver == null ? null : reallyOver.Element;

            if (newOver != null)
            {
                foreach(Button button in Enum.GetValues(typeof(Button)))
                {
                    XnaButtonState newState;
                    switch (button)
                    {
                        case Button.Left:
                            newState = state.LeftButton;
                            break;
                        case Button.Right:
                            newState = state.RightButton;
                            break;
                        case Button.Middle:
                            newState = state.MiddleButton;
                            break;
                        case Button.XButton1:
                            newState = state.XButton1;
                            break;
                        case Button.XButton2:
                            newState = state.XButton2;
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    if (newState != buttonState[button])
                    {
                        buttonState[button] = newState;
                        if (newState == XnaButtonState.Pressed)
                            newOver.HandleRaise(r => r.InterceptMouseDown(button), r => r.HandleMouseDown(button));
                        else
                            newOver.HandleRaise(r => r.InterceptMouseUp(button), r => r.HandleMouseUp(button));
                    }
                }
            }

            if (Pressing == null)
            {
                if (Over != newOver)
                {
                    if (Over != null)
                        Over.OnMouseLeave();
                    if (newOver != null)
                        newOver.OnMouseEnter();

                    Over = newOver;
                }

                if (Over != null && state.LeftButton == XnaButtonState.Pressed)
                {
                    Pressing = Over;
                }
            }
            else if(state.LeftButton == XnaButtonState.Released)
            {
                if (newOver == Pressing)
                {
                    Pressing.HandleRaise(r => r.InterceptClick(), r => r.HandleClick());
                    Pressing = null;
                }
                else
                {
                    Pressing.OnMouseLeave();
                    Pressing = null;

                    if (newOver != null)
                        newOver.OnMouseEnter();

                    Over = newOver;
                }
            }
        }

        static XnaVector2? DetectPosition(VisualElementRenderer renderer, float x, float y)
        {
            var plane = new XnaPlane(new XnaVector3(0, 0, 1), 0);
            plane = plane.Transform(renderer.Effect.World);

            var ray = GetPickRay(renderer, x, y);
            var dist = ray.Intersects(plane);
            if (dist != null)
            {
                var clickPosition = XnaVector3.Transform(ray.Position + ray.Direction * dist.Value, XnaMatrix.Invert(renderer.Effect.World));
                return new XnaVector2(clickPosition.X, clickPosition.Y);
            }
            return null;
        }

        static Microsoft.Xna.Framework.Ray GetPickRay(VisualElementRenderer renderer, float x, float y)
        {
            var nearsource = new XnaVector3(x, y, 0f);
            var farsource = new XnaVector3(x, y, 1f);

            var world = XnaMatrix.Identity;

            var nearPoint = Forms.Game.GraphicsDevice.Viewport.Unproject(nearsource,
                renderer.Effect.Projection, renderer.Effect.View, world);

            var farPoint = Forms.Game.GraphicsDevice.Viewport.Unproject(farsource,
                renderer.Effect.Projection, renderer.Effect.View, world);

            // Create a ray from the near clip plane to the far clip plane.
            var direction = farPoint - nearPoint;
            direction.Normalize();
            return new Microsoft.Xna.Framework.Ray(nearPoint, direction);
        }
    }
}

