[assembly: Xamarin.Forms.Platforms.Xna.ExportRenderer(
    typeof(Xamarin.Forms.VisualElement),
    typeof(Xamarin.Forms.Platforms.Xna.Renderers.VisualElementRenderer<Xamarin.Forms.VisualElement>))]
namespace Xamarin.Forms.Platforms.Xna.Renderers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Xamarin.Forms;
    using Xamarin.Forms.Platforms.Xna;
    using MathHelper = Microsoft.Xna.Framework.MathHelper;
    using Matrix = Microsoft.Xna.Framework.Matrix;
    using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;
    using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
    using Vector2 = Microsoft.Xna.Framework.Vector2;
    using Vector3 = Microsoft.Xna.Framework.Vector3;

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

    public class VisualElementRenderer : IVisualElementRenderer
    {
        #region Static
        static BindableProperty RendererProperty = BindableProperty.CreateAttached("Renderer", typeof(IVisualElementRenderer), typeof(VisualElementRenderer), null);

        public static IVisualElementRenderer GetRenderer(BindableObject obj)
        {
            return (IVisualElementRenderer)obj.GetValue(RendererProperty);
        }
        public static void SetRenderer(Element obj, IVisualElementRenderer renderer)
        {
            obj.SetValue(RendererProperty, renderer);
        }

        public static IVisualElementRenderer Create(VisualElement element)
        {
            if (!Forms.IsInitialized)
                throw new InvalidOperationException("Xamarin.Forms not initialized");

            if (element == null)
                throw new NotImplementedException();

            var renderer = Registrar.Registered.GetHandler<IVisualElementRenderer>(element.GetType());
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
        Rectangle _transformationBounds;
        Microsoft.Xna.Framework.Rectangle _backgroundArea;
        Microsoft.Xna.Framework.Graphics.BasicEffect Effect;
        VisualElement _model;
        ImmutableDictionary<Element, IVisualElementRenderer> ChildrenRenderers = ImmutableDictionary<Element, IVisualElementRenderer>.Empty;
        List<Element> _manuallyAddedElements;
        float? _alpha;
        Texture2D _backgroundTexture;
        bool _isVisible;

        protected readonly PropertyTracker PropertyTracker;
        protected readonly SpriteBatch SpriteBatch;
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

        public IVisualElementRenderer Parent { get; set; }

        public IEnumerable<IVisualElementRenderer> Children { get { return ChildrenRenderers.Values; } }

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

            IsVisible = true;
            SpriteBatch = new SpriteBatch(Forms.Game.GraphicsDevice);
            PropertyTracker = new PropertyTracker();

            PropertyTracker.AddHandler(VisualElement.AnchorXProperty, Handle_Transformation);
            PropertyTracker.AddHandler(VisualElement.AnchorYProperty, Handle_Transformation);
            PropertyTracker.AddHandler(VisualElement.RotationXProperty, Handle_Transformation);
            PropertyTracker.AddHandler(VisualElement.RotationYProperty, Handle_Transformation);
            PropertyTracker.AddHandler(VisualElement.RotationProperty, Handle_Transformation);
            PropertyTracker.AddHandler(VisualElement.ScaleProperty, Handle_Transformation);
            PropertyTracker.AddHandler(VisualElement.OpacityProperty, Handle_Opacity);
            PropertyTracker.AddHandler(VisualElement.BackgroundColorProperty, Handle_BackgroundColor);

            Effect = new Microsoft.Xna.Framework.Graphics.BasicEffect(Forms.Game.GraphicsDevice)
            {
                TextureEnabled = true,
                VertexColorEnabled = true
            };

            _manuallyAddedElements = new List<Element>();
        }
        #endregion

        #region IRenderer
        public virtual SizeRequest Measure(Size availableSize)
        {
            SizeRequest size = new SizeRequest();

            foreach (var child in Children)
            {
                var visualChild = child as IVisualElementRenderer;
                var c = visualChild != null ? visualChild.Model.GetSizeRequest(availableSize.Width, availableSize.Height)
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
            foreach (var childRenderer in ChildrenRenderers.Values)
                childRenderer.Draw(gameTime);
        }

        protected void Render(Microsoft.Xna.Framework.GameTime gameTime)
        {
            BeginDraw();
            LocalDraw(gameTime);
            EndDraw();
        }

        protected virtual void BeginDraw()
        {
            if (Model.Bounds != _transformationBounds)
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

        protected virtual void LocalDraw(Microsoft.Xna.Framework.GameTime gameTime)
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

            foreach (var childRenderer in ChildrenRenderers.Values)
                childRenderer.Update(gameTime);
        }

        public virtual void Appeared()
        {
            foreach (var child in Children)
                child.Appeared();
        }

        public virtual void Disappeared()
        {
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
            if (Model.BackgroundColor == default(Xamarin.Forms.Color))
                _backgroundTexture = null;
            else
            {
                _backgroundTexture = new Texture2D(SpriteBatch.GraphicsDevice, 1, 1);
                _backgroundTexture.SetData(new[] { Model.BackgroundColor.ToXnaColor() });
            }
        }

        public virtual void InvalidateAlpha()
        {
            _alpha = null;
            foreach (var child in ChildrenRenderers)
            {
                var visualRenderer = child.Value as IVisualElementRenderer;
                visualRenderer.InvalidateAlpha();
            }
        }
        #endregion

        #region 3D Transformations

        public virtual void InvalidateTransformations()
        {
            _transformationBounds = default(Rectangle);
            foreach (var child in ChildrenRenderers)
            {
                var visualRenderer = child.Value as IVisualElementRenderer;
                visualRenderer.InvalidateTransformations();
            }
        }

        protected virtual void Arrange()
        {
            Effect.World = GetWorldTransformation(Model);
            Effect.Projection = GetProjectionMatrix(Model);
            _transformationBounds = Model.Bounds;
            _backgroundArea = new Microsoft.Xna.Framework.Rectangle(0, 0, (int)Model.Bounds.Width, (int)Model.Bounds.Height);
        }

        static Matrix GetWorldTransformation(Element element)
        {
            Matrix world = Matrix.Identity;
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

        static Matrix GetProjectionMatrix(VisualElement element)
        {
            if (element.Bounds.Width <= 0 && element.Bounds.Height <= 0)
                return Matrix.Identity;

            var viewport = Forms.Game.GraphicsDevice.Viewport;

            float dist = (float)Math.Max(viewport.Width, viewport.Height) * 2;
            var angle = (float)System.Math.Atan(((float)viewport.Height / 2) / dist) * 2;

            return Matrix.CreateTranslation(-(float)viewport.Width / 2 - 0.5f, -(float)viewport.Height / 2 - 0.5f, -dist)
                 * Matrix.CreatePerspectiveFieldOfView(angle, ((float)viewport.Width / viewport.Height), 0.001f, dist * 2)
                 * Matrix.CreateScale(1, -1, 1);
        }

        static Matrix GetControlTransformation(VisualElement element)
        {
            var absAnchorX = (float)(element.Bounds.Width * element.AnchorX);
            var absAnchorY = (float)(element.Bounds.Height * element.AnchorY);

            var offset = new Vector2(
                (float)(element.Bounds.X + element.TranslationX - (absAnchorX * element.Scale - absAnchorX)),
                (float)(element.Bounds.Y + element.TranslationY - (absAnchorY * element.Scale - absAnchorY))
            );

            return Matrix.CreateTranslation(-absAnchorX, -absAnchorY, 0f)
                 * Matrix.CreateRotationX(MathHelper.ToRadians((float)element.RotationX))
                 * Matrix.CreateRotationY(MathHelper.ToRadians((float)element.RotationY))
                 * Matrix.CreateRotationZ(MathHelper.ToRadians((float)element.Rotation))
                 * Matrix.CreateScale((float)element.Scale)
                 * Matrix.CreateTranslation(absAnchorX * (float)element.Scale, absAnchorY * (float)element.Scale, 0f)
                 * Matrix.CreateTranslation(new Vector3(offset, 0));
        }
        #endregion

        #region Child track
        void Model_ChildAdded(object sender, ElementEventArgs e)
        {
            var childRenderer = VisualElementRenderer.Create((VisualElement)e.Element);
            childRenderer.Parent = this;
            ChildrenRenderers = ChildrenRenderers.Add(e.Element, childRenderer);
        }

        void Model_ChildRemoved(object sender, ElementEventArgs e)
        {
            IVisualElementRenderer childRenderer;
            if (ChildrenRenderers.TryGetValue((VisualElement)e.Element, out childRenderer))
            {
                childRenderer.Parent = null;
                ChildrenRenderers = ChildrenRenderers.Remove(e.Element);
            }
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
            ChildrenRenderers = ChildrenRenderers.RemoveRange(
                ChildrenRenderers.Keys.Where(i => !_manuallyAddedElements.Contains(i)));

            ChildrenRenderers = null;
            model.ChildAdded -= Model_ChildAdded;
            model.ChildRemoved -= Model_ChildRemoved;
        }

        protected virtual void OnModelLoad(VisualElement model)
        {
            ChildrenRenderers = ChildrenRenderers.AddRange(
                model.LogicalChildren.ToDictionary(c => c, c => VisualElementRenderer.Create((VisualElement)c)));

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
    }
}
