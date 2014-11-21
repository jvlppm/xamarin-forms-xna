namespace Xamarin.Forms.Platforms.Xna
{
    using System.Collections.Generic;
    using Xamarin.Forms;
    using Xamarin.Forms.Platforms.Xna.Context;

    class ContextTicker : Ticker
    {
        readonly GameContext _context;
        bool _enabled = true;
        readonly IDictionary<int, ContextOperation> _handles = new Dictionary<int, ContextOperation>();
        int _nextOperationHandle = 1;

        public ContextTicker(GameContext context)
        {
            _context = context;
        }
        protected override void EnableTimer()
        {
            _enabled = true;
        }

        protected override void DisableTimer()
        {
            _enabled = false;
        }

        public override int Insert(System.Func<long, bool> timeout)
        {
            var handle = _nextOperationHandle++;
            var op = _context.CompleteWhen(g =>
            {
                if (!_enabled)
                    return true;
                    bool res;
                try
                {
                    res = timeout((long)g.ElapsedGameTime.TotalMilliseconds);
                }
                catch { res = false; }
                if (!res)
                    _handles.Remove(handle);
                return !res;
            });

            _handles.Add(handle, op);
            return handle;
        }

        public override void Remove(int handle)
        {
            ContextOperation op;
            if (_handles.TryGetValue(handle, out op))
            {
                op.Operation.Cancel();
                _handles.Remove(handle);
            }
        }
    }
}
