#if !INTERNAL_CONTEXT
namespace Xamarin.Forms.Platforms.Xna.Context
{
    using System;

    public static class WhenAnyExtensions
    {
        public static ContextOperation<ContextOperation> WhenAny(this IGameContext context, params ContextOperation[] operations)
        {
            if (operations.Length <= 0)
                throw new ArgumentException("No operations specified", "operations");

            var operation = new DeferredOperation<ContextOperation>();

            foreach (var op in operations)
                op.Operation.OnCompleted(() => operation.SetResult(op));

            return context.Run(operation);
        }

        public static ContextOperation<ContextOperation<T>> WhenAny<T>(this IGameContext context, params ContextOperation<T>[] operations)
        {
            if (operations.Length <= 0)
                throw new ArgumentException("No operations specified", "operations");

            var operation = new DeferredOperation<ContextOperation<T>>();

            foreach (var op in operations)
                op.Operation.OnCompleted(() => operation.SetResult(op));

            return context.Run(operation);
        }
    }
}
#endif
