#if !INTERNAL_CONTEXT
namespace Xamarin.Forms.Platforms.Xna.Context
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public class ContextTaskAwaiter : INotifyCompletion
    {
        readonly Task _task;
        readonly IGameContext _context;

        public bool IsCompleted { get { return _task.IsCompleted; } }

        public ContextTaskAwaiter(Task task, IGameContext context)
        {
            _task = task;
            _context = context;
        }

        public void GetResult()
        {
            try
            {
                _task.Wait();
            }
            catch (AggregateException ex)
            {
                throw ex.Flatten();
            }
        }

        public void OnCompleted(Action continuation)
        {
            _task.ContinueWith(t => _context.Post(continuation), TaskContinuationOptions.ExecuteSynchronously);
        }
    }

    public class ContextTaskAwaiter<T> : INotifyCompletion
    {
        readonly IGameContext _context;
        readonly Task<T> _task;

        public bool IsCompleted { get { return _task.IsCompleted; } }

        public ContextTaskAwaiter(Task<T> task, IGameContext context)
        {
            _task = task;
            _context = context;
        }

        public T GetResult()
        {
            try
            {
                return _task.Result;
            }
            catch (AggregateException ex)
            {
                throw ex.Flatten();
            }
        }

        public void OnCompleted(Action continuation)
        {
            _task.ContinueWith(t => _context.Post(continuation), TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
#endif
