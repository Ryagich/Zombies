using System.Threading.Tasks;
using YG;

namespace Zombies.Bootstrap
{
    public static class YandexSdkAwaiter
    {
        private static TaskCompletionSource<bool> completion;

        public static Task WaitForSdkDataAsync()
        {
            if (YG2.isSDKEnabled)
            {
                return Task.CompletedTask;
            }

            completion ??= new TaskCompletionSource<bool>();

            void OnSdkDataReceived()
            {
                YG2.onGetSDKData -= OnSdkDataReceived;
                completion.TrySetResult(true);
            }

            YG2.onGetSDKData += OnSdkDataReceived;
            return completion.Task;
        }
    }
}
