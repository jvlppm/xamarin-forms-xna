namespace Xamarin.Forms.Platforms.Xna.Context
{
    using Microsoft.Xna.Framework;
    using System;

    #if !INTERNAL_CONTEXT
    public
    #endif
    class RepeatingTimer : GameOperation, IDisposable
    {
        private bool _disposed = false;

        public TimeSpan DueTime, Period;

        public event EventHandler Ellapsed = delegate { };

        public RepeatingTimer(TimeSpan dueTime, TimeSpan period)
        {
            DueTime = dueTime;
            Period = period;
        }

        public RepeatingTimer(TimeSpan period, bool startImmediately = true)
        {
            DueTime = startImmediately? TimeSpan.Zero : period;
            Period = period;
        }

        public override bool Continue(GameTime gameTime)
        {
            if (_disposed)
                return false;

            DueTime -= gameTime.ElapsedGameTime;
            if (DueTime <= TimeSpan.Zero)
            {
                DueTime = Period;
                Ellapsed(this, EventArgs.Empty);
            }

            return true;
        }

        public virtual void Change(TimeSpan dueTime, TimeSpan period)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            Period = period;
            DueTime = dueTime;
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }

    #if !INTERNAL_CONTEXT
    public
    #endif
    static class RepeatingTimerExtensions
    {
        public static RepeatingTimer StartTimer(this IGameContext context, TimeSpan period, bool startImmediatelly = true)
        {
            var timer = new RepeatingTimer(period, startImmediatelly);
            context.Run(timer);
            return timer;
        }
    }
}
