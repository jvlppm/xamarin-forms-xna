using System.Collections.Generic;

namespace Xamarin.Forms.Platforms.Xna.Input
{
    using System;
    using System.Linq;
    using Renderers;
    using Platforms.Xna;
    using XnaMouse = Microsoft.Xna.Framework.Input.Mouse;
    using XnaVector2 = Microsoft.Xna.Framework.Vector2;
    using XnaVector3 = Microsoft.Xna.Framework.Vector3;
    using XnaPlane = Microsoft.Xna.Framework.Plane;
    using XnaMatrix = Microsoft.Xna.Framework.Matrix;
    using XnaMouseState = Microsoft.Xna.Framework.Input.MouseState;
    using XnaButtonState = Microsoft.Xna.Framework.Input.ButtonState;
    using System.Collections.Immutable;

    public static class Mouse
    {
        #region Nested
        struct ElementPosition
        {
            public VisualElementRenderer Element;
            public XnaVector2? Position;

            public bool IsPositionInside()
            {
                if (Position != null &&
                    Position.Value.X >= 0 && Position.Value.X < Element.Model.Bounds.Width &&
                    Position.Value.Y >= 0 && Position.Value.Y < Element.Model.Bounds.Height)
                    return true;
                return false;
            }
        }
        #endregion

        internal static void Init() { }

        public static State Over = State.Register("Over");
        public static State Pressed = State.Register("Pressed");

        static List<ElementPosition> ElementPositions = new List<ElementPosition>();

        public enum Button
        {
            Left,
            Right,
            Middle,
            XButton1,
            XButton2
        }

        static VisualElementRenderer _pressing;
        static VisualElementRenderer _over;

        static readonly IDictionary<Button, XnaButtonState?> buttonState = new Dictionary<Button, XnaButtonState?>
        {
            { Button.Left, null },
            { Button.Right, null },
            { Button.Middle, null },
            { Button.XButton1, null },
            { Button.XButton2, null },
        };

        static readonly IDictionary<Button, XnaButtonState?> buttonStateChange = new Dictionary<Button, XnaButtonState?>
        {
            { Button.Left, null },
            { Button.Right, null },
            { Button.Middle, null },
            { Button.XButton1, null },
            { Button.XButton2, null },
        };

        public static void Update(VisualElementRenderer renderer)
        {
            ElementPositions.Clear();

            XnaMouseState state;
            try
            {
                state = XnaMouse.GetState();
            }
            catch (InvalidOperationException) { return; }

            var buttonState = UpdateButtonsState(state);

            var reallyOver = renderer.FlattenHierarchyReverse()
                .Where(c => !c.Model.InputTransparent && c.Model.IsEnabled)
                .Select(c => new ElementPosition { Element = c, Position = state.ToRelative(c) })
                .Where(c => c.IsPositionInside());

            var newOver = reallyOver.FirstOrDefault();
            var newOverEventArgs = newOver.Element != null? new MouseEventArgs(buttonState, state.ToRelative(newOver.Element)) : null;

            if (newOver.Element != null)
            {
                foreach(var stateChange in buttonStateChange)
                {
                    if (stateChange.Value != null)
                    {
                        if (stateChange.Value == XnaButtonState.Pressed)
                        {
                            newOver.Element.HandleRaise(
                                r => new MouseButtonEventArgs(stateChange.Key, buttonState, state.ToRelative(r)),
                                (r, e) => r.InterceptMouseDown(e),
                                (r, e) => r.HandleMouseDown(e));
                        }
                        else
                            newOver.Element.HandleRaise(
                                r => new MouseButtonEventArgs(stateChange.Key, buttonState, state.ToRelative(r)),
                                (r, e) => r.InterceptMouseUp(e),
                                (r, e) => r.HandleMouseUp(e));
                    }
                }

                if (newOver.Position != null)
                    newOver.Element.OnMouseOver(newOverEventArgs);
            }

            if (_pressing == null)
            {
                if (_over != newOver.Element)
                {
                    if (_over != null)
                        _over.OnMouseLeave(new MouseEventArgs(buttonState, state.ToRelative(_over)));
                    if (newOver.Element != null)
                        newOver.Element.OnMouseEnter(newOverEventArgs);

                    _over = newOver.Element;
                }

                if (_over != null && state.LeftButton == XnaButtonState.Pressed)
                {
                    _pressing = _over;
                }
            }
            else if (state.LeftButton == XnaButtonState.Released)
            {
                if (newOver.Element == _pressing)
                {
                    _pressing.RaiseClick(newOverEventArgs);
                    _pressing = null;
                }
                else
                {
                    _pressing.OnMouseLeave(new MouseEventArgs(buttonState, state.ToRelative(_pressing)));
                    _pressing = null;

                    if (newOver.Element != null)
                        newOver.Element.OnMouseEnter(newOverEventArgs);

                    _over = newOver.Element;
                }
            }
        }

        static ImmutableDictionary<Button, XnaButtonState> UpdateButtonsState(XnaMouseState state)
        {
            foreach (Button button in Enum.GetValues(typeof(Button)))
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
                    buttonStateChange[button] = newState;
                else
                    buttonStateChange[button] = null;

                buttonState[button] = newState;
            }

            return buttonState.ToImmutableDictionary(k => k.Key, e => e.Value.Value);
        }

        static XnaVector2? ToRelative(this XnaMouseState state, VisualElementRenderer renderer)
        {
            foreach (var savedPosition in ElementPositions)
            {
                if (savedPosition.Element == renderer)
                    return savedPosition.Position;
            }

            XnaVector2? position;
            var plane = new XnaPlane(new XnaVector3(0, 0, 1), 0);
            plane = plane.Transform(renderer.Effect.World);

            var ray = GetPickRay(renderer, state.X, state.Y);
            var dist = ray.Intersects(plane);
            if (dist != null)
            {
                var clickPosition = XnaVector3.Transform(ray.Position + ray.Direction * dist.Value, XnaMatrix.Invert(renderer.Effect.World));
                position = new XnaVector2(clickPosition.X, clickPosition.Y);
            }
            else position = null;

            ElementPositions.Add(new ElementPosition
            {
                Element = renderer,
                Position = position
            });
            return position;
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

