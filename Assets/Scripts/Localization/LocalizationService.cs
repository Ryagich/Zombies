using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.Localization.Settings;

namespace Zombies.Localization
{
    public static class LocalizationService
    {
        public static async Task SelectLanguageAsync(string languageCode)
        {
            await LocalizationSettings.InitializationOperation.Task;

            var locale = LocalizationSettings.AvailableLocales.Locales.Find(x => x.Identifier.Code == languageCode)
                         ?? LocalizationSettings.AvailableLocales.Locales.Find(x => x.Identifier.Code == "en");

            if (locale != null)
            {
                LocalizationSettings.SelectedLocale = locale;
            }

            var tables = LocalizationSettings.StringDatabase.GetAllTables();
            await tables;
        }
    }
}
