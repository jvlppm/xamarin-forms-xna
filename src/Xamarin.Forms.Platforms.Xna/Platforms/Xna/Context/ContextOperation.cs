namespace Xamarin.Forms.Platforms.Xna.Context
{
    #if !INTERNAL_CONTEXT
    public
    #endif
    class ContextOperation
    {
        public readonly IGameOperation Operation;
        public readonly IGameContext Context;

        public ContextOperation(IGameOperation operation, IGameContext context)
        {
            Operation = operation;
            Context = context;
        }
    }

#if !INTERNAL_CONTEXT
    public
#endif
    class ContextOperation<T> : ContextOperation
    {
        public ContextOperation(IGameOperation<T> operation, IGameContext context)
            : base(operation, context)
        {
        }
    }
}
