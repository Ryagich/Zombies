using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;
using YG;
using YG.Insides;
using Zombies.Localization;

namespace Zombies.Bootstrap
{
    /// <summary>Initializes the Yandex Games platform and Unity localization before scenes build their session scopes.</summary>
    public sealed class ProjectBootloader : IStartable
    {
        private readonly BootCompletion bootCompletion;

        public ProjectBootloader(BootCompletion bootCompletion)
        {
            this.bootCompletion = bootCompletion;
        }

        public async void Start()
        {
            await StartAsync();
        }

        public async UniTask StartAsync(CancellationToken cancellationToken = default)
        {
            await YandexSdkAwaiter.WaitForSdkDataAsync();
            cancellationToken.ThrowIfCancellationRequested();

            YG2.InitMetrica();
            YG2.GetAuth();
            YG2.GetLanguage();
            YGInsides.LoadProgress();

            await LocalizationService.SelectLanguageAsync(YG2.lang);
            YG2.GameReadyAPI();

            YG2.MetricaSend("GameReady");

            bootCompletion.Signal();
        }
    }
}
