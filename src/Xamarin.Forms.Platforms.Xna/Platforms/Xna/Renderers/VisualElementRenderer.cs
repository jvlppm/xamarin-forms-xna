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
    using XnaBasicEffect = Microsoft.Xna.Framework.Graphics.BasicEffect;
    using XnaMathHelper = Microsoft.Xna.Framework.MathHelper;
    using XnaMatrix = Microsoft.Xna.Framework.Matrix;
    using XnaSpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;
    using XnaTexture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
    using XnaVector2 = Microsoft.Xna.Framework.Vector2;
    using XnaVector3 = Microsoft.Xna.Framework.Vector3;
    using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

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

    public class VisualElementRenderer : IRegisterable
    {
        #region Static

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

        #region Attributes

        public readonly XnaBasicEffect Effect;

        protected readonly PropertyTracker PropertyTracker;
        protected readonly XnaSpriteBatch SpriteBatch;

        Rectangle _lastArrangeBounds;

        XnaRectangle _backgroundArea;
        XnaTexture2D _backgroundTexture;

        VisualElement _model;
        List<Element> _manuallyAddedElements;
        float? _alpha;
        bool _isVisible;

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

            Effect = new XnaBasicEffect(Forms.Game.GraphicsDevice)
            {
                TextureEnabled = true,
                VertexColorEnabled = true
            };

            _manuallyAddedElements = new List<Element>();

            PropertyTracker = new PropertyTracker();
            SpriteBatch = new XnaSpriteBatch(Forms.Game.GraphicsDevice);
            ChildrenRenderers = ImmutableDictionary<Element, VisualElementRenderer>.Empty;
            Children = ImmutableList<VisualElementRenderer>.Empty;
            IsVisible = true;

            PropertyTracker.AddHandler(VisualElement.AnchorXProperty, Handle_Transformation);
            PropertyTracker.AddHandler(VisualElement.AnchorYProperty, Handle_Transformation);
            PropertyTracker.AddHandler(VisualElement.RotationXProperty, Handle_Transformation);
            PropertyTracker.AddHandler(VisualElement.RotationYProperty, Handle_Transformation);
            PropertyTracker.AddHandler(VisualElement.RotationProperty, Handle_Transformation);
            PropertyTracker.AddHandler(VisualElement.ScaleProperty, Handle_Transformation);
            PropertyTracker.AddHandler(VisualElement.OpacityProperty, Handle_Opacity);
            PropertyTracker.AddHandler(VisualElement.BackgroundColorProperty, Handle_BackgroundColor);
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

        public void Layout(Rectangle bounds)
        {
            Model.Layout(bounds);
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
            BeginDraw();
            LocalDraw(gameTime, new XnaRectangle(0, 0, (int)Model.Bounds.Size.Width, (int)Model.Bounds.Size.Height));
            EndDraw();
        }

        protected virtual void BeginDraw()
        {
            if (Model.Bounds != _lastArrangeBounds)
                Arrange();

            var state = Microsoft.Xna.Framework.Graphics.RasterizerState.CullNone;
            var blendState = new Microsoft.Xna.Framework.Graphics.BlendState
            {
                ColorSourceBlend = Microsoft.Xna.Framework.Graphics.Blend.SourceAlpha,
                AlphaSourceBlend = Microsoft.Xna.Framework.Graphics.Blend.SourceAlpha,

                ColorDestinationBlend = Microsoft.Xna.Framework.Graphics.Blend.InverseSourceAlpha,
                AlphaDestinationBlend = Microsoft.Xna.Framework.Graphics.Blend.InverseSourceAlpha
            };

            Effect.Alpha = (_alpha = _alpha ?? GetAlpha()).Value;
            SpriteBatch.Begin(Microsoft.Xna.Framework.Graphics.SpriteSortMode.Immediate, blendState, null, null, state, Effect);

            if (_backgroundTexture != null)
                SpriteBatch.Draw(_backgroundTexture, _backgroundArea, Microsoft.Xna.Framework.Color.White);
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

        void Handle_BackgroundColor(BindableProperty prop)
        {
            if (Model.BackgroundColor == default(Color))
                _backgroundTexture = null;
            else
            {
                _backgroundTexture = new XnaTexture2D(SpriteBatch.GraphicsDevice, 1, 1);
                _backgroundTexture.SetData(new[] { Model.BackgroundColor.ToXnaColor() });
            }
        }

        public virtual void InvalidateAlpha()
        {
            _alpha = null;
            foreach (var childRenderer in Children)
                childRenderer.InvalidateAlpha();
        }

        #endregion

        #region 3D Transformations

        public virtual void InvalidateTransformations()
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

            var viewport = Forms.Game.GraphicsDevice.Viewport;

            float dist = (float)Math.Max(viewport.Width, viewport.Height) * 2;
            var angle = (float)Math.Atan(((float)viewport.Height / 2) / dist) * 2;

            return XnaMatrix.CreateTranslation(-(float)viewport.Width / 2 - 0.5f, -(float)viewport.Height / 2 - 0.5f, -dist)
                 * XnaMatrix.CreatePerspectiveFieldOfView(angle, ((float)viewport.Width / viewport.Height), dist * 0.8f, dist * 2)
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
            Children = Model.LogicalChildren.Select(c => GetRenderer(c)).ToImmutableList();
            ChildrenRenderers = Model.LogicalChildren.ToImmutableDictionary(c => c, c => GetRenderer(c));
        }

        void Model_ChildRemoved(object sender, ElementEventArgs e)
        {
            Children = Model.LogicalChildren.Select(c => GetRenderer(c)).ToImmutableList();
            ChildrenRenderers = Model.LogicalChildren.ToImmutableDictionary(c => c, c => GetRenderer(c));

            VisualElementRenderer childRenderer;
            if (ChildrenRenderers.TryGetValue(e.Element, out childRenderer))
                childRenderer.Parent = null;
        }

        protected void AddElement(Element element)
        {
            if (_manuallyAddedElements.Contains(element))
                return;

            _manuallyAddedElements.Add(element);
            element.Parent = Model;
            Model_ChildAdded(this, new ElementEventArgs(element));
        }

        protected void RemoveElement(Element element)
        {
            if (_manuallyAddedElements.Remove(element))
                Model_ChildRemoved(this, new ElementEventArgs(element));
        }

        #endregion

        #region Protected Methods

        protected void InvalidateMeasure()
        {
            Model.NativeSizeChanged();
        }

        protected virtual void OnModelUnload(VisualElement model)
        {
            Children = ImmutableList<VisualElementRenderer>.Empty;
            ChildrenRenderers = ImmutableDictionary<Element, VisualElementRenderer>.Empty;
            model.ChildAdded -= Model_ChildAdded;
            model.ChildRemoved -= Model_ChildRemoved;
        }

        protected virtual void OnModelLoad(VisualElement model)
        {
            Children = model.LogicalChildren.Select(c => VisualElementRenderer.Create((VisualElement)c)).ToImmutableList();
            ChildrenRenderers = model.LogicalChildren.ToImmutableDictionary(c => c, c => GetRenderer(c));

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

        public virtual void OnMouseEnter()
        {
        }

        public virtual void OnMouseLeave()
        {
        }

        public virtual bool InterceptMouseDown(Mouse.Button button)
        {
            return false;
        }

        public virtual bool HandleMouseDown(Mouse.Button button)
        {
            return false;
        }

        public virtual bool InterceptMouseUp(Mouse.Button button)
        {
            return false;
        }

        public virtual bool HandleMouseUp(Mouse.Button button)
        {
            return false;
        }

        public virtual bool InterceptClick()
        {
            return false;
        }

        public virtual bool HandleClick()
        {
            return false;
        }

        #endregion
    }
}
