namespace Xamarin.Forms.Platforms.Xna
{
    using Xamarin.Forms.Platforms.Xna.Context;
    using Microsoft.Xna.Framework;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;
#if !PORTABLE
    using System.IO.IsolatedStorage;
    using System.Text.RegularExpressions;
    using System.Reflection;
    using Xamarin.Forms.Platforms.Xna.Resources;
#endif

    class PlatformServices : DrawableGameComponent, IPlatformServices
    {
        public readonly GameContext DrawContext;
        public readonly GameContext UpdateContext;
        readonly HttpClient HttpClient;
        readonly SynchronizationContext MainThreadContext;
        readonly Regex PackUriRx = new Regex(@"^pack://(?<location>[^/]+)/((?<assembly>((?!;component/).)+);component/)?(?<path>.*)$");

        public PlatformServices(Game game)
            : base(game)
        {
            DrawContext = new GameContext();
            UpdateContext = new GameContext();
            HttpClient = new HttpClient();
            MainThreadContext = SynchronizationContext.Current;
        }

        public override void Update(GameTime gameTime)
        {
            UpdateContext.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            DrawContext.Update(gameTime);
            base.Draw(gameTime);
        }

        public void BeginInvokeOnMainThread(Action action)
        {
            UpdateContext.Post(action);
        }

        public ITimer CreateTimer(Action<object> callback, object state, uint dueTime, uint period)
        {
            return PlatformTimer.StartNew(UpdateContext, callback, state,
                TimeSpan.FromMilliseconds(dueTime),
                TimeSpan.FromMilliseconds(period));
        }

        public ITimer CreateTimer(Action<object> callback, object state, TimeSpan dueTime, TimeSpan period)
        {
            return PlatformTimer.StartNew(UpdateContext, callback, state, dueTime, period);
        }

        public ITimer CreateTimer(Action<object> callback, object state, long dueTime, long period)
        {
            return PlatformTimer.StartNew(UpdateContext, callback, state,
                TimeSpan.FromMilliseconds(dueTime),
                TimeSpan.FromMilliseconds(period));
        }

        public ITimer CreateTimer(Action<object> callback, object state, int dueTime, int period)
        {
            return PlatformTimer.StartNew(UpdateContext, callback, state,
                TimeSpan.FromMilliseconds(dueTime),
                TimeSpan.FromMilliseconds(period));
        }

        public ITimer CreateTimer(Action<object> callback)
        {
            return new PlatformTimer(UpdateContext, callback);
        }

        public void StartTimer(TimeSpan interval, Func<bool> callback)
        {
            PlatformTimer.StartNew(UpdateContext, callback, interval, interval);
        }

        public async Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
        {
            var packMatch = PackUriRx.Match(uri.OriginalString);
            if (packMatch.Success)
            {
                var assemblyName = packMatch.Groups["assembly"].Success ?
                    new AssemblyName(packMatch.Groups["assembly"].Captures[0].Value) :
                    Assembly.GetEntryAssembly().GetName();

                var assembly = AppDomain.CurrentDomain.GetAssemblies()
                    .SingleOrDefault(a => AssemblyName.ReferenceMatchesDefinition(a.GetName(), assemblyName));

                if (assembly == null)
                    return null;

                var path = packMatch.Groups["path"].Captures[0].Value;
                path = path.Replace("/", ".");

                return assembly.GetManifestResourceNames()
                    .Where(n => Regex.IsMatch(n, Regex.Escape(path) + @"(\.[^.]+)?"))
                    .Select(n => assembly.GetManifestResourceStream(n))
                    .FirstOrDefault();
            }

            var webCancellation = new CancellationTokenSource();
            var getWebResponse = HttpClient.GetAsync(uri, webCancellation.Token);

            if (!uri.IsAbsoluteUri)
            {
                try
                {
                    var stream = await Task.Factory.StartNew(() =>
                        TitleContainer.OpenStream(uri.ToString()), cancellationToken);
                    webCancellation.Cancel();
                    return stream;
                }
                catch { }
            }

            cancellationToken.Register(webCancellation.Cancel);

            var response = await getWebResponse;
            return await response.Content.ReadAsStreamAsync();
        }

        public bool IsInvokeRequired
        {
            get { return MainThreadContext == SynchronizationContext.Current; }
        }

        public System.Reflection.Assembly[] GetAssemblies()
        {
#if PORTABLE
            throw new NotImplementedException();
#else
            return AppDomain.CurrentDomain.GetAssemblies();
#endif
        }

        public void OpenUriAction(Uri uri)
        {
#if PORTABLE
            throw new NotImplementedException();
#else
            Process.Start(uri.ToString());
#endif
        }

        public Xamarin.Forms.IIsolatedStorageFile GetUserStoreForApplication()
        {
#if PORTABLE
            throw new NotImplementedException();
#else
            var scope = IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain;
            var isolatedStorage = System.IO.IsolatedStorage.IsolatedStorageFile.GetStore(scope, null, null);
            return new IsolatedStorageFile(isolatedStorage);
#endif
        }


        public double GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes)
        {
            switch (size)
            {
                case NamedSize.Medium:
                case NamedSize.Default:
                    return 14;
                case NamedSize.Large:
                    return 18;
                case NamedSize.Small:
                    return 12;
                case NamedSize.Micro:
                    return 10;
            }
            throw new NotImplementedException();
        }
    }
}
