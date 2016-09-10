using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Xamarin.Forms.Platforms.Xna.Input;

[assembly: Xamarin.Forms.Platforms.Xna.ExportRenderer(
    typeof(Xamarin.Forms.VisualElement),
    typeof(Xamarin.Forms.Platforms.Xna.Renderers.VisualElementRenderer<Xamarin.Forms.VisualElement>))]
namespace Xamarin.Forms.Platforms.Xna.Renderers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Xamarin.Forms;
    using Xna;
    using XnaMathHelper = Microsoft.Xna.Framework.MathHelper;
    using XnaMatrix = Microsoft.Xna.Framework.Matrix;
    using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
    using XnaVector2 = Microsoft.Xna.Framework.Vector2;
    using XnaVector3 = Microsoft.Xna.Framework.Vector3;

    public class VisualElementRenderer<TModel> : VisualElementRenderer
        where TModel : VisualElement
    {
        public new TModel Model
        {
            get { return (TModel)base.Model; }
            set { base.Model = value; }
        }

        protected virtual void OnModelLoad(TModel model)
        {
            base.OnModelLoad(model);
        }

        protected virtual void OnModelUnload(TModel model)
        {
            base.OnModelUnload(model);
        }

        sealed protected override void OnModelLoad(VisualElement model)
        {
            OnModelLoad((TModel)model);
        }

        sealed protected override void OnModelUnload(VisualElement model)
        {
            OnModelUnload((TModel)model);
        }
    }

    public class VisualElementRenderer : IRegisterable, IDisposable
    {
        #region Static

        public static readonly State Enabled = State.Register("Enabled");

        static BindableProperty RendererProperty = BindableProperty.CreateAttached("Renderer", typeof(VisualElementRenderer), typeof(VisualElementRenderer), null);

        public static VisualElementRenderer GetRenderer(BindableObject obj)
        {
            return (VisualElementRenderer)obj.GetValue(RendererProperty);
        }

        public static void SetRenderer(Element obj, VisualElementRenderer renderer)
        {
            obj.SetValue(RendererProperty, renderer);
        }

        public static VisualElementRenderer Create(VisualElement element)
        {
            if (!Forms.IsInitialized)
                throw new InvalidOperationException("Xamarin.Forms not initialized");

            if (element == null)
                throw new NotImplementedException();

            var renderer = Registrar.Registered.GetHandler<VisualElementRenderer>(element.GetType());
            if (renderer != null)
            {
                SetRenderer(element, renderer);
                renderer.Model = element;
                element.IsPlatformEnabled = true;
            }
            return renderer;
        }

        #endregion

        #region Events

        public event EventHandler<ISet<State>> OnVisualStateChange;
        public event EventHandler<MouseEventArgs> OnMouseMove;
        public event EventHandler<MouseEventArgs> OnMouseClick;

        #endregion

        #region Attributes

        public readonly BasicEffect Effect;

        protected readonly PropertyTracker PropertyTracker;
        protected readonly SpriteBatch SpriteBatch;

        readonly BlendState _blendState;

        Rectangle _lastArrangeBounds;
        XnaVector2? _lastMousePosition;

        XnaRectangle _backgroundArea;
        Texture2D _backgroundTexture;

        VisualElement _model;
        List<VisualElement> _manuallyAddedElements;
        float? _alpha;
        bool _isVisible;
        RenderTarget2D _rendererVisual;
        bool _validVisual;
        bool _disposed;
        VisualElementRenderer _renderParent;

        ImmutableDictionary<Element, VisualElementRenderer> ChildrenRenderers;

        #endregion

        #region Properties

        public VisualElement Model
        {
            get { return _model; }
            set
            {
                if (_model == value)
                    return;

                if (_model != null)
                    OnModelUnload(_model);

                _model = value;

                _manuallyAddedElements.ForEach(m => m.Parent = _model);

                if (_model != null)
                    OnModelLoad(_model);

                PropertyTracker.SetTarget(value);
            }
        }

        public VisualElementRenderer Parent { get; set; }

        public ImmutableList<VisualElementRenderer> Children { get; private set; }

        public ISet<State> VisualState { get; private set; }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible == value)
                    return;
                _isVisible = value;
                if (_isVisible)
                    Appeared();
                else
                    Disappeared();
            }
        }

        #endregion

        #region Constructors

        public VisualElementRenderer()
        {
            if (!Forms.IsInitialized)
                throw new InvalidOperationException("Xamarin.Forms not initialized");

            VisualState = ImmutableHashSet<State>.Empty;

            Effect = new BasicEffect(Forms.Game.GraphicsDevice)
            {
                TextureEnabled = true,
                VertexColorEnabled = true
            };

            _blendState = BlendState.AlphaBlend;

            _manuallyAddedElements = new List<VisualElement>();

            PropertyTracker = new PropertyTracker();
            SpriteBatch = new SpriteBatch(Forms.Game.GraphicsDevice);
            RefreshRenderers();
            IsVisible = true;

            PropertyTracker.AddHandler(VisualElement.AnchorXProperty, Handle_Transformation);
            PropertyTracker.AddHandler(VisualElement.AnchorYProperty, Handle_Transformation);
            PropertyTracker.AddHandler(VisualElement.RotationXProperty, Handle_Transformation);
            PropertyTracker.AddHandler(VisualElement.RotationYProperty, Handle_Transformation);
            PropertyTracker.AddHandler(VisualElement.RotationProperty, Handle_Transformation);
            PropertyTracker.AddHandler(VisualElement.ScaleProperty, Handle_Transformation);
            PropertyTracker.AddHandler(VisualElement.OpacityProperty, Handle_Opacity);
            PropertyTracker.AddHandler(VisualElement.BackgroundColorProperty, Handle_BackgroundColor);
            PropertyTracker.AddHandler(VisualElement.IsEnabledProperty, Handle_Enabled);
        }

        #endregion

        #region IRenderer

        public virtual SizeRequest Measure(Size availableSize)
        {
            var size = new SizeRequest();

            foreach (var child in Children)
            {
                var c = child != null ? child.Model.GetSizeRequest(availableSize.Width, availableSize.Height)
                                            : child.Measure(availableSize);

                size.Minimum = new Size(
                    Math.Max(c.Minimum.Width, size.Minimum.Width),
                    Math.Max(c.Minimum.Height, size.Minimum.Height));

                size.Request = new Size(
                    Math.Max(c.Request.Width, size.Request.Width),
                    Math.Max(c.Request.Height, size.Request.Height));
            }

            return size;
        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (!IsVisible)
                return;

            Render(gameTime);

            RenderChildren(gameTime);
        }

        protected virtual void RenderChildren(Microsoft.Xna.Framework.GameTime gameTime)
        {
            foreach (var childRenderer in Children)
                childRenderer.Draw(gameTime);
        }

        protected void Render(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (_rendererVisual == null ||
                (_rendererVisual.Width != (int)Model.Bounds.Width ||
                _rendererVisual.Height != (int)Model.Bounds.Height))
            {
                if (_rendererVisual != null)
                    _rendererVisual.Dispose();

                _rendererVisual = new RenderTarget2D(SpriteBatch.GraphicsDevice, (int)Model.Bounds.Width, (int)Model.Bounds.Height, false, SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents);
                _validVisual = false;
            }

            if (!_validVisual)
            {
                if (_rendererVisual.Width > 0 && _rendererVisual.Height > 0)
                {
                    SpriteBatch.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
                    SpriteBatch.GraphicsDevice.SetRenderTarget(_rendererVisual);
                    SpriteBatch.Begin();
                    SpriteBatch.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Transparent);
                    LocalDraw(gameTime, new XnaRectangle(0, 0, (int)Model.Bounds.Size.Width, (int)Model.Bounds.Size.Height));
                    SpriteBatch.End();
                    SpriteBatch.GraphicsDevice.SetRenderTarget(null);
                }

                _validVisual = true;
            }

            BeginDraw();
            SpriteBatch.Draw(_rendererVisual, XnaVector2.Zero);
            EndDraw();
        }

        protected virtual void BeginDraw()
        {
            if (Model.Bounds != _lastArrangeBounds)
                Arrange();

            var state = RasterizerState.CullNone;

            Effect.Alpha = (_alpha = _alpha ?? GetAlpha()).Value;
            SpriteBatch.Begin(SpriteSortMode.Immediate, _blendState, null, null, state, Effect);

            if (_backgroundTexture != null)
                SpriteBatch.Draw(_backgroundTexture, _backgroundArea, Microsoft.Xna.Framework.Color.White);

            //state.Dispose();
        }

        protected virtual void LocalDraw(Microsoft.Xna.Framework.GameTime gameTime, XnaRectangle area)
        {
        }

        protected virtual void EndDraw()
        {
            SpriteBatch.End();
        }

        public virtual void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (!IsVisible)
                return;

            foreach (var childRenderer in Children)
                childRenderer.Update(gameTime);
        }

        public virtual void Appeared()
        {
            _isVisible = true;
            foreach (var child in Children)
                child.Appeared();
        }

        public virtual void Disappeared()
        {
            _isVisible = false;
            foreach (var child in Children)
                child.Disappeared();
        }

        #endregion

        #region Property Handlers

        void Handle_Transformation(BindableProperty prop)
        {
            InvalidateTransformations();
        }

        void Handle_Opacity(BindableProperty prop)
        {
            InvalidateAlpha();
        }

        protected virtual void Handle_BackgroundColor(BindableProperty prop)
        {
            if (Model.BackgroundColor == default(Color))
            {
                if (_backgroundTexture != null)
                    _backgroundTexture.Dispose();
                _backgroundTexture = null;
            }
            else
            {
                if (_backgroundTexture == null)
                    _backgroundTexture = new Texture2D(SpriteBatch.GraphicsDevice, 1, 1);
                _backgroundTexture.SetData(new[] { Model.BackgroundColor.ToXnaColor() });
            }
            InvalidateVisual();
        }

        void Handle_Enabled(BindableProperty property)
        {
            if (Model.IsEnabled)
                AddVisualState(Enabled);
            else
                RemoveVisualState(Enabled);
        }

        void InvalidateAlpha()
        {
            _alpha = null;
            foreach (var childRenderer in Children)
                childRenderer.InvalidateAlpha();
        }

        #endregion

        #region 3D Transformations

        internal void InvalidateTransformations()
        {
            _lastArrangeBounds = default(Rectangle);
            foreach (var childRenderer in Children)
                childRenderer.InvalidateTransformations();
        }

        protected virtual void Arrange()
        {
            Effect.World = GetWorldTransformation(Model);
            Effect.Projection = GetProjectionMatrix(Model);
            _lastArrangeBounds = Model.Bounds;
            _backgroundArea = new XnaRectangle(0, 0, (int)Model.Bounds.Width, (int)Model.Bounds.Height);
        }

        static XnaMatrix GetWorldTransformation(Element element)
        {
            XnaMatrix world = XnaMatrix.Identity;
            var currentElement = element;
            while (currentElement != null)
            {
                var currentVisual = currentElement as VisualElement;
                if (currentVisual != null)
                    world *= GetControlTransformation(currentVisual);

                currentElement = currentElement.Parent;
            }

            return world;
        }

        static XnaMatrix GetProjectionMatrix(VisualElement element)
        {
            if (element.Bounds.Width <= 0 && element.Bounds.Height <= 0)
                return XnaMatrix.Identity;

            VisualElementRenderer parent = GetRenderer(element)._renderParent;
            if (parent != null)
                element = parent.Model ?? element;

            var offset = new Point(
                             element.Bounds.X + element.TranslationX,
                             element.Bounds.Y + element.TranslationY
                         ) + element.Bounds.Size * 0.5;

            var viewport = Forms.Game.GraphicsDevice.Viewport;

            float dist = Math.Max(viewport.Width, viewport.Height);
            var angle = (float)Math.Atan(((float)viewport.Height / 2) / dist) * 2;

            return XnaMatrix.CreateTranslation((float)-offset.X - 0.5f, (float)-offset.Y - 0.5f, -dist)
            * XnaMatrix.CreatePerspectiveFieldOfView(angle, ((float)viewport.Width / viewport.Height), dist * 0.8f, dist * 2)
            * XnaMatrix.CreateTranslation(
                (float)offset.X * 2 / viewport.Width - 1,
                (float)offset.Y * 2 / viewport.Height - 1, 0)
            * XnaMatrix.CreateScale(1, -1, 1);
        }

        static XnaMatrix GetControlTransformation(VisualElement element)
        {
            var absAnchorX = (float)(element.Bounds.Width * element.AnchorX);
            var absAnchorY = (float)(element.Bounds.Height * element.AnchorY);

            var offset = new XnaVector2(
                             (float)(element.Bounds.X + element.TranslationX - (absAnchorX * element.Scale - absAnchorX)),
                             (float)(element.Bounds.Y + element.TranslationY - (absAnchorY * element.Scale - absAnchorY))
                         );

            return XnaMatrix.CreateTranslation(-absAnchorX, -absAnchorY, 0f)
            * XnaMatrix.CreateRotationX(XnaMathHelper.ToRadians((float)element.RotationX))
            * XnaMatrix.CreateRotationY(XnaMathHelper.ToRadians((float)element.RotationY))
            * XnaMatrix.CreateRotationZ(XnaMathHelper.ToRadians((float)element.Rotation))
            * XnaMatrix.CreateScale((float)element.Scale)
            * XnaMatrix.CreateTranslation(absAnchorX * (float)element.Scale, absAnchorY * (float)element.Scale, 0f)
            * XnaMatrix.CreateTranslation(new XnaVector3(offset, 0));
        }

        #endregion

        #region Child track

        void Model_ChildAdded(object sender, ElementEventArgs e)
        {
            var childRenderer = Create((VisualElement)e.Element);
            childRenderer.Parent = this;
            RefreshRenderers();
        }

        void Model_ChildRemoved(object sender, ElementEventArgs e)
        {
            VisualElementRenderer childRenderer;
            if (ChildrenRenderers.TryGetValue(e.Element, out childRenderer))
            {
                SetRenderer(e.Element, null);
                childRenderer.Dispose();
            }
            RefreshRenderers();
        }

        void RefreshRenderers()
        {
            var children = Model == null ?
                ImmutableList<VisualElement>.Empty :
                                            ((IVisualElementController)Model).LogicalChildren.OfType<VisualElement>();

            children = children.Union(_manuallyAddedElements);

            Children = children.Select(c => GetRenderer(c) ?? Create(c)).ToImmutableList();
            ChildrenRenderers = children.ToImmutableDictionary(c => (Element)c, c => GetRenderer(c));
        }

        protected void AddElement(VisualElement element)
        {
            if (_manuallyAddedElements.Contains(element))
                return;

            _manuallyAddedElements.Add(element);
            element.Parent = Model;
            Model_ChildAdded(this, new ElementEventArgs(element));
            ((IVisualElementController)element).NativeSizeChanged();
            GetRenderer(element)._renderParent = _renderParent ?? this;
        }

        protected void RemoveElement(VisualElement element)
        {
            if (_manuallyAddedElements.Remove(element))
                Model_ChildRemoved(this, new ElementEventArgs(element));
        }

        #endregion

        #region Protected Methods

        protected void InvalidateMeasure()
        {
            if (Model != null)
                ((IVisualElementController)Model).NativeSizeChanged();
            InvalidateVisual();
        }

        protected void InvalidateVisual()
        {
            _validVisual = false;
        }

        protected virtual void OnModelUnload(VisualElement model)
        {
            foreach (var c in ((IVisualElementController)model).LogicalChildren)
            {
                var childRenderer = GetRenderer(c);
                if (childRenderer != null)
                {
                    SetRenderer(c, null);
                    childRenderer.Dispose();
                }
            }

            System.Diagnostics.Debug.Assert(Model == null);
            RefreshRenderers();
            model.ChildAdded -= Model_ChildAdded;
            model.ChildRemoved -= Model_ChildRemoved;
        }

        protected virtual void OnModelLoad(VisualElement model)
        {
            RefreshRenderers();

            model.ChildAdded += Model_ChildAdded;
            model.ChildRemoved += Model_ChildRemoved;
        }

        protected virtual float GetAlpha()
        {
            var alpha = Model.Opacity;
            var current = Model.Parent;
            while (current != null)
            {
                var visualParent = current as VisualElement;
                if (visualParent != null)
                    alpha *= visualParent.Opacity;

                current = current.Parent;
            }
            return (float)alpha;
        }

        #endregion

        #region Input

        public virtual void OnMouseEnter(MouseEventArgs e)
        {
            AddVisualState(Mouse.Over);
        }

        public virtual void OnMouseLeave(MouseEventArgs e)
        {
            RemoveVisualState(Mouse.Over);
            RemoveVisualState(Mouse.Pressed);
        }

        public virtual void OnMouseOver(MouseEventArgs e)
        {
            if (_lastMousePosition == e.Position)
                return;
            _lastMousePosition = e.Position;

            var handler = OnMouseMove;
            if (handler != null)
                handler(this, e);
        }

        internal void RaiseClick(MouseEventArgs e)
        {
            var handler = OnMouseClick;
            if (handler != null)
                handler(this, e);
        }

        public virtual bool InterceptMouseDown(MouseButtonEventArgs e)
        {
            return false;
        }

        public virtual bool HandleMouseDown(MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
                AddVisualState(Mouse.Pressed);
                return true;
            }
            return false;
        }

        public virtual bool InterceptMouseUp(MouseButtonEventArgs e)
        {
            return false;
        }

        public virtual bool HandleMouseUp(MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
                RemoveVisualState(Mouse.Pressed);
                return true;
            }
            return false;
        }

        #endregion

        #region State

        protected void AddVisualState(State state, params State[] additionalStates)
        {
            UpdateStates((b, s) => b.Add(s), state, additionalStates);
        }

        protected void RemoveVisualState(State state, params State[] additionalStates)
        {
            UpdateStates((b, s) => b.Remove(s), state, additionalStates);
        }

        void UpdateStates(Func<ImmutableHashSet<State>.Builder, State, bool> updateStates, State state, params State[] additionalStates)
        {
            if (state == null || additionalStates.Any(s => s == null))
                throw new ArgumentNullException();

            var builder = ((ImmutableHashSet<State>)VisualState).ToBuilder();

            bool updated = updateStates(builder, state);
            foreach (var adstate in additionalStates)
                updated |= updateStates(builder, adstate);

            if (updated)
            {
                VisualState = builder.ToImmutable();
                var handler = OnVisualStateChange;
                InvalidateMeasure();
                if (handler != null)
                    handler(this, VisualState);
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            _disposed = true;
            if (disposing)
            {
                foreach (var rend in ChildrenRenderers.Values)
                    rend.Dispose();

                SpriteBatch.Dispose();
                Effect.Dispose();
                _blendState.Dispose();
                if (_backgroundTexture != null)
                    _backgroundTexture.Dispose();
                if (_rendererVisual != null)
                    _rendererVisual.Dispose();
            }
        }

        #endregion
    }
}
