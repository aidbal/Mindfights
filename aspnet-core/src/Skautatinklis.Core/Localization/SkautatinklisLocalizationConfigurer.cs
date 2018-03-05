using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace Skautatinklis.Localization
{
    public static class SkautatinklisLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(SkautatinklisConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(SkautatinklisLocalizationConfigurer).GetAssembly(),
                        "Skautatinklis.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}
