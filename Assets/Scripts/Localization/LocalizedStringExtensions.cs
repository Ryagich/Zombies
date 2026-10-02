using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Zombies.Localization
{
    public static class LocalizedStringExtensions
    {
        private static readonly Dictionary<(string Locale, string Table, string Entry), string> Cache = new();

        public static string GetLocalizedStringCached(this LocalizedString localizedString)
        {
            if (localizedString == null)
                return string.Empty;

            var locale = LocalizationSettings.SelectedLocale?.Identifier.Code ?? string.Empty;
            var table = localizedString.TableReference.TableCollectionName;
            var entry = localizedString.TableEntryReference.Key;
            var cacheKey = (locale, table, entry);
            if (Cache.TryGetValue(cacheKey, out var value))
                return value;

            value = localizedString.GetLocalizedString();
            Cache[cacheKey] = value;
            return value;
        }
    }
}
