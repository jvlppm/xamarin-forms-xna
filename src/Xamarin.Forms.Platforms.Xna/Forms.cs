﻿namespace Xamarin.Forms
{
    using Microsoft.Xna.Framework;
    using System.Reflection;
    using Xamarin.Forms.Platforms.Xna;
    using Xamarin.Forms.Platforms.Xna.Context;
    using Xamarin.Forms.Platforms.Xna.Resources;

    public static class Forms
    {
        internal static bool IsInitialized;

        public static Game Game { get; private set; }
        public static IPlatformEngine PlatformEngine { get; private set; }

#if !INTERNAL_CONTEXT
        public static GameContext DrawContext { get; private set; }
        public static GameContext UpdateContext { get; private set; }
#endif

        internal static EmbeddedContent EmbeddedContent { get; private set; }

        public static void Init(Game game)
        {
            if (IsInitialized)
                return;

            PlatformServices platformServices = new PlatformServices(game);
#if !INTERNAL_CONTEXT
            DrawContext = platformServices.DrawContext;
            UpdateContext = platformServices.UpdateContext;
#endif

            game.Components.Add(platformServices);
            Device.PlatformServices = platformServices;

            PlatformEngine = new PlatformEngine();

            Game = game;

            Registrar.RegisterAll(new[]{
                typeof(ExportRendererAttribute),
                typeof(ExportImageSourceHandlerAttribute),
            });

            Ticker.Default = new ContextTicker(platformServices.DrawContext);

            EmbeddedContent = new EmbeddedContent(Assembly.GetExecutingAssembly(), game.Services);

            IsInitialized = true;
        }
    }
}
