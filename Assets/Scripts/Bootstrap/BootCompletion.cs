using Cysharp.Threading.Tasks;

namespace Zombies.Bootstrap
{
    public sealed class BootCompletion
    {
        private readonly UniTaskCompletionSource completion = new();

        public UniTask WaitAsync() => completion.Task;

        public void Signal()
        {
            if (!completion.Task.Status.IsCompleted())
            {
                completion.TrySetResult();
            }
        }
    }
}
