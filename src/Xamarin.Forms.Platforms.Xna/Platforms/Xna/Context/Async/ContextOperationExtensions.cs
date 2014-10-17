#if !INTERNAL_CONTEXT
namespace Xamarin.Forms.Platforms.Xna.Context
{
    using System.Threading.Tasks;

    public static class ContextOperationExtensions
    {
        public static ContextOperationAwaiter GetAwaiter(this ContextOperation op)
        {
            return new ContextOperationAwaiter(op.Operation, op.Context);
        }

        public static Task AsTask(this ContextOperation op)
        {
            var tcs = new TaskCompletionSource<bool>();
            op.Operation.OnCompleted(() =>
            {
                if (op.Operation.IsFaulted)
                    tcs.SetException(op.Operation.Error);
                else if (op.Operation.IsCanceled)
                    tcs.SetCanceled();
                else
                    tcs.SetResult(true);
            });
            return tcs.Task;
        }

        public static ContextOperationAwaiter<T> GetAwaiter<T>(this ContextOperation<T> op)
        {
            return new ContextOperationAwaiter<T>((IGameOperation<T>)op.Operation, op.Context);
        }

        public static Task<T> AsTask<T>(this ContextOperation<T> op)
        {
            var tcs = new TaskCompletionSource<T>();
            op.Operation.OnCompleted(() =>
            {
                if (op.Operation.IsFaulted)
                    tcs.SetException(op.Operation.Error);
                else if (op.Operation.IsCanceled)
                    tcs.SetCanceled();
                else
                    tcs.SetResult(((IGameOperation<T>)op.Operation).GetResult());
            });
            return tcs.Task;
        }
    }
}
#endif
